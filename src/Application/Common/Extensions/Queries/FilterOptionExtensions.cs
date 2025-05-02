using RuanFa.Shop.Application.Common.Extensions.Queries.ExpressionBuilding;

namespace RuanFa.Shop.Application.Common.Extensions.Queries;

public static class FilterOptionExtensions
{
    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, string? filters)
    {
        var criteriaList = filters.ToFilterCriteria();

        return criteriaList.Aggregate(query, (current, criterion) =>
        {
            var propertyInfo = ExpressionBuilder.GetPropertyInfo<T>(criterion.Field);
            return propertyInfo == null
                ? current
                : ExpressionBuilder.ApplyFilterCriterion(current, criterion, propertyInfo);
        });
    }
}
