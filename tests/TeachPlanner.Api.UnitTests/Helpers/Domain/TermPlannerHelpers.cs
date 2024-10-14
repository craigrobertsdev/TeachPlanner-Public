using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.TermPlanners;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Helpers.Domain;

internal static class TermPlannerHelpers
{
    internal static TermPlanner CreateTermPlanner()
    {
        return TermPlanner.Create(new YearDataId(Guid.NewGuid()), 2023,
            [YearLevelValue.Reception, YearLevelValue.Year1]);
    }

    internal static TermPlan CreateTermPlan(TermPlanner termPlanner, string curriculumCode, bool withSubstrands = true)
    {
        var subjects = new List<CurriculumSubject> { CreateSubject("English", curriculumCode) };
        return TermPlan.Create(termPlanner, 1, subjects);
    }

    internal static CurriculumSubject CreateSubject(string name, string curriculumCode)
    {
        var subject = CurriculumSubject.Create(name, [], "");
        var yearLevel = YearLevel.Create(YearLevelValue.Reception, "Description");
        subject.AddYearLevel(yearLevel);

        return subject;
    }

    internal static CurriculumSubject CreateSubject(string name, string curriculumCode, YearLevelValue subjectYearLevel)
    {
        var subject = CurriculumSubject.Create(name, [], "");
        var yearLevel = YearLevel.Create(subjectYearLevel, "Description");
        subject.AddYearLevel(yearLevel);

        return subject;
    }
}