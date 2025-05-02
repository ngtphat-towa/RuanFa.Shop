using ErrorOr;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Extensions.Queries;
using RuanFa.Shop.Application.Todo.Models;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Todo.Get.Lists;

public record GetTodoItemListsQuery :
    TodoItemQueryParameters,
    IQuery<PaginatedList<TodoItemListResult>>;

internal sealed class GetTodoItemListsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetTodoItemListsQuery, PaginatedList<TodoItemListResult>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<ErrorOr<PaginatedList<TodoItemListResult>>> Handle(
        GetTodoItemListsQuery request,
        CancellationToken cancellationToken)
    {
        // Get the queryable Todos
        var paginatedList = await _context.TodoItems
          .Include(m => m.List)
          .AsQueryable()
          .AsNoTracking()
          .ApplyFilters(request.Filters)
          .Where(m => m.ListId == request.ListId)
          .ApplySearch(m => string.IsNullOrEmpty(request.SearchTerm)
                            || string.IsNullOrEmpty(m.Title)
                            || string.IsNullOrEmpty(m.List.Title)
                            || m.Title.Contains(request.SearchTerm)
                            || m.List.Title.Contains(request.SearchTerm))
          .ApplySort(request.SortBy, request.SortDirection)
          .ProjectToType<TodoItemListResult>(_mapper.Config)
          .CreateAsync(
              request.PageIndex,
              request.PageSize,
              cancellationToken);

        return paginatedList;
    }
}
