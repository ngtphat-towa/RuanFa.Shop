using RuanFa.Shop.Application.Common.Services;

namespace RuanFa.Shop.Infrastructure.Services;

internal class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
