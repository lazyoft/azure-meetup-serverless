using Cosmonaut;
using FluentAssertions;
using Machine.Fakes;
using Machine.Specifications;
using NSubstitute;
using System;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace Contoso.Timesheets.Test
{
    public class EmployeeServiceSpecification
    {
        public class TestableEmployeeService : CosmosEmployeeService
        {
            public TestableEmployeeService(ICosmosStore<Employee> store, ICosmosStore<Timesheet> timesheets) : base(store, timesheets) { }
            protected override Task<double> HolidayHoursFor(string employeeId, int year) => Task.FromResult(42d);
        }

        [Subject(typeof(CosmosEmployeeService))]
        class Given_a_valid_employee_id: WithSubject<TestableEmployeeService>
        {
            static readonly string EmployeeId = "042";
            static readonly int Year = 2019;
            static readonly Employee Employee = new Employee { Holidays = new Holidays() };

            Establish context = () => The<ICosmosStore<Employee>>().FindAsync(EmployeeId).Returns(Employee);
            Because of = async () => await Subject.UpdateHolidays(EmployeeId, Year);

            It should_find_the_employee_with_the_given_id = () => The<ICosmosStore<Employee>>().Received(1).FindAsync(EmployeeId);
            It should_update_the_holiday_hours = () => Employee.Holidays.Spent.Should().Be(42d);
            It should_update_the_employee = () => The<ICosmosStore<Employee>>().Received(1).UpsertAsync(Employee);
        }

        [Subject(typeof(CosmosEmployeeService))]
        class Given_a_non_existing_employee_id : WithSubject<TestableEmployeeService>
        {
            static readonly string InvalidEmployeeId = "042";
            static readonly string InvalidEmployeeName = "New Employee";
            static readonly int Year = 2019;

            Establish context = () => The<ICosmosStore<Employee>>().FindAsync(InvalidEmployeeId).Returns(Task.FromResult(default(Employee)));
            Because of = () => Subject.UpdateHolidays(InvalidEmployeeId, Year, InvalidEmployeeName).GetAwaiter().GetResult();

            It should_insert_the_new_employee_on_the_system = () => The<ICosmosStore<Employee>>().Received(1)
                .UpsertAsync(Arg.Is<Employee>(e => e.Id == InvalidEmployeeId && e.Name == InvalidEmployeeName));
        }
    }
}
