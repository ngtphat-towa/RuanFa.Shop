namespace RuanFa.Shop.SharedKernel.Interfaces;

public interface IPersistable
{
    bool EnableSoftDelete { get; }
    bool EnableVersioning { get; }
}
