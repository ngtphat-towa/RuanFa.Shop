using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.UnitTests.Common.Behaviors;

// Sample response type
public record Response(string Value)
{
    public static readonly Response Instance = new("Success");
}

public record NoAttributesRequest : ICommand<Response>;

[ApiAuthorize(permissions: "Read,Write", roles: "Admin", policies: "PolicyA")]
public record WithAttributesRequest : ICommand<Response>;

public record UserMessageNoAttributesRequest : IUserCommand<Response>
{
    public Guid UserId { get; set; }

    public UserMessageNoAttributesRequest(Guid userId)
    {
        UserId = userId;
    }
}

[ApiAuthorize(permissions: "Read", roles: "User")]
public record UserMessageWithAttributesRequest : IUserCommand<Response>
{
    public Guid UserId { get; set; }

    public UserMessageWithAttributesRequest(Guid userId)
    {
        UserId = userId;
    }
}

public record SimpleRequest : ICommand<Response>;
