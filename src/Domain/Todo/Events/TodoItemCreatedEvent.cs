using RuanFa.Shop.Domain.Todo.Entities;
using RuanFa.Shop.SharedKernel.Models;

namespace RuanFa.Shop.Domain.Todo.Events;
public sealed class TodoItemCreatedEvent : DomainEvent
{
    public TodoItemCreatedEvent(TodoItem item) => Item = item;

    public TodoItem Item { get; }
}
