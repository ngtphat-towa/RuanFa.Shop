using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RuanFa.Shop.Application.Todo.Create.CreateTodo;
using RuanFa.Shop.Application.Todo.Delete.DeleteTodoList;
using RuanFa.Shop.Application.Todo.Get.ByIds;
using RuanFa.Shop.Application.Todo.Get.Lists;
using RuanFa.Shop.Application.Todo.Update.UpdateTodoList;
using RuanFa.Shop.Application.Todo.Models;
using RuanFa.Shop.Web.Api.Extensions;

namespace RuanFa.Shop.Web.Api.Endpoints.Todos;

/// <summary>
/// Provides API endpoints for managing Todo Lists
/// </summary>
public class TodoListModule : ICarterModule
{
    /// <summary>
    /// Adds Todo List routes to the application
    /// </summary>
    /// <param name="app">The endpoint route builder</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todos/lists")
            .WithTags("Todo Lists")
            .WithOpenApi();

        // Get all Todo Lists
        group.MapGet("/", GetTodoLists)
            .WithName("GetTodoLists")
            .WithDescription("Retrieves all todo lists")
            .WithSummary("Get all todo lists")
            .Produces<TodoViewResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Get Todo List by ID
        group.MapGet("/{id}", GetTodoListById)
            .WithName("GetTodoListById")
            .WithDescription("Retrieves a specific todo list by its unique identifier")
            .WithSummary("Get todo list by ID")
            .Produces<TodoListResult>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Create new Todo List
        group.MapPost("/", CreateTodoList)
            .WithName("CreateTodoList")
            .WithDescription("Creates a new todo list")
            .WithSummary("Create a new todo list")
            .Produces<int>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Update existing Todo List
        group.MapPut("/{id}", UpdateTodoList)
            .WithName("UpdateTodoList")
            .WithDescription("Updates an existing todo list")
            .WithSummary("Update a todo list")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Delete Todo List
        group.MapDelete("/{id}", DeleteTodoList)
            .WithName("DeleteTodoList")
            .WithDescription("Deletes a todo list")
            .WithSummary("Delete a todo list")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Retrieves all todo lists based on query parameters
    /// </summary>
    /// <param name="query">The query parameters</param>
    /// <param name="sender">The MediatR sender</param>
    /// <returns>A collection of todo lists</returns>
    private static async Task<IResult> GetTodoLists(
        [AsParameters] GetTodoListsQuery query,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    /// <summary>
    /// Retrieves a specific todo list by its ID
    /// </summary>
    /// <param name="id">The ID of the todo list</param>
    /// <param name="sender">The MediatR sender</param>
    /// <returns>The requested todo list</returns>
    private static async Task<IResult> GetTodoListById(
        int id,
        [FromServices] ISender sender)
    {
        var query = new GetTodoListByIdQuery { Id = id };
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    /// <summary>
    /// Creates a new todo list
    /// </summary>
    /// <param name="command">The command containing todo list data</param>
    /// <param name="sender">The MediatR sender</param>
    /// <returns>The ID of the created todo list</returns>
    private static async Task<IResult> CreateTodoList(
        CreateTodoListCommand command,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResultCreated($"/api/todos/lists/{result.Value}");
    }

    /// <summary>
    /// Updates an existing todo list
    /// </summary>
    /// <param name="id">The ID of the todo list to update</param>
    /// <param name="command">The command containing updated todo list data</param>
    /// <param name="sender">The MediatR sender</param>
    /// <returns>No content on success</returns>
    private static async Task<IResult> UpdateTodoList(
        int id,
        UpdateTodoListCommand command,
        [FromServices] ISender sender)
    {
        var commandWithId = command with { Id = id };
        var result = await sender.Send(commandWithId);
        return result.ToTypedResultNoContent();
    }

    /// <summary>
    /// Deletes a todo list
    /// </summary>
    /// <param name="id">The ID of the todo list to delete</param>
    /// <param name="sender">The MediatR sender</param>
    /// <returns>No content on success</returns>
    private static async Task<IResult> DeleteTodoList(
        int id,
        [FromServices] ISender sender)
    {
        var command = new DeleteTodoListCommand { Id = id };
        var result = await sender.Send(command);
        return result.ToTypedResultDeleted();
    }
}
