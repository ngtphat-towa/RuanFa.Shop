using ErrorOr;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Todo.Models;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Todo.Get.ByIds;

public record GetTodoListByIdQuery : IQuery<TodoListResult>
{
    public int Id { get; set; }
}

internal sealed class GetTodoListByIdQueryHandler : IRequestHandler<GetTodoListByIdQuery, ErrorOr<TodoListResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoListByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ErrorOr<TodoListResult>> Handle(GetTodoListByIdQuery request, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .ProjectToType<TodoListResult>(_mapper.Config)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (todoList is null)
        {
            return DomainErrors.TodoList.ListNotFound;
        }

        return todoList;
    }
}
