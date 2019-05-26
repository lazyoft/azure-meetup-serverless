using System.Threading.Tasks;

namespace Contoso.Timesheets
{
    public interface IEmployeeService
    {
        Task UpdateHolidays(string employeeId, int year, string employeeName = default);
    }
}