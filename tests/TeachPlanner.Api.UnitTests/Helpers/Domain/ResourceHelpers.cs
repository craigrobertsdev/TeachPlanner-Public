using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Helpers.Domain;

public static class ResourceHelpers
{
    public static Resource CreateBasicResource()
    {
        return Resource.Create(
            new TeacherId(Guid.NewGuid()),
            "Resource Name",
            "Resource URL",
            false,
            new SubjectId(Guid.NewGuid()),
            [YearLevelValue.Reception],
            null);
    }

    public static Resource CreateBasicResource(TeacherId teacherId)
    {
        return Resource.Create(teacherId, "Resource Name", "Resource URL", false, new SubjectId(Guid.NewGuid()),
            [YearLevelValue.Reception], null);
    }

    public static List<Resource> CreateResources(TeacherId teacherId, List<SubjectId> subjectIds)
    {
        var resources = new List<Resource>();
        var random = new Random();
        for (var i = 0; i < 20; i++)
        {
            resources.Add(Resource.Create(
                teacherId,
                $"Resource {i}",
                "Resource URL",
                false,
                subjectIds[random.Next(subjectIds.Count)],
                [YearLevelValue.Reception, YearLevelValue.Year1],
                null));
        }

        return resources;
    }
}