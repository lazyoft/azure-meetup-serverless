using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;

namespace Contoso.Timesheets
{
    public interface IEventPublisher
    {
        Task Publish(EventGridEvent gridEvent);
    }
}