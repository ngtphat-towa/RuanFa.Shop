using FluentValidation;
using RuanFa.Shop.Domain.Todo.Errors;

namespace RuanFa.Shop.Application.Todo.Create.CreateTodoItem;

internal class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(x => x.ListId)
            .GreaterThan(0)
                .WithMessage(DomainErrors.TodoList.InvalidListId.Description)
                .WithErrorCode(DomainErrors.TodoList.InvalidListId.Code);

        RuleFor(x => x.Title)
            .NotEmpty()
                .WithMessage(DomainErrors.TodoItem.InvalidTitle.Description)
                .WithErrorCode(DomainErrors.TodoItem.InvalidTitle.Code)
            .MinimumLength(3)
                .WithMessage(DomainErrors.TodoItem.TitleTooShort.Description)
                .WithErrorCode(DomainErrors.TodoItem.TitleTooShort.Code)
            .MaximumLength(200)
                .WithMessage(DomainErrors.TodoItem.TitleTooLong.Description)
                .WithErrorCode(DomainErrors.TodoItem.TitleTooLong.Code);

        RuleFor(x => x.Priority)
            .IsInEnum()
                .WithMessage(DomainErrors.TodoItem.InvalidPriority.Description)
                .WithErrorCode(DomainErrors.TodoItem.InvalidPriority.Code);
    }
}
