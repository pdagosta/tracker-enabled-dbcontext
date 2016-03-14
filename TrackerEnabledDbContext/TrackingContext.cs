using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackerEnabledDbContext.Common;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.EventArgs;
using TrackerEnabledDbContext.Common.Interfaces;

namespace TrackerEnabledDbContext
{
    public class TrackingContext : DbContext, ITrackingContext
    {
        Func<ITrackingContext, CoreTrackerContext> _getCoreTrackerContext;

        public TrackingContext(string nameOrConnectionString)
            : this(nameOrConnectionString, (context) => new CoreTrackerContext(context))
        {
        }

        public TrackingContext(string nameOrConnectionString, Func<ITrackingContext, CoreTrackerContext> getCoreTrackerContext)
            : base(nameOrConnectionString)
        {
            _getCoreTrackerContext = getCoreTrackerContext;
        }

        public event EventHandler<AuditLogGeneratedEventArgs> OnAuditLogGenerated;

        /// <summary>
        ///     This method saves the model changes to the database.
        ///     If the tracker for an entity is active, it will also put the old values in tracking table.
        ///     Always use this method instead of SaveChanges() whenever possible.
        /// </summary>
        /// <param name="userName">Username of the logged in identity</param>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public virtual int SaveChanges(object userName)
        {
            if (!GlobalTrackingConfig.Enabled) return base.SaveChanges();

            using (var coreTrackerContext = _getCoreTrackerContext(this))
            {
                coreTrackerContext.CoreTracker.AuditChanges(userName);

                IEnumerable<DbEntityEntry> addedEntries = coreTrackerContext.CoreTracker.GetAdditions();
                // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
                int result = base.SaveChanges();
                //By now., we have got the primary keys of added entries of added entiries because of the call to savechanges.

                coreTrackerContext.CoreTracker.AuditAdditions(userName, addedEntries);

                //save changes to audit of added entries
                base.SaveChanges();
                return result;
            }
        }

        /// <summary>
        ///     This method saves the model changes to the database.
        ///     If the tracker for an entity is active, it will also put the old values in tracking table.
        /// </summary>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public override int SaveChanges()
        {
            if (!GlobalTrackingConfig.Enabled) return base.SaveChanges();
            //var user = Thread.CurrentPrincipal?.Identity?.Name ?? "Anonymous"; 
            return SaveChanges(null);
        }



        #region -- Async --

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        ///     If the tracker for an entity is active, it will also put the old values in tracking table.
        /// </summary>
        /// <param name="userName">Username of the logged in identity</param>
        /// <param name="cancellationToken">
        ///     A System.Threading.CancellationToken to observe while waiting for the task
        ///     to complete.
        /// </param>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public virtual async Task<int> SaveChangesAsync(object userName, CancellationToken cancellationToken)
        {
            if (!GlobalTrackingConfig.Enabled) return await base.SaveChangesAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            using (var coreTrackerContext = _getCoreTrackerContext(this))
            {
                coreTrackerContext.CoreTracker.OnAuditLogGenerated += this.OnAuditLogGenerated;
                coreTrackerContext.CoreTracker.AuditChanges(userName);

                IEnumerable<DbEntityEntry> addedEntries = coreTrackerContext.CoreTracker.GetAdditions();

                // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
                int result = await base.SaveChangesAsync(cancellationToken);

                //By now., we have got the primary keys of added entries of added entiries because of the call to savechanges.
                coreTrackerContext.CoreTracker.AuditAdditions(userName, addedEntries);

                //save changes to audit of added entries
                await base.SaveChangesAsync(cancellationToken);

                return result;
            }
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        ///     If the tracker for an entity is active, it will also put the old values in tracking table.
        ///     Always use this method instead of SaveChangesAsync() whenever possible.
        /// </summary>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public virtual async Task<int> SaveChangesAsync(int userId)
        {
            if (!GlobalTrackingConfig.Enabled) return await base.SaveChangesAsync(CancellationToken.None);

            return await SaveChangesAsync(userId, CancellationToken.None);
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        ///     If the tracker for an entity is active, it will also put the old values in tracking table.
        ///     Always use this method instead of SaveChangesAsync() whenever possible.
        /// </summary>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public virtual async Task<int> SaveChangesAsync(string userName)
        {
            if (!GlobalTrackingConfig.Enabled) return await base.SaveChangesAsync(CancellationToken.None);

            return await SaveChangesAsync(userName, CancellationToken.None);
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        ///     If the tracker for an entity is active, it will also put the old values in tracking table with null UserName.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous save operation.  The task result
        ///     contains the number of objects written to the underlying database.
        /// </returns>
        public override async Task<int> SaveChangesAsync()
        {
            if (!GlobalTrackingConfig.Enabled) return await base.SaveChangesAsync(CancellationToken.None);

            return await SaveChangesAsync(null, CancellationToken.None);
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        ///     If the tracker for an entity is active, it will also put the old values in tracking table with null UserName.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A System.Threading.CancellationToken to observe while waiting for the task
        ///     to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous save operation.  The task result
        ///     contains the number of objects written to the underlying database.
        /// </returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (!GlobalTrackingConfig.Enabled) return await base.SaveChangesAsync(cancellationToken);

            return await SaveChangesAsync(null, cancellationToken);
        }
        #endregion
    }
}