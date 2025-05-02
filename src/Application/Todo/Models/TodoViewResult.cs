using RuanFa.Shop.Application.Common.Models.Queries;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Todo.Models;

public record TodoViewResult
{
    public List<LookupResult<int>> PriorityLevels { get; set; } = default!;
    public PaginatedList<TodoListResult> TodoLists { get; set; } = default!;
}
