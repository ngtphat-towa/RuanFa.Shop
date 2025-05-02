namespace RuanFa.Shop.Application.Common.Models.Queries;

public record QueryParameters
{
    public string? SearchTerm { get; set; }
    public string? Filters { get; set; }
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
}
