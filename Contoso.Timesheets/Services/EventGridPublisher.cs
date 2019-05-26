using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;

namespace Contoso.Timesheets
{
    public class EventGridPublisher : IEventPublisher
    {
        readonly IEventGridClient Client;
        readonly string HostName;

        public EventGridPublisher(IEventGridClient client, string hostName) => (Client, HostName) = (client, hostName);

        public async Task Publish(EventGridEvent gridEvent)
        {
            await Client.PublishEventsAsync(HostName, new[] { gridEvent });
        }
    }
}