using System.Collections.ObjectModel;
using ErrorOr;
using RuanFa.Shop.Domain.Todo.Entities;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.Domain.Todo.ValueObjects;
using RuanFa.Shop.SharedKernel.Interfaces;
using RuanFa.Shop.SharedKernel.Models;

namespace RuanFa.Shop.Domain.Todo;
public class TodoList : AggregateRoot<int>, IPersistable
{
    #region
    public bool EnableSoftDelete => false;
    public bool EnableVersioning => true;
    #endregion
    #region Properties
    public string Title { get; private set; }
    public Colour Colour { get; private set; }
    private readonly List<TodoItem> _items = new();
    public IReadOnlyList<TodoItem> Items => new ReadOnlyCollection<TodoItem>(_items);


    #endregion

    #region Constructor
    private TodoList(string title, Colour colour)
    {
        Title = title;
        Colour = colour;
    }
    #endregion

    #region Factory Method
    public static ErrorOr<TodoList> Create(string title, Colour colour)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return DomainErrors.TodoList.InvalidTitle;
        }

        return new TodoList(title.Trim(), colour);
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            return DomainErrors.TodoList.InvalidTitle;
        }

        Title = newTitle.Trim();
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateColour(Colour newColour)
    {
        Colour = newColour;
        return Result.Updated;
    }

    public ErrorOr<Created> AddItem(TodoItem item)
    {
        _items.Add(item);
        return Result.Created;
    }

    public void RemoveItem(int itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
        }
    }

    public void ClearCompletedItems()
    {
        _items.RemoveAll(item => item.Done);
    }
    #endregion
}
