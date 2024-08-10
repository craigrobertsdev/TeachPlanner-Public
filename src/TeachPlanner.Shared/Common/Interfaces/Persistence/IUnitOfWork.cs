using Microsoft.EntityFrameworkCore.Storage;

namespace TeachPlanner.Shared.Common.Interfaces.Persistence;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    IDbContextTransaction BeginTransaction();
}