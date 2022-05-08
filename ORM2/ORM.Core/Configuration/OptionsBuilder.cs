﻿using System.Data;
using System.Linq;
using ORM.Core.Caching;
using ORM.Core.Interfaces;

namespace ORM.Core.Configuration
{
    /// <summary>
    /// Applies a configuration on a database context context
    /// </summary>
    public class OptionsBuilder
    {
        /// <summary>
        /// Used for caching entity
        /// </summary>
        public ICache? Cache;

        /// <summary>
        /// Used for building commands to be executed against the current database connection
        /// </summary>
        public ICommandBuilder? CommandBuilder;

        /// <summary>
        /// Query provider for executing LINQ expressions against the current database connection 
        /// </summary>
        public IQueryProvider? QueryProvider;
        
        public OptionsBuilder()
        {
        }

        /// <summary>
        /// Use an entity cache
        /// </summary>
        public void UseEntityCache()
        {
            Cache = new EntityCache();
        }

        /// <summary>
        /// Use a state tracking cache
        /// </summary>
        public void UseStateTrackingCache()
        {
            Cache = new StateTrackingCache();
        }

        /// <summary>
        /// Use no cache
        /// </summary>
        public void UseNoCache()
        {
            Cache = null;
        }

        /// <summary>
        /// Use a specific cache.
        /// </summary>
        /// <param name="cache"></param>
        public void UseCache(ICache cache)
        {
            Cache = cache;
        }

        /// <summary>
        /// Use a specific command builder.
        /// This is used for implementing your own translator for database context operations.
        /// </summary>
        /// <param name="commandBuilder"></param>
        public void UseCommandBuilder(ICommandBuilder commandBuilder)
        {
            CommandBuilder = commandBuilder;
        }

        /// <summary>
        /// Use a specific query provider.
        /// This is used for implementing your own expression tree translator. 
        /// </summary>
        /// <param name="queryProvider"></param>
        public void UseQueryProvider(IQueryProvider queryProvider)
        {
            QueryProvider = queryProvider;
        }
    }
}