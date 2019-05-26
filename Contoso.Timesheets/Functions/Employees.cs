using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.Timesheets
{
    public class Employees
    {
        readonly IEmployeeService Service;

        public Employees(IEmployeeService service) => Service = service;

        [FunctionName(nameof(UpdateHolidays))]
        public async Task UpdateHolidays([CosmosDBTrigger(
                                             databaseName: "contoso", 
                                             collectionName: "timesheets",
                                             ConnectionStringSetting = "ConnectionString",
                                             LeaseCollectionPrefix = "employees-",
                                             StartFromBeginning = true,
                                             CreateLeaseCollectionIfNotExists = true)]
                                         IReadOnlyList<Document> documents)
        {
            foreach (var timesheet in documents.Select(d => (Timesheet)(dynamic)d))
                await Service.UpdateHolidays(timesheet.EmployeeId, timesheet.Year, timesheet.Name);
        }
    }
}
