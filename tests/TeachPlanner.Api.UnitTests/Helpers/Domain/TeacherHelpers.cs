using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Shared.Contracts.PlannerTemplates;
using TeachPlanner.Shared.Contracts.Services;
using TeachPlanner.Shared.Contracts.Teachers.AccountSetup;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Api.Tests.Helpers.Domain;

internal static class TeacherHelpers
{
    internal static Teacher CreateTeacher()
    {
        var teacher = Teacher.Create(Guid.NewGuid().ToString(), "First", "Last");
        teacher.AddYearData(YearDataEntry.Create(2024, new YearDataId(Guid.NewGuid())));

        return teacher;
    }

    internal static List<string> CreateSubjectNames()
    {
        return ["Mathematics", "English", "Science"];
    }

    internal static List<CurriculumSubject> CreateCurriculumSubjects()
    {
        return CreateSubjectNames().Select(subjectNames => CurriculumSubject.Create(subjectNames, [], "")).ToList();
    }

    internal static DayPlanPatternDto CreateDayPlanPatternDto()
    {
        return new DayPlanPatternDto(
            [
                new(
                    new TimeOnly(9, 10, 0),
                    new TimeOnly(10, 0, 0)),

                new(
                    new TimeOnly(10, 0, 0),
                    new TimeOnly(10, 50, 0)),

                new(
                    new TimeOnly(11, 20, 0),
                    new TimeOnly(12, 10, 0)),

                new(
                    new TimeOnly(12, 10, 0),
                    new TimeOnly(1, 0, 0)),

                new(
                    new TimeOnly(1, 30, 0),
                    new TimeOnly(2, 20, 0)),

                new(
                    new TimeOnly(2, 20, 0),
                    new TimeOnly(3, 10, 0))
            ],
            [
                new(
                    "Recess",
                    new TimeOnly(10, 50, 0),
                    new TimeOnly(11, 20, 0)),

                new(
                    "Lunch",
                    new TimeOnly(1, 0, 0),
                    new TimeOnly(1, 30, 0))
            ]);
    }

    internal static DayPlanPatternDto CreateDayPlanPatternDtoWithOverlappingTimes()
    {
        return new DayPlanPatternDto(
            [
                new(
                    new TimeOnly(9, 10, 0),
                    new TimeOnly(10, 0, 0)),

                new(
                    new TimeOnly(10, 0, 0),
                    new TimeOnly(10, 50, 0)),

                new(
                    new TimeOnly(11, 20, 0),
                    new TimeOnly(12, 10, 0)),

                new(
                    new TimeOnly(12, 10, 0),
                    new TimeOnly(1, 0, 0)),

                new(
                    new TimeOnly(1, 30, 0),
                    new TimeOnly(2, 20, 0)),

                new(
                    new TimeOnly(2, 20, 0),
                    new TimeOnly(3, 10, 0))
            ],
            [
                new(
                    "Recess",
                    new TimeOnly(10, 50, 0),
                    new TimeOnly(11, 20, 0)),

                new(
                    "Lunch",
                    new TimeOnly(1, 0, 0),
                    new TimeOnly(1, 30, 0))
            ]);
    }

    internal static List<TermDate> CreateTermDates()
    {
        return
        [
            new(1, new DateOnly(2023, 1, 30), new DateOnly(2023, 4, 1)),
            new(2, new DateOnly(2023, 4, 15), new DateOnly(2023, 6, 30)),
            new(3, new DateOnly(2023, 7, 14), new DateOnly(2023, 9, 25)),
            new(4, new DateOnly(2023, 10, 10), new DateOnly(2023, 12, 15))
        ];
    }

    internal static List<TermDateDto> CreateTermDateDtos()
    {
        return
        [
            new("2023-01-30", "2023-04-01"),
            new("2023-04-15", "2023-06-30"),
            new("2023-07-14", "2023-09-25"),
            new("2023-10-10", "2023-12-15")
        ];
    }

    internal static List<TermDateDto> CreateTermDateDtosWithOverlappingDates()
    {
        return
        [
            new("2023-01-30", "2023-04-01"),
            new("2023-04-15", "2023-06-30"),
            new("2023-07-14", "2023-09-25"),
            new("2023-10-10", "2023-12-15")
        ];
    }

    internal static WeekStructure CreateDayPlanTemplate(DayPlanPatternDto dayPlanPattern)
    {
        var periodTemplates = new List<TemplatePeriod>
        {
            new(PeriodType.Lesson, "Lesson 1", new TimeOnly(9, 10), new TimeOnly(10, 0)),
            new(PeriodType.Lesson, "Lesson 2", new TimeOnly(10, 0), new TimeOnly(10, 50)),
            new(PeriodType.Break, dayPlanPattern.BreakTemplates[0].Name, new TimeOnly(10, 50),
                new TimeOnly(11, 20)),
            new(PeriodType.Lesson, "Lesson 3", new TimeOnly(11, 20), new TimeOnly(12, 10)),
            new(PeriodType.Lesson, "Lesson 4", new TimeOnly(12, 10), new TimeOnly(13, 0)),
            new(PeriodType.Break, dayPlanPattern.BreakTemplates[1].Name, new TimeOnly(13, 0),
                new TimeOnly(13, 30)),
            new(PeriodType.Lesson, "Lesson 5", new TimeOnly(13, 30), new TimeOnly(14, 20)),
            new(PeriodType.Lesson, "Lesson 6", new TimeOnly(14, 20), new TimeOnly(15, 10))
        };

        return WeekStructure.Create(periodTemplates, new TeacherId(Guid.NewGuid()));
    }

    internal static AccountSetupRequest CreateAccountSetupRequest()
    {
        return new AccountSetupRequest(CreateSubjectNames(), CreateYearLevelsTaughtAsStringList(),
            CreateWeekStructureDto());
    }

    internal static AccountSetupRequest CreateAccountSetupRequestWithOverlappingTimes()
    {
        var request = new AccountSetupRequest(["English"], CreateYearLevelsTaughtAsStringList(),
            CreateWeekStructureDto());

        request.WeekStructure.Periods[0].StartTime.AddHours(1);
        
        return request;
    }

    internal static List<CurriculumSubject> CreateCurriculumSubjects(List<string> subjectNames)
    {
        return subjectNames.Select(subjectName => CurriculumSubject.Create(subjectName, [], "")).ToList();
    }

    private static List<string> CreateYearLevelsTaughtAsStringList()
    {
        return ["Year1", "Year2"];
    }

    internal static List<YearLevelValue> CreateYearLevelsTaught()
    {
        return [YearLevelValue.Year1, YearLevelValue.Year2];
    }

    private static WeekStructureDto CreateWeekStructureDto()
    {
        var weekStructure = CreateDayPlanTemplate(CreateDayPlanPatternDto());
        var dayTemplates = new DayTemplateDto[]
        {
            new([
                    new LessonTemplateDto("English", 2, 1),
                    new LessonTemplateDto("Mathematics", 2, 4),
                    new LessonTemplateDto("English", 2, 7)
                ],
                false),
            new([
                    new LessonTemplateDto("English", 2, 1),
                    new LessonTemplateDto("Mathematics", 2, 4),
                    new LessonTemplateDto("English", 2, 7)
                ],
                false)
        };

        return new WeekStructureDto(weekStructure.ToDto().Periods, dayTemplates);
    }
}