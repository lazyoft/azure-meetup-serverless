using System;
using OfficeOpenXml;
using System.IO;
using System.Linq;

namespace Contoso.Timesheets
{
    public class ExcelTimesheetParser : ITimesheetParser
    {
        public Timesheet Parse(Stream stream)
        {
            using (var package = new ExcelPackage(stream))
            {
                var tableRange = package.Workbook.Worksheets[0].Tables["Timesheet"]?.Address.Address;
                if(tableRange == null)
                    throw new ArgumentException("The timesheet table in the file does not exist");

                var table = package.Workbook.Worksheets[0].Cells[tableRange]
                    .GroupBy(cell => cell.Start.Row)
                    .Skip(1)
                    .SkipLast(1);

                var entries = (from row in table
                    let cells = row.Select(cell => cell.Value).ToArray()
                    where cells[0] != null
                    select new TimesheetEntry
                    {
                        ProjectId = cells[0] as string,
                        Hours = cells.Skip(2).Select(cell => cell ?? 0.0).Cast<double>().ToArray()
                    }).ToList();

                var result = new Timesheet
                {
                    Year = package.Workbook.Names?["Year"].GetValue<int>() ?? 0,
                    Month = package.Workbook.Names?["Month"].GetValue<int>() ?? 0,
                    EmployeeId = package.Workbook.Names?["EmployeeId"].GetValue<string>() ?? string.Empty,
                    Name = package.Workbook.Names?["Employee"].GetValue<string>() ?? string.Empty,
                    Entries = entries
                };

                if(result.Year == 0 || result.Month == 0 || string.IsNullOrEmpty(result.EmployeeId))
                    throw new ArgumentException("The arguments in the file were not properly filled in");

                return result;
            }
        }
    }
}
