namespace RuanFa.Shop.Application.Common.Security.Authorization.Attributes;

public static class AttributeExtentions
{
    public static class PolicyPrefix
    {
        public static string Permission => $"{nameof(Permission).ToLower()}:";
        public static string Role => $"{nameof(Role).ToLower()}:";
        public static string Policy => $"{nameof(Policy).ToLower()}:";
    }
}
