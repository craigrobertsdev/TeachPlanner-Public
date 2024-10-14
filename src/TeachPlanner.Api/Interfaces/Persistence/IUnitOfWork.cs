using Microsoft.EntityFrameworkCore.Storage;

namespace TeachPlanner.Api.Interfaces.Persistence;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    IDbContextTransaction BeginTransaction();
}