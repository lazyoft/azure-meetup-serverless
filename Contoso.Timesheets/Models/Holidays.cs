namespace Contoso.Timesheets
{
    public class Holidays
    {
        public double Available { get; set; }
        public double Spent { get; set; }
        public double Remaining => Available - Spent;
    }
}