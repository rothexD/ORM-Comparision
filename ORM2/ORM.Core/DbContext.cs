using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml.Schema;
using ORM.Core.Configuration;
using ORM.Core.Interfaces;
using ORM.Core.Loading;
using ORM.Core.Models;
using ORM.Core.Models.Enums;
using ORM.Core.Models.Exceptions;
using ORM.Core.Models.Extensions;

namespace ORM.Core
{
    /// <summary>
    /// Database context for performing methods in the current database context
    /// </summary>
    public class DbContext : IDbContext
    {
        /// <summary>
        /// Builds commands that are executed against a database connection
        /// </summary>
        public ICommandBuilder _commandBuilder;
        
        /// <summary>
        /// Executes LINQ expressions against a database connection
        /// </summary>
        public IQueryProvider _queryProvider;
        
        /// <summary>
        /// Cache for storing entities
        /// </summary>
        public ICache? _cache;

        /// <summary>
        /// Configuration of the current database context
        /// </summary>
        private static OptionsBuilder _options = new OptionsBuilder();

        protected DbContext()
        {
            _cache = _options.Cache;
            _commandBuilder = _options.CommandBuilder ?? throw new OrmException($"No command builder is defined. Please configure a database by writing for example {nameof(DbContext)}.{nameof(Configure)}(c => c.UsePostgres());");
            _queryProvider = _options.QueryProvider ?? throw new OrmException($"No query provider is defined. Please configure a database by writing for example {nameof(DbContext)}.{nameof(Configure)}((c => c.UsePostgres());");
            InitializeDbContext();
        }
        
        /// <summary>
        /// Apply a configuration to this database context.
        /// </summary>
        /// <param name="configure"></param>
        public static void Configure(Action<OptionsBuilder> configure)
        {
            configure(_options);
        }

        /// <summary>
        /// Resets the applied configuration for this database context.
        /// </summary>
        public static void ResetConfiguration()
        {
        }

        /// <summary>
        /// Creates the database schema
        /// </summary>
        /// <param name="assembly"></param>
        public void EnsureCreated(Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            var tables = GetTables(assembly).ToList();
            var cmd = _commandBuilder.BuildEnsureCreated(tables);
            cmd.ExecuteNonQuery();
        }
        
        /// <summary>
        /// Saves an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        public void Save<T>(T entity)
        {
            if (entity is null)
            {
                throw new OrmException("Entity is set to null");
            }
            
            // Update references on object
            UpdateReferences(entity);
            
            // Do nothing if cached entity did not changed
            if (_cache is not null && !_cache.HasChanged(entity))
            {
                return;
            }

            // Save entity
            SaveEntity(entity);
            SaveReferences(entity);
            
            // Save entity in cache
            _cache?.Save(entity);
        }
        
        /// <summary>
        /// Saves an entity as-is and updates its primary key
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private object SaveEntity<T>(T entity)
        {
            if (entity is null)
            {
                throw new OrmException("Entity is set to null");
            }
            
            // Save entity
            var saveCmd = _commandBuilder.BuildSave(entity);
            object? pk = saveCmd.ExecuteScalar();

            if (pk is null)
            {
                throw new OrmException("Primary key was not returned from save");
            }
                    
            // Update primary key on entity
            var type = entity.GetType();
            var entityTable = type.ToTable();
            entityTable.PrimaryKey.SetValue(entity, pk);
            return pk;
        }

        /// <summary>
        /// Updates references on an entity
        /// </summary>
        /// <param name="entity"></param>
        private void UpdateReferences(object entity)
        {
            var entityType = entity.GetType();
            var entityTable = entityType.ToTable();
            var collectionReference = entityTable.GetPropertiesOf(RelationshipType.OneToMany);
            
            // Update references that are collections (one to many)
            foreach (var property in collectionReference)
            {
                var collection = property.GetValue(entity) as IEnumerable<object>;

                // if collection is null, initialize list
                if (collection is null)
                {
                    object? instance = Activator.CreateInstance(property.PropertyType);
                    collection = instance as IEnumerable<object> ?? new List<object>();
                    property.SetValue(entity, instance);
                }

                collection = collection.Where(x => x is not null);
                
                // Update references to this entity in the items of the list
                foreach (object reference in collection)
                {
                    var referenceType = reference.GetType();
                    var navigatedProperty = referenceType.GetNavigatedProperty(entityType);
                    navigatedProperty?.SetValue(reference, entity);
                }
            }
            
            var singleReferences = entityTable.GetPropertiesOf(RelationshipType.ManyToOne);

            // Update single references (many to one)
            foreach (var property in singleReferences)
            {
                object? reference = property.GetValue(entity);

                // If reference is null, throw an error, as its primary key needs to be stored as a foreign key
                if (reference is null)
                {
                    throw new OrmException($"Can't save entity of type '{entityType.Name}', because the " +
                                           $"many-to-one property '{property.Name}' is set to null. Please first save " +
                                           $"an entity of type '{property.PropertyType.Name}' using " +
                                           $"{nameof(DbContext)}.{nameof(Save)}() and then set the property.");
                }

                var referenceType = reference.GetType();
                var referenceTable = referenceType.ToTable();
                object? referencePk = referenceTable.PrimaryKey.GetValue(reference);

                // If the reference is set, but not saved yet, save it
                if (_cache is not null && _cache.Get(referenceType, referencePk) is null)
                {
                    SaveEntity(reference);
                }
                
                // Get collection on reference
                var navigatedProperty = property.PropertyType.GetNavigatedProperty(entityType) ?? throw new OrmException($"Could not find navigated property of type {entityType.Name} on type {property.PropertyType.Name}");
                var navigatedEnumerable = navigatedProperty.GetValue(reference) as IEnumerable<object>;

                // if collection is null, create it
                if (navigatedEnumerable is null)
                {
                    navigatedEnumerable = Activator.CreateInstance(navigatedProperty.PropertyType) as IEnumerable<object>;
                    navigatedProperty.SetValue(reference, navigatedEnumerable);
                }
                
                var navigatedList = navigatedEnumerable?.ToList();
                
                // If entity is not stored in reference collection, add it
                if (navigatedList is not null && !navigatedList.Contains(entity))
                {
                    navigatedList.Add(entity);
                    var itemType = navigatedProperty.PropertyType.GetUnderlyingType();

                    var x = navigatedList.OfType<object>();
                    
                    // Convert list to correct type as List<object> can not be set on the entity
                    var ofTypeMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.OfType));
                    var toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));
                    var typedOfTypeMethod = ofTypeMethod?.MakeGenericMethod(itemType);
                    var typedToListMethod = toListMethod?.MakeGenericMethod(itemType);
                    
                    object? typedEnumerable = typedOfTypeMethod?.Invoke(null, new object?[]{ navigatedList });
                    object? typedList = typedToListMethod?.Invoke(null, new[] { typedEnumerable });
                    
                    // Set property 
                    navigatedProperty.SetValue(reference, typedList);
                }
            }
        }

        /// <summary>
        /// Saves the references of an entity in the database
        /// </summary>
        /// <param name="entity"></param>
        private void SaveReferences(object entity)
        {
            var entityType = entity.GetType();
            var entityTable = entityType.ToTable();

            var references = entityTable.GetPropertiesOf(RelationshipType.ManyToMany);
            
            // Save many to many references
            foreach (var property in references)
            {
                var entries = property.GetValue(entity) as IEnumerable<object>;
                
                // if collection is null, create it
                if (entries is null)
                {
                    entries = Activator.CreateInstance(property.PropertyType) as IEnumerable<object>;
                    property.SetValue(entity, entries);
                }
                
                entries = entries.ToList();
                
                var referencesPrimaryKeys = entries.Select(SaveEntity).ToList();
                var referenceType = property.PropertyType.GetUnderlyingType();
                
                var removeCmd = _commandBuilder.BuildRemoveManyToManyReferences(entity, referenceType);
                removeCmd.ExecuteNonQuery();

                if (entries.Any())
                {
                    var addCmd = _commandBuilder.BuildSaveManyToManyReferences(entity, referenceType, referencesPrimaryKeys);
                    addCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Get all entities of a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAll<T>()
        {
            var cmd = _commandBuilder.BuildGetAll<T>();
            var reader = cmd.ExecuteReader();
            cmd.Dispose();
            var result = CreateObjectReader<T>(reader);
            reader.Close();
            return result;
        }

        /// <summary>
        /// Get an entity by its primary key
        /// </summary>
        /// <param name="pk"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetById<T>(object pk)
        {
            object? cachedEntry = _cache?.Get(typeof(T), pk);
            
            if (cachedEntry is not null)
            {
                return (T) cachedEntry;
            }
            var cmd = _commandBuilder.BuildGetById<T>(pk);
            var reader = cmd.ExecuteReader();
            var result = (T) CreateObjectReader<T>(reader);
            cmd.Dispose();
            reader.Close();
            reader.Dispose();
            return result;
        }

        /// <summary>
        /// Deletes an entity by its primary key
        /// </summary>
        /// <param name="pk"></param>
        public void DeleteById<T>(object pk)
        {
            var cmd = _commandBuilder.BuildDeleteById<T>(pk);
            cmd.ExecuteNonQuery();
        }
        
        /// <summary>
        /// Initialize the DBSets of this instance.
        /// </summary>
        private void InitializeDbContext()
        {
            var dbSets = GetDbSets();

            foreach (var dbSetProperty in dbSets)
            {
                object? dbSet = Activator.CreateInstance(dbSetProperty.PropertyType, _queryProvider);
                dbSetProperty.SetValue(this, dbSet);
            }
        }

        /// <summary>
        /// Create an object reader to create objects usign a data reader
        /// </summary>
        /// <param name="dataReader"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ObjectReader<T> CreateObjectReader<T>(IDataReader dataReader)
        {
            var loader = new LazyLoader(_commandBuilder);
            return new ObjectReader<T>(dataReader, loader);            
        }

        /// <summary>
        /// Get the tables for all entities in a given assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private IEnumerable<Table> GetTables(Assembly assembly)
        {
            var entityTypes = GetEntityTypes(assembly);
            var entityTables = entityTypes.Select(t => t.ToTable()).ToList();
            var tables = GetAllTables(entityTables);
            return tables;
        }
        
        /// <summary>
        /// Returns an union of the given tables and all connected tables
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        private static IEnumerable<Table> GetAllTables(List<EntityTable> tables)
        {
            var tableList = tables.ToList();
            var allTables = tableList.OfType<Table>().ToList();
            
            foreach (var table in tableList)
            {
                var externalTables = table.ExternalFields.Select(_ => _.Table);
                var manyToManyTables = table.ForeignKeyTables;
                allTables = allTables.Union(externalTables).ToList();
                allTables = allTables.Union(manyToManyTables).ToList();
            }
            
            return allTables
                .GroupBy(x => x.Name)
                .Select(grp => grp.First())
                .ToList();
        }
        
        /// <summary>
        /// Returns all entity types in a given assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private List<Type> GetEntityTypes(Assembly assembly)
        {
            var dbContexts = GetDbContextTypes(assembly);
            var entities = new List<Type>();
            
            foreach (var context in dbContexts)
            {
                var properties = context.GetProperties();
                var propertyTypes = properties.Select(p => p.PropertyType).Distinct();
                var genericProperties = propertyTypes.Where(t => t.IsGenericType);
                var dbSetProperties = genericProperties.Where(t => t.GetGenericTypeDefinition() == typeof(DbSet<>));
                var entityTypes = dbSetProperties.Select(t => t.GetGenericArguments().First());
                entities = entities.Union(entityTypes).ToList();
            }

            return entities;
        }

        /// <summary>
        /// Get the db sets of the current db context.
        /// </summary>
        /// <returns></returns>
        private List<PropertyInfo> GetDbSets()
        {
            var properties = GetType().GetProperties();

            var dbSets = properties.Where(x =>
                x.PropertyType.IsGenericType && 
                x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            return dbSets.ToList();
        }
        
        /// <summary>
        /// Gets all the types that inherit from DbContext in a given assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private IEnumerable<Type> GetDbContextTypes(Assembly assembly)
        {
            var dbContextType = typeof(DbContext);
            var types = assembly.GetTypes();
            var contexts = types.Where(t => t.IsSubclassOf(dbContextType));
            return contexts;
        }
    }
}