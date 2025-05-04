namespace RuanFa.Shop.Infrastructure.Data.Seeds;
public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
