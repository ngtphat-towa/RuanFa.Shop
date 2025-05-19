namespace RuanFa.Shop.Application.Common.Security.Permissions;

public static partial class Permission
{
    public static class AttributeGroup
    {
        public const string Create = "create:attribute-group";
        public const string Update = "update:attribute-group";
        public const string Get = "get:attribute-group";
        public const string Delete = "delete:attribute-group";

        public static readonly List<string> Module = [Create, Update, Get, Delete];
        public static readonly List<string> ViewMoudle = [Get];
    }
}
