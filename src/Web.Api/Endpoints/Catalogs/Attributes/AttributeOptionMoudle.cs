using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RuanFa.Shop.Application.Catalogs.Attributes.Options.Create;
using RuanFa.Shop.Application.Catalogs.Attributes.Options.Delete;
using RuanFa.Shop.Application.Catalogs.Attributes.Options.GetById;
using RuanFa.Shop.Application.Catalogs.Attributes.Options.GetList;
using RuanFa.Shop.Application.Catalogs.Attributes.Options.Update;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Options;
using RuanFa.Shop.SharedKernel.Models.Wrappers;
using RuanFa.Shop.Web.Api.Extensions;

namespace RuanFa.Shop.Web.Api.Endpoints.Catalogs.Attributes;

/// <summary>
/// Provides API endpoints for managing attribute options
/// </summary>
public class AttributeOptionModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog/attribute-options")
            .WithTags("Catalog Attribute Options")
            .WithOpenApi()
            .RequireAuthorization();

        // Get all attribute options
        group.MapGet("/", GetAttributeOptions)
            .WithName("GetAttributeOptions")
            .WithDescription("Retrieves a paginated list of attribute options")
            .WithSummary("Get all attribute options")
            .Produces<PaginatedList<AttributeOptionListResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Get attribute option by ID
        group.MapGet("/{id:guid}", GetAttributeOptionById)
            .WithName("GetAttributeOptionById")
            .WithDescription("Retrieves a specific attribute option by its ID")
            .WithSummary("Get attribute option by ID")
            .Produces<AttributeOptionResult>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Create a new attribute option
        group.MapPost("/", CreateAttributeOption)
            .WithName("CreateAttributeOption")
            .WithDescription("Creates a new attribute option")
            .WithSummary("Create an attribute option")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Update an existing attribute option
        group.MapPut("/{id:guid}", UpdateAttributeOption)
            .WithName("UpdateAttributeOption")
            .WithDescription("Updates an existing attribute option")
            .WithSummary("Update an attribute option")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Delete an attribute option
        group.MapDelete("/{id:guid}", DeleteAttributeOption)
            .WithName("DeleteAttributeOption")
            .WithDescription("Deletes an attribute option")
            .WithSummary("Delete an attribute option")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAttributeOptions(
        [AsParameters] GetAttributeOptionsQuery query,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    private static async Task<IResult> GetAttributeOptionById(
        Guid id,
        [FromServices] ISender sender)
    {
        var query = new GetAttributeOptionByIdQuery { Id = id };
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    private static async Task<IResult> CreateAttributeOption(
        [FromBody] CreateAttributeOptionCommand command,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResultCreated($"/api/catalog/attribute-options/{result.Value}");
    }

    private static async Task<IResult> UpdateAttributeOption(
        Guid id,
        [FromBody] UpdateAttributeOptionCommand command,
        [FromServices] ISender sender)
    {
        var commandWithId = command with { Id = id };
        var result = await sender.Send(commandWithId);
        return result.ToTypedResultNoContent();
    }

    private static async Task<IResult> DeleteAttributeOption(
        Guid id,
        [FromServices] ISender sender)
    {
        var command = new DeleteAttributeOptionCommand { Id = id };
        var result = await sender.Send(command);
        return result.ToTypedResultDeleted();
    }
}
