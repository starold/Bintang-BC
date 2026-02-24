namespace EnterpriseCRUD.Application.Common.Interfaces;

/// <summary>
/// Unit of Work pattern interface for coordinating repository transactions.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
