using RuanFa.Shop.Domain.Todo.Enums;
using RuanFa.Shop.SharedKernel.Models;
using ErrorOr;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.Domain.Todo.Events;

namespace RuanFa.Shop.Domain.Todo.Entities
{
    public class TodoItem : Entity<int>
    {
        #region Properties
        public string? Title { get; private set; }
        public string? Note { get; private set; }
        public PriorityLevel Priority { get; private set; }
        private bool _done;
        public bool Done
        {
            get => _done;
            set
            {
                if (value && !_done)
                {
                    AddDomainEvent(new TodoItemCompletedEvent(this));
                }

                _done = value;
            }
        }
        #endregion

        #region Relationship
        public int ListId { get; private set; }
        public TodoList List { get; private set; } = null!;
        #endregion

        #region Contructor
        private TodoItem()
        {
            Priority = PriorityLevel.None;
        }

        private TodoItem(
            string? title,
            string? note,
            PriorityLevel priority,
            int listId) : this()
        {
            Title = title;
            Note = note;
            Priority = priority;
            ListId = listId;
        }
        #endregion

        #region Methods
        public static ErrorOr<TodoItem> Create(
            string? title,
            string? note,
            int priority,
            int listId)
        {
            if (listId <= 0)
            {
                return DomainErrors.TodoItem.InvalidListId;
            }


            if (!Enum.IsDefined(typeof(PriorityLevel), priority))
            {
                return DomainErrors.TodoItem.InvalidPriority;
            }

            var item = new TodoItem(title, note, (PriorityLevel)priority, listId);
            return item;
        }
        public ErrorOr<Updated> Update(string? title, string? note)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return DomainErrors.TodoItem.InvalidTitle;
            }
            Title = title;
            Note = note;
            return Result.Updated;
        }

        public ErrorOr<Updated> UpdatePriority(int priority)
        {
            // Check if the priority is valid
            if (!Enum.IsDefined(typeof(PriorityLevel), priority))
            {
                return DomainErrors.TodoItem.InvalidPriority;
            }

            // Update the priority if valid
            Priority = (PriorityLevel)priority;
            return Result.Updated;
        }
        #endregion
    }
}
