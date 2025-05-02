using RuanFa.Shop.Application.Common.Models.Queries;

namespace RuanFa.Shop.Application.Todo.Models;

public record TodoItemQueryParameters : QueryParameters
{
    public int ListId { get; set; }
}
