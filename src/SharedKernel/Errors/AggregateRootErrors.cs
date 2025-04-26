using ErrorOr;

namespace RuanFa.Shop.SharedKernel.Errors;
public static partial class DomainErrors
{
    public static class AggregateRoot
    {
            public static Error AggregateDeleted => Error.Conflict(
                code: "AggregateRoot.Deleted",
                description: "Cannot modify a deleted aggregate");

            public static Error DomainEventNull => Error.Validation(
                code: "AggregateRoot.NullEvent",
                description: "Domain event cannot be null");

            public static Error AlreadyDeleted => Error.Conflict(
                code: "AggregateRoot.AlreadyDeleted",
                description: "Aggregate is already deleted");
        
    }
}
