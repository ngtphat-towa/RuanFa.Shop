namespace RuanFa.Shop.Application.Todo.Models;

public record TodoItemListResult
{
    public int Id { get; init; }
    public int ListId { get; init; }
    public string? Title { get; init; }
    public bool Done { get; init; }
}
