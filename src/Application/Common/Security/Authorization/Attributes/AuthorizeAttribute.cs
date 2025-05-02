using Microsoft.AspNetCore.Authorization;

namespace RuanFa.Shop.Application.Common.Security.Authorization.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class ApiAuthorizeAttribute : AuthorizeAttribute
{
    public ApiAuthorizeAttribute(string? permissions = null)
    {
        Permissions = permissions;
        Policy = BuildPolicy();
    }

    public ApiAuthorizeAttribute(
        string? permissions = null, 
        string? policies = null, 
        string? roles = null)
    {
        Permissions = permissions;
        Policies = policies;
        Roles = roles;
        Policy = BuildPolicy();
    }

    public string? Permissions { get; set; }
    public string? Policies { get; set; }

    private string BuildPolicy()
    {
        var policyParts = new List<string>();

        if (!string.IsNullOrEmpty(Permissions))
            policyParts.Add($"{AttributeExtentions.PolicyPrefix.Permission}{Permissions}");

        if (!string.IsNullOrEmpty(Policies))
            policyParts.Add($"{AttributeExtentions.PolicyPrefix.Policy}{Policies}");

        if (!string.IsNullOrEmpty(Roles))
            policyParts.Add($"{AttributeExtentions.PolicyPrefix.Role}{Roles}");

        return string.Join(";", policyParts);
    }
}
