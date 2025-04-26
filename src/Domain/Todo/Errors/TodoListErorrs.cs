using ErrorOr;

namespace RuanFa.Shop.Domain.Todo.Errors;
public static partial class DomainErrors
{
    public static class TodoList
    {
        public static Error InvalidTitle => Error.Validation(
            code: "TodoList.InvalidTitle",
            description: "Title must not be empty");
    }
}
