namespace RuanFa.Shop.Application.Common.Security.Permissions;

public static partial class Permission
{
    public static class AttributeOption
    {
        public const string Create = "create:attribute-option";
        public const string Update = "update:attribute-option";
        public const string Get = "get:attribute-option";
        public const string Delete = "delete:attribute-option";

        public static readonly List<string> Module = [Create, Update, Get, Delete];
        public static readonly List<string> ViewMoudle = [Get];
    }
}
