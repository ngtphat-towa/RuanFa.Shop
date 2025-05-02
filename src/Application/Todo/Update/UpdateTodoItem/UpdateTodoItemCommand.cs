using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Todo.Update.UpdateTodoItem;

[ApiAuthorize(Permission.TodoItem.Update)]
public record UpdateTodoItemCommand : ICommand<Updated>
{
    public required int Id { get; init; } // Item Id
    public required string Title { get; init; }
    public string? Note { get; init; }
    public int Priority { get; init; } = 0; // Default None
}

internal sealed class UpdateTodoItemCommandHandler(IApplicationDbContext context)
    : ICommandHandler<UpdateTodoItemCommand, Updated>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Updated>> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (todoItem is null)
        {
            return DomainErrors.TodoItem.ListNotFound; 
        }

        todoItem.Update(
            title: request.Title,
            note: request.Note);

        todoItem.UpdatePriority((Domain.Todo.Enums.PriorityLevel)request.Priority);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
