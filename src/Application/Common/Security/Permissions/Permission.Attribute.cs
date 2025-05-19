namespace RuanFa.Shop.Application.Common.Security.Permissions;

public static partial class Permission
{
    public static class Attribute
    {
        public const string Create = "create:attribute";
        public const string Update = "update:attribute";
        public const string Get = "get:attribute";
        public const string Delete = "delete:attribute";

        public static readonly List<string> Module = [Create, Update, Get, Delete];
        public static readonly List<string> ViewMoudle = [Get];
    }
}
