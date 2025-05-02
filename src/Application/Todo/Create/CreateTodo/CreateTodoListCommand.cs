using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using ErrorOr;
using RuanFa.Shop.Application.Common.Data;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.Domain.Todo.ValueObjects;
using RuanFa.Shop.Domain.Todo;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Todo.Create.CreateTodo;

[ApiAuthorize(Permission.TodoList.Create)]
public record CreateTodoListCommand : ICommand<int>
{
    public required string Title { get; init; }
    public string? Colour { get; init; }
}

internal sealed class CreateTodoListCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateTodoListCommand, int>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<int>> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {

        var isDuplicate = await _context.TodoLists
            .AnyAsync(m => m.Title == request.Title, cancellationToken);

        if (isDuplicate)
        {
            return DomainErrors.TodoList.DuplicateTitle(request.Title);
        }

        Colour? colour = null;
        if (!string.IsNullOrEmpty(request.Colour))
        {
            var colorResult = Colour.Create(request.Colour);
            if (colorResult.IsError) return colorResult.Errors;
        }

        var todoList = TodoList.Create(
            title: request.Title,
            colour: colour);

        _context.TodoLists.Add(todoList);
        await _context.SaveChangesAsync(cancellationToken);

        return todoList.Id;
    }
}
