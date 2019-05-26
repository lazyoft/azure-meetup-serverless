using System.Collections.Generic;
using Cosmonaut.Attributes;

namespace Contoso.Timesheets
{
    public class Timesheet
    {
        [CosmosPartitionKey] public string Id => $"{EmployeeId}-{Year}-{Month}";
        public int Year { get; set; }
        public int Month { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public List<TimesheetEntry> Entries { get; set; }
    }
}