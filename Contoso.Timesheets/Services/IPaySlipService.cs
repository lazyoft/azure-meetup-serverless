using System.Threading.Tasks;

namespace Contoso.Timesheets
{
    public interface IPaySlipService
    {
        Task<PaySlip> CreatePaySlip(Timesheet timesheet);
        Task ApprovePaySlip(PaySlip paySlip, string approvalId);
    }
}