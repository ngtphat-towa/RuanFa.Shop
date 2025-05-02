namespace RuanFa.Shop.Application.Todo.Models;

public record TodoListResult
{
    public int Id { get; init; }
    public string? Title { get; init; }
    public string? Colour { get; init; }
    public List<TodoItemResult> Items { get; init; } = [];
}
