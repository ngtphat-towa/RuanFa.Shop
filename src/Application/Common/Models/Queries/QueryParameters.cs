namespace RuanFa.Shop.Application.Common.Models.Queries;

public record QueryParameters
{
    public string? SearchTerm { get; set; } = null;
    public int? PageIndex { get; set; } = null;
    public int? PageSize { get; set; } = null;
    public string? SortBy { get; set; } = null;
    public string? SortDirection { get; set; } = "asc";
}
