using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.YearDataRecords;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Helpers.Domain;

public static class SubjectHelpers
{
    public static CurriculumSubject CreateCurriculumSubject()
    {
        return CurriculumSubject.Create("English", CreateYearLevels(), "");
    }

    public static List<CurriculumSubject> CreateCurriculumSubjects()
    {
        List<CurriculumSubject> subjects =
            [CreateCurriculumSubjectWithYearLevels(), CreateCurriculumSubjectWithBandLevels()];
        return subjects;
    }

    public static List<Subject> CreateSubjects()
    {
        List<Subject> subjects = [];

        for (var i = 0; i < 2; i++)
        {
            var subject = Subject.Create(new SubjectId(Guid.NewGuid()), "English" + i,
                []);
            subjects.Add(subject);
        }

        return subjects;
    }

    public static CurriculumSubject CreateCurriculumSubjectWithYearLevels()
    {
        return CurriculumSubject.Create("English", CreateYearLevels(), "");
    }

    public static CurriculumSubject CreateCurriculumSubjectWithBandLevels()
    {
        return CurriculumSubject.Create("Humanities and Social Sciences", CreateYearLevelsWithBandLevels(), "");
    }

    private static List<YearLevel> CreateYearLevels()
    {
        var random = new Random();
        return [YearLevel.Create(YearLevelValue.Reception, ""), YearLevel.Create(YearLevelValue.Year1, "")];
    }

    private static List<YearLevel> CreateYearLevelsWithBandLevels()
    {
        return
        [
            YearLevel.Create(
                YearLevelValue.Years1To2,
                ""),

            YearLevel.Create(
                YearLevelValue.Years3To4,
                "")
        ];
    }
}