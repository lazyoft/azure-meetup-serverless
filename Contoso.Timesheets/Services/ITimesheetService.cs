using System.Threading.Tasks;

namespace Contoso.Timesheets
{
    public interface ITimesheetService
    {
        Task Save(Timesheet timesheet);
    }
}