using System.ComponentModel.DataAnnotations;
using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.BlazorClient.Models.Subjects;

public record Subject
{
    [Required] public SubjectId SubjectId { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public List<YearLevel> YearLevels { get; set; } = [];
}

public static class SubjectExtensions
{
    public static List<Subject> ConvertFromDtos(this IEnumerable<CurriculumSubjectDto> subjects)
    {
        return subjects.Select(s => new Subject
            {
                SubjectId = new SubjectId(s.Id), Name = s.Name, YearLevels = s.YearLevels.ConvertFromDtos()
            })
            .ToList();
    }

    public static Subject ConvertFromDto(this CurriculumSubjectDto subject)
    {
        return new Subject
        {
            SubjectId = new SubjectId(subject.Id),
            Name = subject.Name,
            YearLevels = subject.YearLevels.ConvertFromDtos()
        };
    }
}