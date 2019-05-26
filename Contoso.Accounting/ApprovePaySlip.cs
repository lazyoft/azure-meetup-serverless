using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace Contoso.Accounting
{
    public static class ApprovePaySlip
    {
        [FunctionName("ApprovePaySlip")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "approve/{id}")] HttpRequest req,
            string id,
            [OrchestrationClient] DurableOrchestrationClient client)
        {
            await client.RaiseEventAsync(id, "Contoso.PaySlipApproved");
            return new OkObjectResult(id);
        }
    }
}
