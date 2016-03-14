using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackerEnabledDbContext.Common.EventArgs;

namespace TrackerEnabledDbContext.Common.Interfaces
{
    public interface ITrackingContext: IDbContext
    {
        event EventHandler<AuditLogGeneratedEventArgs> OnAuditLogGenerated;
        int SaveChanges(object userName);
        //async
        Task<int> SaveChangesAsync(object userName, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(int userId);
        Task<int> SaveChangesAsync(string userName);
    }
}
