using FluentValidation;
using RuanFa.Shop.Domain.Todo.Errors;

namespace RuanFa.Shop.Application.Todo.Update.UpdateTodoList;

internal class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
{
    public UpdateTodoListCommandValidator()
    {
        RuleFor(x => x.Id)
          .GreaterThan(0)
              .WithMessage(DomainErrors.TodoItem.InvalidItemId.Description)
              .WithErrorCode(DomainErrors.TodoItem.InvalidItemId.Code);

        RuleFor(x => x.Title)
            .NotEmpty()
                .WithMessage(DomainErrors.TodoList.InvalidTitle.Description)
                .WithErrorCode(DomainErrors.TodoList.InvalidTitle.Code)
            .MinimumLength(3)
                .WithMessage(DomainErrors.TodoList.TitleTooShort.Description)
                .WithErrorCode(DomainErrors.TodoList.TitleTooShort.Code)
            .MaximumLength(100)
                .WithMessage(DomainErrors.TodoList.InvalidTitleLength.Description)
                .WithErrorCode(DomainErrors.TodoList.InvalidTitleLength.Code);

        RuleFor(x => x.Colour)
            .Matches(@"^#[0-9A-Fa-f]{6}$")
                .WithMessage(DomainErrors.TodoList.InvalidColour.Description)
                .WithErrorCode(DomainErrors.TodoList.InvalidColour.Code)
                .When(x => !string.IsNullOrEmpty(x.Colour));
    }
}
