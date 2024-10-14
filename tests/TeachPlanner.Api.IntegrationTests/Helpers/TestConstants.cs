using TeachPlanner.Api.Domain.Curriculum;

namespace TeachPlanner.Api.IntegrationTests.Helpers;

public static class TestConstants
{
    public static DateOnly FirstDayOfTerm2024 => new DateOnly(2024, 1, 29);
    public static List<string> SubjectNames => ["Mathematics", "English", "Science"];
    
    public static List<CurriculumSubject> CurriculumSubjects => 
        SubjectNames.Select(subjectNames => CurriculumSubject.Create(subjectNames, [], "")).ToList();
}