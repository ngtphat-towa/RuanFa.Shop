using ErrorOr;

namespace RuanFa.Shop.Domain.Todo.Errors;
public static partial class DomainErrors
{
    public static class TodoItem
    {
        public static Error InvalidTitle => Error.Failure(
                   code: "TodoItem.InvalidTitle",
                   description: "The title is invalid.");

        public static Error InvalidPriority => Error.Failure(
            code: "TodoItem.InvalidPriority",
            description: "The priority value is invalid.");

        public static Error InvalidListId => Error.Failure(
           code: "TodoItem.InvalidListId",
           description: "The ListId is invalid. It must be greater than zero.");
    }
}
