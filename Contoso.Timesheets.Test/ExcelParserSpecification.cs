using System;
using FluentAssertions;
using Machine.Fakes;
using Machine.Specifications;
using System.IO;
// ReSharper disable InconsistentNaming

namespace Contoso.Timesheets.Test
{
    public class ExcelParserSpecification
    {
        [Subject(typeof(ExcelTimesheetParser))]
        public class Given_a_valid_excel_stream: WithSubject<ExcelTimesheetParser>
        {
            static Timesheet Parsed;
            static Stream Stream;

            Establish context = () => Stream = new FileStream("./valid.xlsx", FileMode.Open);
            Because of = () => Parsed = Subject.Parse(Stream);

            It should_parse_the_year = () => Parsed.Year.Should().Be(2019);
            It should_parse_the_month = () => Parsed.Month.Should().Be(4);
            It should_parse_the_employee_id = () => Parsed.EmployeeId.Should().Be("042");
            It should_parse_the_employee_name = () => Parsed.Name.Should().Be("Fabrizio Chignoli");
            It should_parse_the_timesheet_entries = () => Parsed.Entries.Should().HaveCount(3)
                .And.Contain(t => t.ProjectId != null && t.Hours.Length == 31);
        }

        [Subject(typeof(ExcelTimesheetParser))]
        public class Given_a_non_excel_stream : WithSubject<ExcelTimesheetParser>
        {
            static Stream Stream;
            static Exception Exception;

            Establish context = () => Stream = new FileStream("./nonexcel.txt", FileMode.Open);
            Because of = () => Exception = Catch.Exception(() => Subject.Parse(Stream));

            It should_throw_an_invalid_data_exception = () => Exception.Should().BeOfType<InvalidDataException>();
        }

        [Subject(typeof(ExcelTimesheetParser))]
        public class Given_an_incomplete_excel_stream: WithSubject<ExcelTimesheetParser>
        {
            static Stream Stream;
            static Exception Exception;

            Establish context = () => Stream = new FileStream("./incomplete.xlsx", FileMode.Open);
            Because of = () => Exception = Catch.Exception(() => Subject.Parse(Stream));

            It should_throw_an_argument_exception = () => Exception.Should().BeOfType<ArgumentException>();
        }

        [Subject(typeof(ExcelTimesheetParser))]
        public class Given_an_invalid_excel_stream : WithSubject<ExcelTimesheetParser>
        {
            static Stream Stream;
            static Exception Exception;

            Establish context = () => Stream = new FileStream("./invalid.xlsx", FileMode.Open);
            Because of = () => Exception = Catch.Exception(() => Subject.Parse(Stream));

            It should_throw_an_argument_exception = () => Exception.Should().BeOfType<ArgumentException>();
        }
    }
}
