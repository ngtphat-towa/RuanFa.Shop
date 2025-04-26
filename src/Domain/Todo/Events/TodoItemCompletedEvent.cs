using RuanFa.Shop.Domain.Todo.Entities;
using RuanFa.Shop.SharedKernel.Models;

namespace RuanFa.Shop.Domain.Todo.Events;
public class TodoItemCompletedEvent(TodoItem item) : DomainEvent
{
    public TodoItem Item { get; } = item;
}
