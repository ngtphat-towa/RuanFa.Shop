using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.Domain.Todo.ValueObjects;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Todo.Update.UpdateTodoList;

[ApiAuthorize(Permission.TodoList.Update)]
public record UpdateTodoListCommand : ICommand<Updated>
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public string? Colour { get; init; }
}
internal sealed class UpdateTodoListCommandHandler(IApplicationDbContext context)
    : ICommandHandler<UpdateTodoListCommand, Updated>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Updated>> Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (todoList is null)
        {
            return DomainErrors.TodoList.ListNotFound;
        }

        var isDuplicate = await _context.TodoLists
            .AnyAsync(x => x.Id != request.Id && x.Title == request.Title, cancellationToken);

        if (isDuplicate)
        {
            return DomainErrors.TodoList.DuplicateTitle(request.Title);
        }

        Colour? colour = null;
        if (!string.IsNullOrEmpty(request.Colour))
        {
            var colorResult = Colour.Create(request.Colour);
            if (colorResult.IsError)
            {
                return colorResult.Errors;
            }
            colour = colorResult.Value;
        }

        todoList.Update(request.Title, colour);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
