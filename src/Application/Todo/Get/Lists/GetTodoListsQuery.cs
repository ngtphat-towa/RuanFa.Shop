using ErrorOr;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Extensions.Queries;
using RuanFa.Shop.Application.Common.Models.Queries;
using RuanFa.Shop.Application.Todo.Models;
using RuanFa.Shop.Domain.Todo.Enums;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Todo.Get.Lists;

public record GetTodoListsQuery : TodoListQueryParameters, IQuery<TodoViewResult>
{
}

internal sealed class GetTodoListsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetTodoListsQuery, TodoViewResult>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<ErrorOr<TodoViewResult>> Handle(GetTodoListsQuery request, CancellationToken cancellationToken)
    {
        var priorityLevels = Enum.GetValues(typeof(PriorityLevel))
             .Cast<PriorityLevel>()
             .Select(p => new LookupResult<int> { Id = (int)p, Title = p.ToString() })
             .ToList();

        // Get the queryable Todos
        var paginatedList = await _context.TodoLists
          .AsQueryable()
          .AsNoTracking()
          .ApplyFilters(request.Filters)
          .ApplySearch(predicate: m=>!string.IsNullOrEmpty(request.SearchTerm) && m.Title.Contains(request.SearchTerm))
          .ApplySort(request.SortBy, request.SortDirection)
          .ProjectToType<TodoListResult>(_mapper.Config)
          .CreateAsync(
              request.PageIndex,
              request.PageSize,
              cancellationToken);

        TodoViewResult result = new()
        {
            PriorityLevels = priorityLevels,
            TodoLists = paginatedList
        };

        return result;
    }
}
