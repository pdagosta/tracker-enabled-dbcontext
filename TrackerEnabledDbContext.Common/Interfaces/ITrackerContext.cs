using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Interfaces
{
    public interface ITrackerContext: ITrackingContext, ITrackerOnlyContext
    {
    }
}
