namespace RuanFa.Shop.Application.Common.Security.Permissions;

public static partial class Permission
{
    public static class TodoItem
    {
        public const string Create = "create:todo-item";
        public const string Update = "update:todo-item";
        public const string Get = "get:todo-item";
        public const string Delete = "delete:todo-item";

        public static readonly List<string> Module = [Create, Update, Get, Delete];
    }
}
