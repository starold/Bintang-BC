using EnterpriseCRUD.Application.Common.Interfaces;
using EnterpriseCRUD.Infrastructure.Data;

namespace EnterpriseCRUD.Infrastructure.UnitOfWork;

/// <summary>
/// Unit of Work wrapping DbContext.SaveChangesAsync.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
