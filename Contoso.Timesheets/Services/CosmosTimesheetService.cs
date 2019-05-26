using Cosmonaut;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.Timesheets
{
    public class CosmosTimesheetService : ITimesheetService
    {
        readonly ICosmosStore<Timesheet> Store;

        public CosmosTimesheetService(ICosmosStore<Timesheet> store) => Store = store;

        public Task Save(Timesheet timesheet)
        {
            if(timesheet == null)
                throw new ArgumentNullException(nameof(timesheet));

            return Store.UpsertAsync(timesheet);
        }
    }
}