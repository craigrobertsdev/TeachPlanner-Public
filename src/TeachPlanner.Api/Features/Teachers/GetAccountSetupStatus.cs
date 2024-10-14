using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Database;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.Teachers;

public static class GetAccountSetupStatus
{
    public static async Task<IResult> Endpoint([FromRoute] Guid teacherId, ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var teacher = await context.Teachers
            .Where(t => t.Id == new TeacherId(teacherId))
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher is not null)
        {
            return Results.Ok();
        }

        return Results.NotFound();
    }
}