using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using FluentValidation;
using RuanFa.Shop.Domain.Todo.Errors;

namespace RuanFa.Shop.Application.Todo.Create.CreateTodo;

[ApiAuthorize(Permission.TodoItem.Create)]
internal class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
{
    public CreateTodoListCommandValidator()
    {
        RuleFor(x => x.Title)
         .NotEmpty()
             .WithMessage(DomainErrors.TodoList.InvalidTitle.Description)
             .WithErrorCode(DomainErrors.TodoList.InvalidTitle.Code)
         .MaximumLength(100)
             .WithMessage(DomainErrors.TodoList.InvalidTitleLength.Description)
             .WithErrorCode(DomainErrors.TodoList.InvalidTitleLength.Code);

        RuleFor(x => x.Colour)
            .Matches(@"^#[0-9A-Fa-f]{6}$")
                .WithMessage(DomainErrors.TodoList.InvalidColour.Description)
                .WithErrorCode(DomainErrors.TodoList.InvalidColour.Code)
                .When(m => !string.IsNullOrEmpty(m.Colour));
    }
}
