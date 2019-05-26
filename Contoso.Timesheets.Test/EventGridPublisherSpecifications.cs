using Machine.Fakes;
using Machine.Specifications;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace Contoso.Timesheets.Test
{
    public class EventGridPublisherSpecifications
    {
        [Subject(typeof(EventGridPublisher))]
        public class Given_an_event_grid_publisher : WithSubject<EventGridPublisher>
        {
            static readonly EventGridEvent Event = new EventGridEvent();

            Because of = async () => await Subject.Publish(Event);

            It should_send_the_event_to_the_event_grid = async () => await The<IEventGridClient>().Received(1)
                .PublishEventsAsync(Arg.Any<string>(), Arg.Is<IList<EventGridEvent>>(l => l.First() == Event));
        }
    }
}
