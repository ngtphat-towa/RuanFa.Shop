using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Aduits;
internal class HasDomainEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IHasDomainEvent
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Ignore Domain Events
        builder.Ignore(ca => ca.DomainEvents);
    }
}
