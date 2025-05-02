using System.Collections;
using System.Linq.Expressions;
using System.Text.Json;
using RuanFa.Shop.SharedKernel.Enums;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Common.Extensions.Queries.ExpressionBuilding;

internal static class ExpressionBuilders
{
    public static Expression BuildIsNull(MemberExpression property, Type type)
        => !type.IsClass && !IsNullable(type)
            ? Expression.Constant(false)
            : Expression.Equal(property, Expression.Constant(null, property.Type));

    public static Expression BuildIsNotNull(MemberExpression property, Type type)
        => !type.IsClass && !IsNullable(type)
            ? Expression.Constant(true)
            : Expression.NotEqual(property, Expression.Constant(null, property.Type));

    public static Expression BuildNonNull(FilterCriteria c, MemberExpression prop, Type targetType, Type baseType)
    {
        if (IsNullable(targetType))
            prop = Expression.Property(prop, "Value");

        return c.Operator switch
        {
            FilterOperator.Equal or FilterOperator.Equals => BuildComparison(prop, c.Value, baseType, Expression.Equal),
            FilterOperator.NotEqual or FilterOperator.NotEquals => BuildComparison(prop, c.Value, baseType, Expression.NotEqual),
            FilterOperator.GreaterThan => BuildComparison(prop, c.Value, baseType, Expression.GreaterThan),
            FilterOperator.GreaterThanOrEqual => BuildComparison(prop, c.Value, baseType, Expression.GreaterThanOrEqual),
            FilterOperator.LessThan => BuildComparison(prop, c.Value, baseType, Expression.LessThan),
            FilterOperator.LessThanOrEqual => BuildComparison(prop, c.Value, baseType, Expression.LessThanOrEqual),
            FilterOperator.Contain or FilterOperator.Contains => BuildStringOp(prop, c.Value, "Contains"),
            FilterOperator.StartsWith => BuildStringOp(prop, c.Value, "StartsWith"),
            FilterOperator.EndsWith => BuildStringOp(prop, c.Value, "EndsWith"),
            FilterOperator.In => BuildIn(prop, c.Value, baseType),
            FilterOperator.NotIn => Expression.Not(BuildIn(prop, c.Value, baseType)),
            FilterOperator.Range => BuildRange(prop, c.Value, baseType),
            _ => Expression.Constant(false)
        };
    }

    private static Expression BuildComparison(
    MemberExpression prop,
    object? val,
    Type type,
    Func<Expression, Expression, BinaryExpression> cmp)
    {
        if (val == null)
            return Expression.Constant(false);

        try
        {
            // unwrap JsonElement
            if (val is JsonElement je)
            {
                val = je.ValueKind switch
                {
                    JsonValueKind.Number => type == typeof(decimal)
                        ? je.GetDecimal()
                        : je.GetInt32(),
                    JsonValueKind.String => je.GetString(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => null
                };
            }

            if (val == null)
                return Expression.Constant(false);

            // decimal path: compare directly
            if (type == typeof(decimal))
            {
                var decimalValue = Convert.ToDecimal(val);
                // prop is already decimal (or decimal?)
                return cmp(
                    prop,
                    Expression.Constant(decimalValue, typeof(decimal))
                );
            }

            // all other primitives / strings / bools
            var converted = Convert.ChangeType(val, type)!;
            return cmp(prop, Expression.Constant(converted, type));
        }
        catch
        {
            return Expression.Constant(false);
        }
    }

    // Add this helper method
    private static bool IsNumeric(this Type type)
    {
        return type == typeof(int) ||
               type == typeof(long) ||
               type == typeof(float) ||
               type == typeof(double) ||
               type == typeof(decimal) ||
               type == typeof(short) ||
               type == typeof(byte) ||
               type == typeof(uint) ||
               type == typeof(ulong) ||
               type == typeof(ushort) ||
               type == typeof(sbyte);
    }



    private static Expression BuildStringOp(MemberExpression prop, object? val, string methodName)
    {
        var strVal = val?.ToString();
        if (string.IsNullOrEmpty(strVal))
            return Expression.Constant(false);

        var nullCheck = Expression.NotEqual(prop, Expression.Constant(null, typeof(string)));
        var methodInfo = typeof(string).GetMethod(methodName, [typeof(string), typeof(StringComparison)])!;

        var methodCall = Expression.Call(
            prop,
            methodInfo,
            Expression.Constant(strVal),
            Expression.Constant(StringComparison.OrdinalIgnoreCase)
        );

        return Expression.AndAlso(nullCheck, methodCall);
    }

    private static Expression BuildIn(MemberExpression prop, object? val, Type type)
    {
        if (val == null)
            return Expression.Constant(false);

        try
        {
            // 1) extract raw values
            IEnumerable<object?> raw;
            if (val is JsonElement je && je.ValueKind == JsonValueKind.Array)
            {
                raw = je.EnumerateArray()
                        .Select(x => ConvertJsonElement(x, type))
                        .Where(x => x != null)
                        .ToList()!;
            }
            else if (val is IEnumerable list && val is not string)
            {
                raw = list.Cast<object>()
                          .Select(x => Convert.ChangeType(x, type))
                          .ToList();
            }
            else
            {
                return Expression.Constant(false);
            }

            if (!raw.Any())
                return Expression.Constant(false);

            // 2) build a strongly-typed array T[] 
            var array = Array.CreateInstance(type, raw.Count());
            int i = 0;
            foreach (var o in raw)
                array.SetValue(o, i++);

            // 3) make the Expression.Constant of IEnumerable<T>
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(type);
            var constantArray = Expression.Constant(array, enumerableType);

            // 4) pick Enumerable.Contains<T>(IEnumerable<T>, T)
            var containsMth = typeof(Enumerable)
                .GetMethods()
                .First(m => m.Name == nameof(Enumerable.Contains)
                         && m.GetParameters().Length == 2)
                .MakeGenericMethod(type);

            // 5) call it:  new T[]{…}.Contains(prop)
            return Expression.Call(containsMth, constantArray, prop);
        }
        catch
        {
            return Expression.Constant(false);
        }
    }

    private static Expression BuildRange(MemberExpression prop, object? val, Type type)
    {
        Dictionary<string, decimal> range;

        if (val is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Object)
        {
            range = new Dictionary<string, decimal>();
            foreach (var property in jsonElement.EnumerateObject())
            {
                if (decimal.TryParse(property.Value.ToString(), out var value))
                {
                    range[property.Name] = value;
                }
            }
        }
        else if (val is IDictionary<string, object> dict)
        {
            range = dict
                .Where(kvp => decimal.TryParse(kvp.Value.ToString(), out _))
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => decimal.Parse(kvp.Value.ToString()!)
                );
        }
        else
        {
            return Expression.Constant(false);
        }

        if (range.Count == 0)
            return Expression.Constant(true);

        // Fix: Cast both expressions to the base Expression type
        Expression propertyExpression = type != typeof(decimal)
            ? Expression.Convert(prop, typeof(decimal))
            : (Expression)prop;

        Expression? result = null;

        if (range.TryGetValue("min", out var minVal))
        {
            result = Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(minVal));
        }

        if (range.TryGetValue("max", out var maxVal))
        {
            var maxExpression = Expression.LessThanOrEqual(propertyExpression, Expression.Constant(maxVal));
            result = result == null
                ? maxExpression
                : Expression.AndAlso(result, maxExpression);
        }

        return result ?? Expression.Constant(true);
    }


    private static object? ConvertJsonElement(JsonElement element, Type targetType)
    {
        try
        {
            return element.ValueKind switch
            {
                JsonValueKind.Number when targetType == typeof(decimal) => element.GetDecimal(),
                JsonValueKind.Number when targetType == typeof(int) => element.GetInt32(),
                JsonValueKind.Number when targetType == typeof(double) => element.GetDouble(),
                JsonValueKind.String => Convert.ChangeType(element.GetString(), targetType),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    private static bool IsNullable(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
}
