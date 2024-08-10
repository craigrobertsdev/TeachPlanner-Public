using TeachPlanner.Shared.Contracts.Services;
using TeachPlanner.Shared.Contracts.Teachers.AccountSetup;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.PlannerTemplates;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.YearDataRecords;

namespace TeachPlanner.Api.UnitTests.Helpers;
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
        return new List<string>
        {
            "Mathematics",
            "English",
            "Science",
        };
    }

    internal static List<CurriculumSubject> CreateCurriculumSubjects()
    {
        return CreateSubjectNames().Select(subjectNames => CurriculumSubject.Create(subjectNames, new(), "Description")).ToList();
    }

    internal static DayPlanPatternDto CreateDayPlanPatternDto()
    {
        return new DayPlanPatternDto(
             new List<DayPlanLessonTemplateDto>
             {
                 new DayPlanLessonTemplateDto(
                     new TimeOnly(9, 10, 0),
                     new TimeOnly(10, 0, 0)),
                 new DayPlanLessonTemplateDto(
                     new TimeOnly(10, 0, 0),
                     new TimeOnly(10, 50, 0)),
                 new DayPlanLessonTemplateDto(
                     new TimeOnly(11, 20, 0),
                     new TimeOnly(12, 10, 0)),
                 new DayPlanLessonTemplateDto(
                     new TimeOnly(12, 10, 0),
                     new TimeOnly(1, 0, 0)),
                 new DayPlanLessonTemplateDto(
                     new TimeOnly(1, 30, 0),
                     new TimeOnly(2, 20, 0)),
                 new DayPlanLessonTemplateDto(
                     new TimeOnly(2, 20, 0),
                     new TimeOnly(3, 10, 0))
             },
             new List<DayPlanBreakTemplateDto>
             {
                 new DayPlanBreakTemplateDto(
                     "Recess",
                     new TimeOnly(10, 50, 0),
                     new TimeOnly(11, 20, 0)),
                 new DayPlanBreakTemplateDto(
                     "Lunch",
                     new TimeOnly(1, 0, 0),
                     new TimeOnly(1, 30, 0))
             });
    }

    internal static DayPlanPatternDto CreateDayPlanPatternDtoWithOverlappingTimes()
    {
        return new DayPlanPatternDto(
         new List<DayPlanLessonTemplateDto>
         {
             new DayPlanLessonTemplateDto(
                 new TimeOnly(9, 10, 0),
                 new TimeOnly(10, 0, 0)),
             new DayPlanLessonTemplateDto(
                 new TimeOnly(10, 0, 0),
                 new TimeOnly(10, 50, 0)),
             new DayPlanLessonTemplateDto(
                 new TimeOnly(11, 20, 0),
                 new TimeOnly(12, 10, 0)),
             new DayPlanLessonTemplateDto(
                 new TimeOnly(12, 10, 0),
                 new TimeOnly(1, 0, 0)),
             new DayPlanLessonTemplateDto(
                 new TimeOnly(1, 30, 0),
                 new TimeOnly(2, 20, 0)),
             new DayPlanLessonTemplateDto(
                 new TimeOnly(2, 20, 0),
                 new TimeOnly(3, 10, 0))
         },
         new List<DayPlanBreakTemplateDto>
         {
             new DayPlanBreakTemplateDto(
                 "Recess",
                 new TimeOnly(10, 50, 0),
                 new TimeOnly(11, 20, 0)),
             new DayPlanBreakTemplateDto(
                 "Lunch",
                 new TimeOnly(1, 0, 0),
                 new TimeOnly(1, 30, 0))
         });
    }

    internal static List<TermDate> CreateTermDates()
    {
        return new List<TermDate>()
        {
            new TermDate(1, new DateOnly(2023, 1, 30), new DateOnly(2023, 4, 1)),
            new TermDate(2, new DateOnly(2023, 4, 15), new DateOnly(2023, 6, 30)),
            new TermDate(3, new DateOnly(2023, 7, 14), new DateOnly(2023, 9, 25)),
            new TermDate(4, new DateOnly(2023, 10, 10), new DateOnly(2023, 12, 15)),
        };
    }
    internal static List<TermDateDto> CreateTermDateDtos()
    {
        return new List<TermDateDto>()
        {
            new TermDateDto("2023-01-30", "2023-04-01"),
            new TermDateDto("2023-04-15", "2023-06-30"),
            new TermDateDto("2023-07-14", "2023-09-25"),
            new TermDateDto("2023-10-10", "2023-12-15")
        };
    }

    internal static List<TermDateDto> CreateTermDateDtosWithOverlappingDates()
    {
        return new List<TermDateDto>()
        {
            new TermDateDto("2023-01-30", "2023-04-01"),
            new TermDateDto("2023-04-15", "2023-06-30"),
            new TermDateDto("2023-07-14", "2023-09-25"),
            new TermDateDto("2023-10-10", "2023-12-15")
        };

    }
    internal static WeekStructure CreateDayPlanTemplate(DayPlanPatternDto dayPlanPattern)
    {
        var periodTemplates = new List<TemplatePeriod>
        {
            new TemplatePeriod(PeriodType.Lesson, "Lesson 1", new TimeOnly(9, 10), new TimeOnly(10, 0)),
            new TemplatePeriod(PeriodType.Lesson, "Lesson 2", new TimeOnly(10, 0), new TimeOnly(10, 50)),
            new TemplatePeriod(PeriodType.Break, dayPlanPattern.BreakTemplates[0].Name, new TimeOnly(10, 50), new TimeOnly(11, 20)),
            new TemplatePeriod(PeriodType.Lesson, "Lesson 3", new TimeOnly(11, 20), new TimeOnly(12, 10)),
            new TemplatePeriod(PeriodType.Lesson, "Lesson 4", new TimeOnly(12, 10), new TimeOnly(13, 0)),
            new TemplatePeriod(PeriodType.Break, dayPlanPattern.BreakTemplates[1].Name, new TimeOnly(13, 0), new TimeOnly(13, 30)),
            new TemplatePeriod(PeriodType.Lesson, "Lesson 5", new TimeOnly(13, 30), new TimeOnly(14, 20)),
            new TemplatePeriod(PeriodType.Lesson, "Lesson 6", new TimeOnly(14, 20), new TimeOnly(15, 10))
        };

        return WeekStructure.Create(periodTemplates, new TeacherId(Guid.NewGuid()));
    }

    internal static AccountSetupRequest CreateAccountSetupRequest()
    {
        return new AccountSetupRequest(CreateSubjectNames(), CreateYearLevelsTaughtAsStringList(), CreateDayPlanPatternDto());
    }
    internal static AccountSetupRequest CreateAccountSetupRequestWithOverlappingTimes()
    {
        return new AccountSetupRequest(CreateSubjectNames(), CreateYearLevelsTaughtAsStringList(), CreateDayPlanPatternDtoWithOverlappingTimes());
    }

    internal static AccountSetupRequest CreateAccountSetupRequestWithOverlappingDates()
    {
        return new AccountSetupRequest(CreateSubjectNames(), CreateYearLevelsTaughtAsStringList(), CreateDayPlanPatternDto());
    }

    internal static List<CurriculumSubject> CreateCurriculumSubjects(List<string> subjectNames)
    {
        return subjectNames.Select(subjectNames => CurriculumSubject.Create(subjectNames, new(), "Description")).ToList();
    }

    internal static List<string> CreateYearLevelsTaughtAsStringList()
    {
        return new List<string> { "Year1", "Year2" };
    }

    internal static List<YearLevelValue> CreateYearLevelsTaught()
    {
        return new List<YearLevelValue> { YearLevelValue.Year1, YearLevelValue.Year2 };
    }
}
