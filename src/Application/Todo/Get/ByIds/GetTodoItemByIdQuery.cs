using ErrorOr;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Todo.Models;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Todo.Get.ByIds;

public record GetTodoItemByIdQuery : IQuery<TodoItemResult>
{
    public int Id { get; init; }
}

internal sealed class GetTodoItemByIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<GetTodoItemByIdQuery, ErrorOr<TodoItemResult>>
{
    public async Task<ErrorOr<TodoItemResult>> Handle(GetTodoItemByIdQuery request, CancellationToken cancellationToken)
    {
        var todoItem = await context.TodoItems
            .Include(m => m.List)
            .ProjectToType<TodoItemResult>(mapper.Config)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (todoItem is null)
        {
            return DomainErrors.TodoItem.NotFound;
        }

        return todoItem;
    }
}
