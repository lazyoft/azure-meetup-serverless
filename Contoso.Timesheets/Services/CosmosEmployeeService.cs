using Cosmonaut;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.Timesheets
{
    public class CosmosEmployeeService : IEmployeeService
    {
        readonly ICosmosStore<Employee> Store;
        readonly ICosmosStore<Timesheet> Timesheets;

        public CosmosEmployeeService(ICosmosStore<Employee> store, ICosmosStore<Timesheet> timesheets) => 
            (Store, Timesheets) = (store, timesheets);

        public async Task UpdateHolidays(string employeeId, int year, string employeeName = default)
        {
            var employee = await Store.FindAsync(employeeId) ?? new Employee
            {
                Id = employeeId,
                Name = employeeName,
                Holidays = new Holidays {Available = 240}
            };

            employee.Holidays.Spent = await HolidayHoursFor(employeeId, year);
            await Store.UpsertAsync(employee);
        }

        protected virtual async Task<double> HolidayHoursFor(string employeeId, int year)
        {
            return await Timesheets.Query()
                .Where(t => t.EmployeeId == employeeId && t.Year == year)
                .SelectMany(t => t.Entries)
                .Where(e => e.ProjectId == "HOL")
                .Select(e => e.Hours.Sum())
                .SumAsync();
        }
    }
}