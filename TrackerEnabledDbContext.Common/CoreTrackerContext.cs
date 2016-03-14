using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackerEnabledDbContext.Common.EventArgs;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common
{
    public class CoreTrackerContext : DbContext, ITrackerOnlyContext
    {
        private readonly CoreTracker _coreTracker;
        public CoreTracker CoreTracker { get { return _coreTracker;} }

        public CoreTrackerContext(ITrackingContext trackingContext)
        {
            _coreTracker = new CoreTracker(trackingContext, this);
        }

        public CoreTrackerContext(ITrackingContext trackingContext, DbCompiledModel model)
            : base(model)
        {
            _coreTracker = new CoreTracker(trackingContext, this);
        }

        public CoreTrackerContext(ITrackingContext trackingContext, string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            _coreTracker = new CoreTracker(trackingContext, this);
        }

        public CoreTrackerContext(ITrackingContext trackingContext, string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            _coreTracker = new CoreTracker(trackingContext, this);
        }

        public CoreTrackerContext(ITrackingContext trackingContext, DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            _coreTracker = new CoreTracker(trackingContext, this);
        }

        public CoreTrackerContext(ITrackingContext trackingContext, DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            _coreTracker = new CoreTracker(trackingContext, this);
        }

        public CoreTrackerContext(ITrackingContext trackingContext, ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            _coreTracker = new CoreTracker(trackingContext, this);
        }
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<AuditLogDetail> LogDetails { get; set; }

        /// <summary>
        ///     Get all logs for the given model type
        /// </summary>
        /// <typeparam name="TEntity">Type of domain model</typeparam>
        /// <returns></returns>
        public IQueryable<AuditLog> GetLogs<TEntity>()
        {
            return _coreTracker.GetLogs<TEntity>();
        }

        /// <summary>
        ///     Get all logs for the given entity name
        /// </summary>
        /// <param name="entityName">full name of entity</param>
        /// <returns></returns>
        public IQueryable<AuditLog> GetLogs(string entityName)
        {
            return _coreTracker.GetLogs(entityName);
        }

        /// <summary>
        ///     Get all logs for the given model type for a specific record
        /// </summary>
        /// <typeparam name="TEntity">Type of domain model</typeparam>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public IQueryable<AuditLog> GetLogs<TEntity>(object primaryKey)
        {
            return _coreTracker.GetLogs<TEntity>(primaryKey);
        }

        /// <summary>
        ///     Get all logs for the given entity name for a specific record
        /// </summary>
        /// <param name="entityName">full name of entity</param>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public IQueryable<AuditLog> GetLogs(string entityName, object primaryKey)
        {
            return _coreTracker.GetLogs(entityName, primaryKey);
        }
    }
}
