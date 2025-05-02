using System.Linq.Expressions;
using System.Reflection;

namespace RuanFa.Shop.Application.Common.Extensions.Queries;

public static class SortOptionsExtension
{
    /// <summary>
    /// Applies sorting to the query based on the provided sort field and direction.
    /// </summary>
    /// <typeparam name="T">The type of the entities in the query.</typeparam>
    /// <param name="query">The query to sort.</param>
    /// <param name="sortBy">The name of the property to sort by. If null or empty, sorts by 'Id' or the first property.</param>
    /// <param name="sortDirection">The sort direction, either "asc" (ascending) or "desc" (descending). Defaults to "asc".</param>
    /// <returns>The sorted query.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="sortDirection"/> is neither "asc" nor "desc".</exception>
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortBy, string? sortDirection = "asc")
    {
        // Validate sortDirection
        if (sortDirection != null && sortDirection.ToLower() is not ("asc" or "desc"))
            throw new ArgumentException($"Invalid sort direction '{sortDirection}'. Must be 'asc' or 'desc'.", nameof(sortDirection));

        if (string.IsNullOrEmpty(sortBy))
        {
            // Default to sorting by "Id" if available; otherwise, the first property
            var defaultProperty = typeof(T).GetProperty("Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                                ?? typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();
            if (defaultProperty == null)
                return query;

            sortBy = defaultProperty.Name;
            sortDirection ??= "asc";
        }

        var propertyInfo = typeof(T).GetProperty(
            sortBy,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
        );

        if (propertyInfo == null)
            return query; // Skip invalid properties

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
        var orderByExp = Expression.Lambda(propertyAccess, parameter);

        // Apply sorting
        string methodName = sortDirection?.ToLower() == "asc"
            ? nameof(Queryable.OrderBy)
            : nameof(Queryable.OrderByDescending);

        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), propertyInfo.PropertyType);

        return (IQueryable<T>)method.Invoke(null, new object[] { query, orderByExp })!;
    }
}
