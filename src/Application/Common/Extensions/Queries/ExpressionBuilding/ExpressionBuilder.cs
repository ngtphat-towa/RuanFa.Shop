using System.Linq.Expressions;
using System.Reflection;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Common.Extensions.Queries.ExpressionBuilding;

public static class ExpressionBuilder
{
    public static PropertyInfo? GetPropertyInfo<T>(string propertyName)
    {
        return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
    }

    public static IQueryable<T> ApplyFilterCriterion<T>(
        IQueryable<T> query,
        FilterCriteria criterion,
        PropertyInfo propertyInfo)
    {
        try
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);

            var expression = FilterExpressionFactory.Build(criterion, property, propertyInfo.PropertyType);
            if (expression == null)
                return query;

            var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            return query.Where(lambda);
        }
        catch
        {
            return query;
        }
    }
}
