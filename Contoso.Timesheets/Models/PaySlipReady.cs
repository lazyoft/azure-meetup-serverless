using System;
using Microsoft.Azure.EventGrid.Models;

namespace Contoso.Timesheets
{
    class PaySlipReady : EventGridEvent
    {
        public PaySlipReady(PaySlip payslip)
        {
            Id = Guid.NewGuid().ToString();
            EventType = "Contoso.PaySlipReady";
            Data = payslip;
            EventTime = DateTime.Now;
            Subject = payslip.Id;
            DataVersion = "1.0";
        }
    }
}