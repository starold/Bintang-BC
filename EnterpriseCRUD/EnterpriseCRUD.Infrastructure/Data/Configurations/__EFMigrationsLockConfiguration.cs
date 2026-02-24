using EnterpriseCRUD.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseCRUD.Infrastructure.Data.Configurations;

public class __EFMigrationsLockConfiguration : IEntityTypeConfiguration<__EFMigrationsLock>
{
    public void Configure(EntityTypeBuilder<__EFMigrationsLock> builder)
    {
        builder.Property(x => x.Timestamp).IsRequired();
    }
}
