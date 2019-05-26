using System;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;

namespace Contoso.Timesheets
{
    public class CosmosPaySlipService : IPaySlipService
    {
        readonly ICosmosStore<PaySlip> Store;

        public CosmosPaySlipService(ICosmosStore<PaySlip> store) => Store = store;

        public async Task<PaySlip> CreatePaySlip(Timesheet timesheet)
        {
            var worked = timesheet.Entries
                .Where(e => e.ProjectId != "HOL")
                .Select(e => WeekDayHours(e, timesheet.Month, timesheet.Year))
                .Sum();

            var overtime = timesheet.Entries
                .Where(e => e.ProjectId != "HOL")
                .Select(e => WeekendHours(e, timesheet.Month, timesheet.Year))
                .Sum();

            var payslip = new PaySlip
            {
                EmployeeId = timesheet.EmployeeId,
                Year = timesheet.Year,
                Month = timesheet.Month,
                Name = timesheet.Name,
                Hours = worked,
                Overtime = overtime
            };
            await Store.UpsertAsync(payslip);
            return payslip;
        }

        static double WeekDayHours(TimesheetEntry entry, int month, int year)
        {
            return entry.Hours
                .Where((_, i) => (i + 1) <= DateTime.DaysInMonth(year, month))
                .Select((h, i) => (day: new DateTime(year, month, i + 1).DayOfWeek, hours: h))
                .Where(t => t.day != DayOfWeek.Saturday && t.day != DayOfWeek.Sunday)
                .Select(t => t.hours)
                .Sum();
        }

        static double WeekendHours(TimesheetEntry entry, int month, int year)
        {
            return entry.Hours
                .Where((_, i) => (i + 1) <= DateTime.DaysInMonth(year, month))
                .Select((h, i) => (day: new DateTime(year, month, i + 1).DayOfWeek, hours: h))
                .Where(t => t.day == DayOfWeek.Saturday || t.day == DayOfWeek.Sunday)
                .Select(t => t.hours)
                .Sum();
        }

        public async Task ApprovePaySlip(PaySlip paySlip, string approvalId)
        {
            paySlip.Approved = true;
            paySlip.ApprovalId = approvalId;

            await Store.UpsertAsync(paySlip);
        }
    }
}