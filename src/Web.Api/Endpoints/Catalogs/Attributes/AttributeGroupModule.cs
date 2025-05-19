using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RuanFa.Shop.Application.Catalogs.Attributes.Groups.Create;
using RuanFa.Shop.Application.Catalogs.Attributes.Groups.Delete;
using RuanFa.Shop.Application.Catalogs.Attributes.Groups.GetById;
using RuanFa.Shop.Application.Catalogs.Attributes.Groups.GetList;
using RuanFa.Shop.Application.Catalogs.Attributes.Groups.Update;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Groups;
using RuanFa.Shop.SharedKernel.Models.Wrappers;
using RuanFa.Shop.Web.Api.Extensions;

namespace RuanFa.Shop.Web.Api.Endpoints.Catalogs.Attributes;

/// <summary>
/// Provides API endpoints for managing attribute groups
/// </summary>
public class AttributeGroupModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog/attribute-groups")
            .WithTags("Catalog Attribute Groups")
            .WithOpenApi()
            .RequireAuthorization();

        // Get all attribute groups
        group.MapGet("/", GetAttributeGroups)
            .WithName("GetAttributeGroups")
            .WithDescription("Retrieves a paginated list of attribute groups")
            .WithSummary("Get all attribute groups")
            .Produces<PaginatedList<AttributeGroupListResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Get attribute group by ID
        group.MapGet("/{id:guid}", GetAttributeGroupById)
            .WithName("GetAttributeGroupById")
            .WithDescription("Retrieves a specific attribute group by its ID")
            .WithSummary("Get attribute group by ID")
            .Produces<AttributeGroupResult>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Create a new attribute group
        group.MapPost("/", CreateAttributeGroup)
            .WithName("CreateAttributeGroup")
            .WithDescription("Creates a new attribute group")
            .WithSummary("Create a new attribute group")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Update an existing attribute group
        group.MapPut("/{id:guid}", UpdateAttributeGroup)
            .WithName("UpdateAttributeGroup")
            .WithDescription("Updates an existing attribute group")
            .WithSummary("Update an attribute group")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // Delete an attribute group
        group.MapDelete("/{id:guid}", DeleteAttributeGroup)
            .WithName("DeleteAttributeGroup")
            .WithDescription("Deletes an attribute group")
            .WithSummary("Delete an attribute group")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAttributeGroups(
        [AsParameters] GetAttributeGroupsQuery query,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    private static async Task<IResult> GetAttributeGroupById(
        Guid id,
        [FromServices] ISender sender)
    {
        var query = new GetAttributeGroupByIdQuery { Id = id };
        var result = await sender.Send(query);
        return result.ToTypedResult();
    }

    private static async Task<IResult> CreateAttributeGroup(
        [FromBody] CreateAttributeGroupCommand command,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(command);
        return result.ToTypedResultCreated($"/api/catalog/attribute-groups/{result.Value}");
    }

    private static async Task<IResult> UpdateAttributeGroup(
        Guid id,
        [FromBody] UpdateAttributeGroupCommand command,
        [FromServices] ISender sender)
    {
        var commandWithId = command with { Id = id };
        var result = await sender.Send(commandWithId);
        return result.ToTypedResultNoContent();
    }

    private static async Task<IResult> DeleteAttributeGroup(
        Guid id,
        [FromServices] ISender sender)
    {
        var command = new DeleteAttributeGroupCommand { Id = id };
        var result = await sender.Send(command);
        return result.ToTypedResultDeleted();
    }
}
