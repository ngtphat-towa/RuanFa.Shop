using ErrorOr;

namespace RuanFa.Shop.Domain.Todo.Errors;

public static partial class DomainErrors
{
    public static class TodoItem
    {
        public static Error InvalidItemId => Error.Validation(
            code: "TodoItem.InvalidItemId",
            description: "The specified Item Id is invalid.");

        public static Error InvalidTitle => Error.Validation(
            code: "TodoItem.InvalidTitle",
            description: "Title must not be empty or contain only whitespace.");

        public static Error TitleTooLong => Error.Validation(
            code: "TodoItem.TitleTooLong",
            description: "Title must not exceed 200 characters.");

        public static Error TitleTooShort => Error.Validation(
            code: "TodoItem.TitleTooShort",
            description: "Title must be at least 3 characters long.");

        public static Error InvalidPriority => Error.Validation(
            code: "TodoItem.InvalidPriority",
            description: "The priority value is invalid.");

        public static Error InvalidDoneAt => Error.Validation(
            code: "TodoItem.InvalidDoneAt",
            description: "The done time is invalid.");

        public static Error ListNotFound => Error.NotFound(
            code: "TodoItem.ListNotFound",
            description: "The specified todo list was not found.");
        public static Error NotFound => Error.NotFound(
              code: "TodoItem.NotFound",
              description: "The specified todo item was not found.");
    }
}
