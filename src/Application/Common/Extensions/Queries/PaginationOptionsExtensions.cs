using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Common.Extensions.Queries;

public static class PaginationOptionsExtensions
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    /// <summary>
    /// Executes the query with optional pagination.
    /// If both <paramref name="index"/> and <paramref name="size"/> are null, returns items up to the default page size.
    /// </summary>
    /// <typeparam name="T">The type of the entities in the query.</typeparam>
    /// <param name="query">The query to paginate.</param>
    /// <param name="index">The page index (1-based). Defaults to 1 if null.</param>
    /// <param name="size">The number of items per page. Defaults to 10 if null.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="PaginatedList{T}"/> containing the paginated results.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> or <paramref name="size"/> is invalid.</exception>
    public static async Task<PaginatedList<T>> CreateAsync<T>(
        this IQueryable<T> query,
        int? index = null,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        int pageIndex = index.GetValueOrDefault(1);
        int pageSize = size.GetValueOrDefault(DefaultPageSize);

        // Validate inputs
        if (pageIndex < 1)
            throw new ArgumentOutOfRangeException(nameof(index), "Page index must be greater than or equal to 1.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(size), "Page size must be greater than or equal to 1.");
        if (pageSize > MaxPageSize)
            throw new ArgumentOutOfRangeException(nameof(size), $"Page size cannot exceed {MaxPageSize}.");

        // Handle empty query
        IReadOnlyCollection<T> items;
        if (totalCount == 0)
        {
            items = Array.Empty<T>().ToList();
        }
        else
        {
            // Apply pagination
            query = query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            items = await query.ToListAsync(cancellationToken);
        }

        return new PaginatedList<T>(items, totalCount, pageIndex, pageSize);
    }
}
