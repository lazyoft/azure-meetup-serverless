using System.IO;

namespace Contoso.Timesheets
{
    public interface ITimesheetParser
    {
        Timesheet Parse(Stream stream);
    }
}