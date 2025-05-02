using System.Linq.Expressions;
using RuanFa.Shop.SharedKernel.Enums;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Common.Extensions.Queries.ExpressionBuilding;

public static class FilterExpressionFactory
{
    public static Expression? Build(FilterCriteria criterion, MemberExpression property, Type targetType)
    {
        var isNullable = targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>);
        var nonNullableType = isNullable ? Nullable.GetUnderlyingType(targetType)! : targetType;

        return criterion.Operator switch
        {
            FilterOperator.IsNull => ExpressionBuilders.BuildIsNull(property, targetType),
            FilterOperator.IsNotNull => ExpressionBuilders.BuildIsNotNull(property, targetType),
            _ => ExpressionBuilders.BuildNonNull(criterion, property, targetType, nonNullableType)
        };
    }
}
