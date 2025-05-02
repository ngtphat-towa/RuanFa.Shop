using Mapster;
using RuanFa.Shop.Application.Todo.Models;
using RuanFa.Shop.Domain.Todo;
using RuanFa.Shop.Domain.Todo.Entities;

namespace RuanFa.Shop.Application.Todo.Mappings;

public class TodoProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TodoList, TodoListResult>();

        config.NewConfig<TodoItem, TodoItemResult>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Done, src => src.Done)
            .Map(dest => dest.Priority, src =>(int)src.Priority);
    }
}
