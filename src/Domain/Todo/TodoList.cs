using System.Collections.ObjectModel;
using RuanFa.Shop.Domain.Todo.Entities;
using RuanFa.Shop.Domain.Todo.ValueObjects;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Todo;

public class TodoList : AggregateRoot<int>
{
    #region Properties
    public string Title { get; private set; }
    public Colour? Colour { get; private set; }
    private readonly List<TodoItem> _items = new();
    public IReadOnlyList<TodoItem> Items => new ReadOnlyCollection<TodoItem>(_items);


    #endregion

    #region Constructor
    private TodoList(string title, Colour? colour)
    {
        Title = title;
        Colour = colour;
    }
    #endregion

    #region Factory Method
    public static TodoList Create(string title, Colour? colour)
    {
        return new TodoList(title.Trim(), colour);
    }
    #endregion

    #region Methods
    public TodoList Update(string? newTitle, Colour? colour)
    {
        Title = newTitle ?? Title;
        Colour = colour;

        return this;
    }

    public void AddItem(TodoItem item)
    {
        _items.Add(item);
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
