using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Todo.Delete.DeleteTodoList;

public record DeleteTodoListCommand : ICommand<Deleted>
{
    public int Id { get; init; }
}

internal sealed class DeleteTodoListCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteTodoListCommand, Deleted>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Deleted>> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (todoList is null)
            return DomainErrors.TodoList.ListNotFound;

        _context.TodoLists.Remove(todoList);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
