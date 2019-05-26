using System;
using Cosmonaut;
using FluentAssertions;
using Machine.Fakes;
using Machine.Specifications;
using NSubstitute;
// ReSharper disable InconsistentNaming

namespace Contoso.Timesheets.Test
{
    class TimesheetServiceSpecification
    {
        [Subject(typeof(CosmosTimesheetService))]
        class Given_a_valid_timesheet: WithSubject<CosmosTimesheetService>
        {
            static Timesheet Timesheet = new Timesheet();

            Because of = () => Subject.Save(Timesheet);
            It should_save_it = () => The<ICosmosStore<Timesheet>>().Received(1).UpsertAsync(Arg.Is(Timesheet));
        }

        [Subject(typeof(CosmosTimesheetService))]
        class Given_no_timesheet : WithSubject<CosmosTimesheetService>
        {
            static Exception Exception; 

            Because of = () => Exception = Catch.Exception(() => Subject.Save(null));
            It should_throw_an_argument_null_exception = () => Exception.Should().BeOfType<ArgumentNullException>();
        }

    }
}
