using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RuanFa.Shop.Application.Catalogs.Attributes.GetById;
using RuanFa.Shop.Application.Catalogs.Attributes.GetList;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Attributes;
using RuanFa.Shop.SharedKernel.Models.Wrappers;
using RuanFa.Shop.Web.Api.Extensions;
using RuanFa.Shop.Application.Catalogs.Attributes.Create;
using RuanFa.Shop.Application.Catalogs.Attributes.Update;
using RuanFa.Shop.Application.Catalogs.Attributes.Delete;

namespace RuanFa.Shop.Web.Api.Endpoints.Catalogs.Attributes;

/// <summary>
/// Provides API endpoints for managing catalog attributes
/// </summary>
public class AttributeModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog/attributes")
            .WithTags("Catalog Attributes")
            .WithOpenApi()
            .RequireAuthorization();

        // Get all catalog attributes
        group.MapGet("/", GetAttributes)
            .WithName("GetAttributes")
            .WithDescription("Retrieves a paginated list of catalog attributes")
            .WithSummary("Get all catalog attributes")
            .Produces<PaginatedList<AttributeListResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Get catalog attribute by ID
        group.MapGet("/{id:guid}", GetAttributeById)
            .WithName("GetAttributeById")
            .WithDescription("Retrieves a specific catalog attribute by its unique identifier")
            .WithSummary("Get catalog attribute by ID")
            .Produces<AttributeResult>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Create new catalog attribute
        group.MapPost("/", CreateAttribute)
            .WithName("CreateAttribute")
            .WithDescription("Creates a new catalog attribute")
            .WithSummary("Create a new catalog attribute")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Update existing catalog attribute
        group.MapPut("/{id:guid}", UpdateAttribute)
            .WithName("UpdateAttribute")
            .WithDescription("Updates an existing catalog attribute")
            .WithSummary("Update a catalog attribute")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Delete catalog attribute
        group.MapDelete("/{id:guid}", DeleteAttribute)
            .WithName("DeleteAttribute")
            .WithDescription("Deletes a catalog attribute")
            .WithSummary("Delete a catalog attribute")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAttributes(
        [AsParameters] GetCatalogAttributesQuery query,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    private static async Task<IResult> GetAttributeById(
        Guid id,
        [FromServices] ISender sender)
    {
        var query = new GetCatalogAttributeByIdQUery { Id = id };
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    private static async Task<IResult> CreateAttribute(
        [FromBody] CreateCatalogAttributeCommand command,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResultCreated($"/api/catalog/attributes/{result.Value}");
    }

    private static async Task<IResult> UpdateAttribute(
        Guid id,
        [FromBody] UpdateCatalogAttributeCommand command,
        [FromServices] ISender sender)
    {
        var commandWithId = command with { Id = id };
        var result = await sender.Send(commandWithId);
        return result.ToTypedResultNoContent();
    }

    private static async Task<IResult> DeleteAttribute(
        Guid id,
        [FromServices] ISender sender)
    {
        var command = new DeleteCatalogAttributeCommand { Id = id };
        var result = await sender.Send(command);
        return result.ToTypedResultDeleted();
    }
}
