using Cosmonaut;
using FluentAssertions;
using Machine.Fakes;
using Machine.Specifications;
using NSubstitute;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace Contoso.Timesheets.Test
{
    public class PaySlipServiceSpecification
    {
        [Subject(typeof(CosmosPaySlipService))]
        class Given_a_valid_employee : WithSubject<CosmosPaySlipService>
        {
            static readonly Timesheet Timesheet = new Timesheet
            {
                EmployeeId = "042",
                Year = 2019,
                Month = 5,
                Entries = new List<TimesheetEntry>
                {
                    new TimesheetEntry {ProjectId = "HOL", Hours = new[] {0d, 0d, 0d, 0d, 8d}},
                    new TimesheetEntry {ProjectId = "AAA", Hours = new[] {8d, 8d, 8d, 8d, 8d, 8d, 8d}}
                }
            };
            static PaySlip PaySlip;

            Because of = async () => PaySlip = await Subject.CreatePaySlip(Timesheet);

            It should_save_the_payslip = () => The<ICosmosStore<PaySlip>>().Received(1).UpsertAsync(PaySlip);
            It should_have_set_the_proper_worked_hours = () => PaySlip.Hours.Should().Be(40);
            It should_have_set_the_proper_overtime_hours = () => PaySlip.Overtime.Should().Be(16);
            It should_have_set_the_proper_employee = () => PaySlip.EmployeeId.Should().Be(Timesheet.EmployeeId);
            It should_have_set_the_proper_year = () => PaySlip.Year.Should().Be(Timesheet.Year);
            It should_have_set_the_proper_month = () => PaySlip.Month.Should().Be(Timesheet.Month);
        }

        [Subject(typeof(CosmosPaySlipService))]
        class When_approving_a_payslip: WithSubject<CosmosPaySlipService>
        {
            static readonly PaySlip PaySlip = new PaySlip
            {
                EmployeeId = "042",
                Year = 2019,
                Month = 5,
                Hours = 160,
                Overtime = 40
            };
            static readonly string ApprovalId = "ApprovalId";

            Because of = async () => await Subject.ApprovePaySlip(PaySlip, ApprovalId);

            It should_save_the_payslip = () => The<ICosmosStore<PaySlip>>().Received(1).UpsertAsync(PaySlip);
            It should_mark_the_payslip_as_approved = () => PaySlip.Approved.Should().BeTrue();
            It shoul_assign_the_corresponding_approval_id = () => PaySlip.ApprovalId.Should().Be(ApprovalId);
        }
    }
}
