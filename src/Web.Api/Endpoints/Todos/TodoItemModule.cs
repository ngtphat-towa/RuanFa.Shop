using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RuanFa.Shop.Application.Todo.Create.CreateTodoItem;
using RuanFa.Shop.Application.Todo.Delete.DeleteTodoItem;
using RuanFa.Shop.Application.Todo.Get.ByIds;
using RuanFa.Shop.Application.Todo.Get.Lists;
using RuanFa.Shop.Application.Todo.Update.UpdateTodoItem;
using RuanFa.Shop.Application.Todo.Models;
using RuanFa.Shop.SharedKernel.Models.Wrappers;
using RuanFa.Shop.Web.Api.Extensions;

namespace RuanFa.Shop.Web.Api.Endpoints.Todos;

/// <summary>
/// Provides API endpoints for managing Todo Items
/// </summary>
public class TodoItemModule : ICarterModule
{
    /// <summary>
    /// Adds Todo Item routes to the application
    /// </summary>
    /// <param name="app">The endpoint route builder</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todos/items")
            .WithTags("Todo Items")
            .WithOpenApi()
            .RequireAuthorization();

        // Get all Todo Items
        group.MapGet("/", GetTodoItems)
            .WithName("GetTodoItems")
            .WithDescription("Retrieves a paginated list of todo items")
            .WithSummary("Get all todo items")
            .Produces<PaginatedList<TodoItemListResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Get Todo Item by ID
        group.MapGet("/{id}", GetTodoItemById)
            .WithName("GetTodoItemById")
            .WithDescription("Retrieves a specific todo item by its unique identifier")
            .WithSummary("Get todo item by ID")
            .Produces<TodoItemResult>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Create new Todo Item
        group.MapPost("/", CreateTodoItem)
            .WithName("CreateTodoItem")
            .WithDescription("Creates a new todo item")
            .WithSummary("Create a new todo item")
            .Produces<int>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Update existing Todo Item
        group.MapPut("/{id}", UpdateTodoItem)
            .WithName("UpdateTodoItem")
            .WithDescription("Updates an existing todo item")
            .WithSummary("Update a todo item")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Delete Todo Item
        group.MapDelete("/{id}", DeleteTodoItem)
            .WithName("DeleteTodoItem")
            .WithDescription("Deletes a todo item")
            .WithSummary("Delete a todo item")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Retrieves all todo items with pagination
    /// </summary>
    private static async Task<IResult> GetTodoItems(
        [AsParameters] GetTodoItemListsQuery query,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    /// <summary>
    /// Retrieves a specific todo item by its ID
    /// </summary>
    private static async Task<IResult> GetTodoItemById(
        int id,
        [FromServices] ISender sender)
    {
        var query = new GetTodoItemByIdQuery { Id = id };
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    /// <summary>
    /// Creates a new todo item
    /// </summary>
    private static async Task<IResult> CreateTodoItem(
        CreateTodoItemCommand command,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResultCreated($"/api/todos/items/{result.Value}");
    }

    /// <summary>
    /// Updates an existing todo item
    /// </summary>
    private static async Task<IResult> UpdateTodoItem(
        int id,
        UpdateTodoItemCommand command,
        [FromServices] ISender sender)
    {
        var commandWithId = command with { Id = id };
        var result = await sender.Send(commandWithId);
        return result.ToTypedResultNoContent();
    }

    /// <summary>
    /// Deletes a todo item
    /// </summary>
    private static async Task<IResult> DeleteTodoItem(
        int id,
        [FromServices] ISender sender)
    {
        var command = new DeleteTodoItemCommand { Id = id };
        var result = await sender.Send(command);
        return result.ToTypedResultDeleted();
    }
}
