using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace Contoso.Timesheets
{
    public class Timesheets
    {
        readonly ITimesheetParser Parser;
        readonly ITimesheetService Service;

        public Timesheets(ITimesheetParser parser, ITimesheetService service) => (Parser, Service) = (parser, service);

        [FunctionName(nameof(Save))]
        public async Task<IActionResult> Save(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            HttpRequest req)
        {
            var timesheet = Parser.Parse(req.Body);

            await Service.Save(timesheet);

            return new OkObjectResult(timesheet);
        }
    }
}
