using OsitoPolar.Analytics.Service.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace OsitoPolar.Analytics.Service.Shared.Infrastructure.Persistence.EFC.Repositories;

public class UnitOfWork(DbContext context) : IUnitOfWork
{
    /// <inheritdoc />
    public async Task CompleteAsync()
    {
        await context.SaveChangesAsync();
    }
}