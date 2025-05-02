namespace RuanFa.Shop.SharedKernel.Models.Wrappers;

/// <summary>
/// Represents a paginated list of items with metadata about the pagination.
/// </summary>
/// <typeparam name="T">The type of the items in the list.</typeparam>
public class PaginatedList<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedList{T}"/> class.
    /// </summary>
    /// <param name="items">The collection of items for the current page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <param name="pageIndex">The current page index (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="totalCount"/>, <paramref name="pageIndex"/>, or <paramref name="pageSize"/> is invalid.</exception>
    public PaginatedList(IReadOnlyCollection<T> items, int totalCount, int pageIndex, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(items, nameof(items));
        if (totalCount < 0)
            throw new ArgumentOutOfRangeException(nameof(totalCount), "Total count must be non-negative.");
        if (pageIndex < 1)
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index must be greater than or equal to 1.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than or equal to 1.");

        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
    }

    /// <summary>
    /// Gets the collection of items for the current page.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; }

    /// <summary>
    /// Gets the current page index (1-based).
    /// </summary>
    public int PageIndex { get; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages && TotalPages > 0;
}
