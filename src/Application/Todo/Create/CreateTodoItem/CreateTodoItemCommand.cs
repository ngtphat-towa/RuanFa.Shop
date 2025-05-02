using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Todo.Entities;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Todo.Create.CreateTodoItem;

[ApiAuthorize(Permission.TodoItem.Create)]
public sealed record CreateTodoItemCommand : ICommand<int>
{
    public required int ListId { get; init; }
    public required string Title { get; init; }
    public string? Note { get; init; }
    public int Priority { get; init; } = 0; // Default to None
}


internal sealed class CreateTodoItemCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateTodoItemCommand, int>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<int>> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .FirstOrDefaultAsync(x => x.Id == request.ListId, cancellationToken);

        if (todoList is null)
        {
            return DomainErrors.TodoList.ListNotFound;
        }

        var item = TodoItem.Create(
            title: request.Title,
            note: request.Note,
            priority: request.Priority,
            listId: request.ListId);

        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);

        return item.Id;
    }
}
