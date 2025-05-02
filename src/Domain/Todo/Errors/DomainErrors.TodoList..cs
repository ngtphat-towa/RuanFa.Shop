using ErrorOr;

namespace RuanFa.Shop.Domain.Todo.Errors;

public static partial class DomainErrors
{
    public static class TodoList
    {
        public static Error InvalidListId => Error.Validation(
            code: "TodoList.InvalidListId",
            description: "The specified List Id is invalid.");
        public static Error InvalidTitle => Error.Validation(
            code: "TodoList.InvalidTitle",
            description: "Title must not be empty or contain only whitespace.");

        public static Error InvalidTitleLength => Error.Validation(
            code: "TodoList.InvalidTitleLength",
            description: "Title must not exceed 100 characters.");

        public static Error DuplicateTitle(string? title) => Error.Validation(
            code: "TodoList.DuplicateTitle",
            description: $"A todo list with the title '{title}' already exists.");

        public static Error ListNotFound => Error.NotFound(
            code: "TodoList.ListNotFound",
            description: "The specified Todo list was not found.");

        public static Error TitleTooShort => Error.Validation(
            code: "TodoList.TitleTooShort",
            description: "Title must be at least 3 characters long.");

        public static Error InvalidColour => Error.Validation(
            code: "TodoList.InvalidColour",
            description: "The specified colour is invalid.");
    }
}
