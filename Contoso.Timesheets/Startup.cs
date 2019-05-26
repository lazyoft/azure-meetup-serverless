using Contoso.Timesheets;
using Cosmonaut;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Contoso.Timesheets
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddSingleton(p =>
                {
                    var config = p.GetService<IConfiguration>();
                    return new CosmosStoreSettings("contoso", config["Endpoint"], config["AuthKey"]);
                })
                .AddSingleton<IEventGridClient>(p =>
                {
                    var config = p.GetService<IConfiguration>();
                    return new EventGridClient(new TopicCredentials(config["EventGridAuthKey"]));
                })
                .AddSingleton<IEventPublisher>(p =>
                {
                    var config = p.GetService<IConfiguration>();
                    return new EventGridPublisher(p.GetService<IEventGridClient>(), config["EventGridHostName"]);
                })

                .AddSingleton<ICosmosStore<Timesheet>>(p => new CosmosStore<Timesheet>(p.GetService<CosmosStoreSettings>()))
                .AddSingleton<ICosmosStore<Employee>>(p => new CosmosStore<Employee>(p.GetService<CosmosStoreSettings>()))
                .AddSingleton<ICosmosStore<PaySlip>>(p => new CosmosStore<PaySlip>(p.GetService<CosmosStoreSettings>()))
                .AddSingleton<ITimesheetService, CosmosTimesheetService>()
                .AddSingleton<IEmployeeService, CosmosEmployeeService>()
                .AddSingleton<IPaySlipService, CosmosPaySlipService>()
                .AddSingleton<ITimesheetParser, ExcelTimesheetParser>();
        }
    }
}
