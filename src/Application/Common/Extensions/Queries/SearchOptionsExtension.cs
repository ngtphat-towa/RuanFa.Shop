using System.Linq.Expressions;
using System.Reflection;

namespace RuanFa.Shop.Application.Common.Extensions.Queries;

public static class SearchOptionsExtension
{
    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string? searchTerm, string? propertyName)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return query;
        }

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            // Search all string properties when propertyName is null or empty
            var stringProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => p.Name)
                .ToList();

            if (stringProperties.Count == 0)
            {
                return query;
            }

            var xParam = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var propName in stringProperties)
            {
                var propInfoLoop = typeof(T).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (propInfoLoop == null)
                {
                    continue;
                }

                MemberExpression propertyExpr = Expression.Property(xParam, propInfoLoop);
                var searchTermExpression = Expression.Constant(searchTerm.ToLower());
                var propertyToLower = Expression.Call(propertyExpr, nameof(string.ToLower), null);
                var containsMethod = Expression.Call(propertyToLower, nameof(string.Contains), null, searchTermExpression);

                combinedExpression = combinedExpression == null
                    ? containsMethod
                    : Expression.OrElse(combinedExpression, containsMethod);
            }

            if (combinedExpression == null)
            {
                return query;
            }

            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, xParam);
            return query.Where(lambda);
        }

        var propInfo = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (propInfo == null)
        {
            return query;
        }

        var xParamOuter = Expression.Parameter(typeof(T), "x");
        var propertyOuter = Expression.Property(xParamOuter, propInfo);
        var propertyType = propInfo.PropertyType;

        // Handle supported types
        if (propertyType == typeof(string))
        {
            var searchTermExpression = Expression.Constant(searchTerm.ToLower());
            var propertyToLower = Expression.Call(propertyOuter, nameof(string.ToLower), null);
            var containsMethod = Expression.Call(propertyToLower, nameof(string.Contains), null, searchTermExpression);
            var lambda = Expression.Lambda<Func<T, bool>>(containsMethod, xParamOuter);
            return query.Where(lambda);
        }
        else if (propertyType == typeof(int) && int.TryParse(searchTerm, out var intValue))
        {
            var constant = Expression.Constant(intValue);
            var equal = Expression.Equal(propertyOuter, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, xParamOuter);
            return query.Where(lambda);
        }
        else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTimeOffset) ||
             Nullable.GetUnderlyingType(propertyType) == typeof(DateTime) ||
             Nullable.GetUnderlyingType(propertyType) == typeof(DateTimeOffset))
        {
            // Check if it's a nullable type
            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            bool isNullable = underlyingType != null;

            // Determine the actual type to work with (DateTime or DateTimeOffset)
            Type actualType = isNullable ? underlyingType! : propertyType;
            bool isDateTimeOffset = actualType == typeof(DateTimeOffset);

            // Try parsing the search term
            bool parsedSuccessfully = false;
            DateTime dateTimeValue = default;
            DateTimeOffset dateOffsetValue = default;

            // For EF Core in-memory provider, we need a simple approach to date comparison
            if (isDateTimeOffset)
            {
                parsedSuccessfully = DateTimeOffset.TryParse(searchTerm, out dateOffsetValue);
            }
            else
            {
                parsedSuccessfully = DateTime.TryParse(searchTerm, out dateTimeValue);
            }

            if (parsedSuccessfully)
            {
                var xParam = Expression.Parameter(typeof(T), "x");
                var propertyExpression = Expression.Property(xParam, propInfo);
                Expression comparisonExpression;

                // Get just the date part for comparison (ignore time)
                var searchDate = isDateTimeOffset
                    ? dateOffsetValue.Date
                    : dateTimeValue.Date;

                if (isNullable)
                {
                    // For nullable DateTime/DateTimeOffset
                    var hasValueProperty = Expression.Property(propertyExpression, "HasValue");
                    var valueProperty = Expression.Property(propertyExpression, "Value");

                    // Get the Date property from the value
                    var dateProperty = Expression.Property(valueProperty, "Date");

                    // Create constant for comparison
                    var dateConstant = isDateTimeOffset
                        ? Expression.Constant(searchDate, typeof(DateTime))
                        : Expression.Constant(searchDate, typeof(DateTime));

                    // Build the equality check for the date parts
                    var dateEquals = Expression.Equal(dateProperty, dateConstant);

                    // Only compare if the property has a value
                    comparisonExpression = Expression.AndAlso(hasValueProperty, dateEquals);
                }
                else
                {
                    // For non-nullable DateTime/DateTimeOffset
                    var dateProperty = Expression.Property(propertyExpression, "Date");
                    var dateConstant = isDateTimeOffset
                        ? Expression.Constant(searchDate, typeof(DateTime))
                        : Expression.Constant(searchDate, typeof(DateTime));

                    comparisonExpression = Expression.Equal(dateProperty, dateConstant);
                }

                var lambda = Expression.Lambda<Func<T, bool>>(comparisonExpression, xParam);
                return query.Where(lambda);
            }

            return query;


        }

        // Return original query for unsupported types
        return query;
    }

    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string? searchTerm, params string[]? propertyNames)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return query;
        }

        var stringProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(string))
            .Select(p => p.Name)
            .ToList();

        var validProperties = propertyNames != null && propertyNames.Any()
            ? propertyNames.Intersect(stringProperties, StringComparer.OrdinalIgnoreCase).ToList()
            : stringProperties; // Null or empty propertyNames search all string properties

        if (validProperties.Count == 0)
        {
            return query;
        }

        var xParam = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var propName in validProperties)
        {
            var propInfoLoop = typeof(T).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (propInfoLoop == null)
            {
                continue;
            }

            var propertyExpr = Expression.Property(xParam, propInfoLoop);
            var searchTermExpression = Expression.Constant(searchTerm.ToLower());
            var propertyToLower = Expression.Call(propertyExpr, nameof(string.ToLower), null);
            var containsMethod = Expression.Call(propertyToLower, nameof(string.Contains), null, searchTermExpression);

            combinedExpression = combinedExpression == null
                ? containsMethod
                : Expression.OrElse(combinedExpression, containsMethod);
        }

        if (combinedExpression == null)
        {
            return query;
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, xParam);
        return query.Where(lambda);
    }

    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string? searchTerm, params Expression<Func<T, string>>[]? propertySelectors)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return query;
        }

        var selectors = propertySelectors?.Length > 0
            ? propertySelectors
            : typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string))
                .Select(p =>
                {
                    var paramExpr = Expression.Parameter(typeof(T), "p");
                    return Expression.Lambda<Func<T, string>>(Expression.Property(paramExpr, p), paramExpr);
                })
                .ToArray();

        if (selectors == null || !selectors.Any())
        {
            return query;
        }

        var xParam = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var selector in selectors)
        {
            var replacedSelector = new ParameterReplacer(xParam).VisitAndConvert(selector.Body, nameof(ApplySearch));
            var searchTermExpression = Expression.Constant(searchTerm.ToLower());
            var propertyToLower = Expression.Call(replacedSelector, nameof(string.ToLower), null);
            var containsMethod = Expression.Call(propertyToLower, nameof(string.Contains), null, searchTermExpression);

            combinedExpression = combinedExpression == null
                ? containsMethod
                : Expression.OrElse(combinedExpression, containsMethod);
        }

        if (combinedExpression == null)
        {
            return query;
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, xParam);
        return query.Where(lambda);
    }

    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return query.Where(predicate);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        public ParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameter;
        }
    }
}
