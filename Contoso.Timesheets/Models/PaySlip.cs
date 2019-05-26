using Cosmonaut.Attributes;

namespace Contoso.Timesheets
{
    public class PaySlip
    {
        [CosmosPartitionKey] public string Id => $"{EmployeeId}-{Year}-{Month}";
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public double Hours { get; set; }
        public double Overtime { get; set; }
        public bool Approved { get; set; }
        public string ApprovalId { get; set; }
    }
}