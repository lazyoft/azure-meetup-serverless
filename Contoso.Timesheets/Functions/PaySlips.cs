using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

namespace Contoso.Timesheets
{
    public class PaySlips
    {
        readonly IPaySlipService Service;
        readonly IEventPublisher Publisher;

        public PaySlips(IPaySlipService service, IEventPublisher publisher) => (Service, Publisher) = (service, publisher);

        [FunctionName(nameof(CreatePaySlip))]
        public async Task CreatePaySlip([CosmosDBTrigger(
                                             databaseName: "contoso", 
                                             collectionName: "timesheets",
                                             ConnectionStringSetting = "ConnectionString",
                                             LeaseCollectionPrefix = "payslips-",
                                             StartFromBeginning = true,
                                             CreateLeaseCollectionIfNotExists = true)]
                                         IReadOnlyList<Document> documents)
        {
            foreach(var timesheet in documents.Select(d => (Timesheet) (dynamic) d))
            {
                var payslip = await Service.CreatePaySlip(timesheet);
                await Publisher.Publish(new PaySlipReady(payslip));
            }
        }

        [FunctionName(nameof(EmitPaySlip))]
        public async Task EmitPaySlip([EventGridTrigger] EventGridEvent gridEvent,
                                      [OrchestrationClient] DurableOrchestrationClient starter,
                                      ILogger log)
        {
            var instanceId = await starter.StartNewAsync(nameof(RunOrchestrator), gridEvent.Data);
            var payload = starter.CreateHttpManagementPayload(instanceId);
            log.LogWarning($"PaySlip id: {payload.Id}, status: {payload.StatusQueryGetUri}");
        }

        [FunctionName(nameof(RunOrchestrator))]
        public async Task<string> RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var paySlip = context.GetInput<PaySlip>();
            await context.WaitForExternalEvent("Contoso.PaySlipApproved", TimeSpan.FromMinutes(10));
            await Service.ApprovePaySlip(paySlip, context.InstanceId);
            return context.InstanceId;
        }
    }
}
