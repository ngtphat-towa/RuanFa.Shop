using RuanFa.Shop.SharedKernel.Interfaces;

namespace RuanFa.Shop.SharedKernel.Models;
public abstract class VersionedEntity<TId> : Entity<TId>, IVersionable
    where TId : notnull
{
    public int Version { get; private set; } = 1;

    protected virtual void IncrementVersion() => Version++;
}