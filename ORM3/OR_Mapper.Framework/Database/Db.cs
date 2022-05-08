﻿using System.Data;
using Npgsql;
using OR_Mapper.Framework.Caching;
using OR_Mapper.Framework.Exceptions;
using OR_Mapper.Framework.Extensions;

namespace OR_Mapper.Framework.Database
{
    /// <summary>
    /// This class contains all Db related information.
    /// </summary>
    public static class Db
    {
        /// <summary>
        /// Holds the Db connection string.
        /// </summary>
        public static string? ConnectionString;

        /// <summary>
        /// Holds the Db schema name.
        /// </summary>
        public static string? DbSchema { get; set; }

        /// <summary>
        /// Holds the cache.
        /// </summary>
        public static ICache Cache { get; set; } = new Cache();

        /// <summary>
        /// Gets the Db connection.
        /// </summary>
        /// <returns>Connection.</returns>
        public static IDbConnection GetConnection() => new NpgsqlConnection(ConnectionString);
        
        
        /// <summary>
        /// Saves given entity in Db (insert or update if it already exists).
        /// </summary>
        /// <param name="entity">Entity.</param>
        public static void Save(Entity entity)
        {
            if (!Cache.HasChanged(entity))
            {
                return;
            }
            var conn = Connect();
            var type = entity.GetType();
            var model = new Model(type);

            string sql = $"INSERT INTO {GetTableName(model.TableName)} (";

            var columnNames = model.Fields.Select(x => x.ColumnName);
            sql += string.Join(',', columnNames);
            sql += ") VALUES (";
            var cmd = conn.CreateCommand();

            var modelFieldsWithoutFk = model.Fields.Where(x => !x.IsForeignKey).ToList();
            var insertParameters = new List<string>();
            var parameterCount = 0;
            
            // Build parameters for internal fields
            foreach (var fields in modelFieldsWithoutFk)
            {
                string paramName = "@p" + parameterCount;
                insertParameters.Add(paramName);
                
                var paramValue = fields.GetValue(entity) ?? DBNull.Value;
                
                if (fields.ColumnType.IsEnum && paramValue != DBNull.Value)
                {
                    paramValue = (int) paramValue;
                }
                
                cmd.AddParameter(paramName, paramValue);
                parameterCount++;
            }

            var modelFieldsWithFk = model.Fields.Where(x => x.IsForeignKey).ToList();
            
            // Build parameters for foreign keys
            foreach (var fields in modelFieldsWithFk)
            {
                string paramName = $"@p{parameterCount}";
                insertParameters.Add(paramName);

                var foreignKey = model.ForeignKeys.First(x => x.LocalColumn == fields);
                var correspondingField = model.ExternalFields.First(x => x.Model == foreignKey.ForeignTable);
		
                var externalEntity = correspondingField.GetValue(entity);
                var pk = correspondingField.Model.PrimaryKey.GetValue(externalEntity);
                cmd.AddParameter(paramName, pk ?? DBNull.Value);
                parameterCount++;
            }
            
            sql += string.Join(',', insertParameters);
            sql += ") ";
	
            // Update row on duplicate primary key
            sql += $"ON CONFLICT ({model.PrimaryKey.ColumnName}) DO UPDATE SET ";
	
            parameterCount = 0;
            var updateParameters = new List<string>();
            
            foreach (var column in columnNames)
            {
                string paramName = $"{column} = @p{parameterCount}";
                updateParameters.Add(paramName);
                parameterCount++;
            }
            
            sql += string.Join(',', updateParameters);

            // Return primary key
            sql += $" RETURNING {model.PrimaryKey.ColumnName}";

            // Execute command
            try
            {
                cmd.CommandText = sql;
                //Console.WriteLine(sql);
                var pk = cmd.ExecuteScalar();
                Cache.Add(entity);
                //Console.WriteLine("Added successfully.");
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("A database error occurred, see inner exception for details.", ex);
            }

            conn.Close();
        }

        /// <summary>
        /// Deletes given entity from Db.
        /// </summary>
        /// <param name="entity"></param>
        public static void Delete(Entity entity)
        {
            var conn = Connect();
            var type = entity.GetType();
            var model = new Model(type);

            string sql = $"DELETE FROM {GetTableName(model.TableName)} WHERE id=@id";
            
            var cmd = conn.CreateCommand();
            
            var pk = model.Fields.First(x => x.IsPrimaryKey);
            var pkValue = pk.GetValue(entity);
            cmd.AddParameter("@id", pkValue);
            
            cmd.CommandText = sql;

            try
            {
                var success = cmd.ExecuteScalar();
                Cache.Remove(entity);
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("A database error occurred, see inner exception for details.", ex);
            }

            conn.Close();
        }

        /// <summary>
        /// Gets all objects of specified class from database.
        /// </summary>
        /// <typeparam name="TEntity">Class of entity.</typeparam>
        /// <returns>Queried objects.</returns>
        public static List<TEntity> GetAll<TEntity>() where TEntity : class, new()
        {
            var cache = Cache.GetAll<TEntity>();
            if (cache.Count > 0)
            {
                return cache.ToList();
            }
            
            var model = new Model(typeof(TEntity));
            var conn = Connect();
            string sql = $"SELECT * FROM {GetTableName(model.TableName)}";

            var cmd = conn.CreateCommand();
            
            // Execute command
            try
            {
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                var loader = new ObjectLoader(reader);
                var entityList = loader.LoadCollection<TEntity>();
                conn.Close();
                Cache.AddCollection(entityList);
                return entityList;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("A database error occurred, see inner exception for details.", ex);
            }
            
            return null;
        }
        
        /// <summary>
        /// Gets object by given id of specified class from database.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <typeparam name="TEntity">Class of entity.</typeparam>
        /// <returns>Queried object.</returns>
        public static TEntity? GetById<TEntity>(int id) where TEntity : new()
        {
            if (Cache.ExistsById(id, typeof(TEntity)))
            {
                return (TEntity) Cache.Get(id, typeof(TEntity));
            }
            
            var model = new Model(typeof(TEntity));
            var conn = Connect();
            string sql = $"SELECT * FROM {GetTableName(model.TableName)}";
            sql += $" WHERE id = @id";

            var cmd = conn.CreateCommand();
            cmd.AddParameter("@id", id);
            
            // Execute command
            try
            {
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                var loader = new ObjectLoader(reader);
                var entity = loader.LoadSingle<TEntity>();
                conn.Close();
                if (entity != null)
                {
                    Cache.Add(entity);
                    return entity;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("A database error occurred, see inner exception for details.", ex);
            }
            
            return new TEntity();
        }
        
        /// <summary>
        /// Helper method for connecting to db.
        /// </summary>
        /// <returns></returns>
        private static IDbConnection Connect()
        {
            try
            {
                var conn = GetConnection();
                conn.Open();
                return conn;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("A database error occurred, see inner exception for details.", ex);
            }

            return null;
        }
        
        /// <summary>
        /// Loads objects that are in a one to one relation. Also used for many to one relations.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <param name="field">Field.</param>
        /// <typeparam name="TCorrespondingType">Type of corresponding class.</typeparam>
        /// <returns>Corresponding object.</returns>
        public static TCorrespondingType LoadOneToOne<TCorrespondingType>(object record, ExternalField field) where TCorrespondingType : new()
        {
            var conn = Connect();
            
            // Get foreign information if foreign key is in current table
            var mainModel = new Model(record.GetType());
            var foreignModel = field.Model;
            
            var columnNames = field.Model.Fields.Select(x => "v." + x.ColumnName);
            var allColumns = string.Join(',', columnNames);
            
            var fk = foreignModel.ForeignKeys.FirstOrDefault(x => x.ForeignTable.Member == mainModel.Member);
            var joinPredicate = "";

            // foreign key is in current
            if (fk is null)
            {
                fk = mainModel.ForeignKeys.First(x => x.ForeignTable.Member == foreignModel.Member);
                joinPredicate = $"v.{foreignModel.PrimaryKey.ColumnName} = {GetTableName(mainModel.TableName)}.{fk.LocalColumn.ColumnName}";
            }
            // foreign key is in the other
            else
            {
                joinPredicate = $"{GetTableName(mainModel.TableName)}.{mainModel.PrimaryKey.ColumnName} = v.{fk.LocalColumn.ColumnName}";
            }
            
            var sql = "";

            sql += $"SELECT {allColumns} FROM {GetTableName(mainModel.TableName)} " +
                   $"JOIN {GetTableName(foreignModel.TableName)} v " +
                   $"ON {joinPredicate} " +
                   $"WHERE {GetTableName(mainModel.TableName)}.{mainModel.PrimaryKey.ColumnName} = @p0";
                
            var cmd = conn.CreateCommand();
            cmd.AddParameter("@p0", mainModel.PrimaryKey.GetValue(record));
            
            // Execute command
            try
            {
               // Console.WriteLine($"Executing sql: {sql}");
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                var loader = new ObjectLoader(reader);
                var entity = loader.LoadSingle<TCorrespondingType>();
                conn.Close();
                return entity;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("A database error occurred, see inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Loads objects that are in a many to many relation.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <param name="field">Field.</param>
        /// <exception cref="NotImplementedException">NotImplementedException.</exception>
        public static void LoadManyToMany(object record, ExternalField field)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Loads objects that are in a one to many relation.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <param name="field">Field.</param>
        /// <typeparam name="TCorrespondingType">Type of corresponding class.</typeparam>
        /// <returns>Corresponding object.</returns>
        public static List<TCorrespondingType> LoadOneToMany<TCorrespondingType>(object record, ExternalField field) where TCorrespondingType : new()
        {
            var conn = Connect();
            
            // Get foreign information if foreign key is in other table
            var mainModel = new Model(record.GetType());
            var foreignModel = field.Model;

            var fields = foreignModel.Fields.Select(x => x.ColumnName);
            var foreignKey = foreignModel.ForeignKeys.First(_ => _.ForeignTable.TableName == mainModel.TableName);
            var fieldsString = string.Join(',', fields);

            var sql = $"SELECT {fieldsString} " + 
                           $"FROM {GetTableName(foreignModel.TableName)} " + 
                           $"WHERE {foreignKey.LocalColumn.ColumnName} = @p0";
            
            var cmd = conn.CreateCommand();
            cmd.AddParameter("@p0", mainModel.PrimaryKey.GetValue(record));
            
            // Execute command
            try
            {
             //  Console.WriteLine($"Executing sql: {sql}");
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                var loader = new ObjectLoader(reader);
                var entityList = loader.LoadCollection<TCorrespondingType>();
                conn.Close();
                return entityList;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("A database error occurred, see inner exception for details.", ex);
            }

        }

        /// <summary>
        /// Gets the correct table name including the db schema if needed.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal static string GetTableName(string tableName)
        {
            return DbSchema is null ? tableName : $"{DbSchema}.{tableName}";
        }

    }
    
}