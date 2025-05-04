using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Todo.Delete.DeleteTodoItem;

[ApiAuthorize(Permission.TodoItem.Delete)]
public record DeleteTodoItemCommand : ICommand<Deleted>
{
    public int Id { get; init; }
}

internal sealed class DeleteTodoItemCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteTodoItemCommand, Deleted>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Deleted>> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (todoItem is null)
            return DomainErrors.TodoItem.NotFound;

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
