using System.ComponentModel.DataAnnotations;
using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.BlazorClient.Models.Subjects;

public record SubjectModel
{
    [Required]
    public SubjectId SubjectId { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public List<YearLevelModel> YearLevels { get; set; } = [];
}

public static class SubjectModelExtensions
{
    public static List<SubjectModel> ConvertFromDtos(this IEnumerable<CurriculumSubjectDto> subjects) =>
        subjects.Select(s => new SubjectModel
        {
            SubjectId = new SubjectId(s.Id),
            Name = s.Name,
            YearLevels = s.YearLevels.ConvertFromDtos()
        })
        .ToList();

    public static SubjectModel ConvertFromDto(this CurriculumSubjectDto subject) =>
        new()
        {
            SubjectId = new SubjectId(subject.Id),
            Name = subject.Name,
            YearLevels = subject.YearLevels.ConvertFromDtos()
        };


}
