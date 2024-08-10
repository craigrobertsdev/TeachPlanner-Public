using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeachPlanner.Shared.Database;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Api.Features.Teachers;

public static class GetAccountSetupStatus
{
    public static async Task<IResult> Delegate([FromRoute] Guid teacherId, ApplicationDbContext context, CancellationToken cancellationToken)
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
