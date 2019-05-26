using Cosmonaut.Attributes;

namespace Contoso.Timesheets
{
    public class Employee
    {
        [CosmosPartitionKey] public string Id { get; set; }
        public string Name { get; set; }
        public Holidays Holidays { get; set; }
    }
}