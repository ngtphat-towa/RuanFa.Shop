namespace RuanFa.Shop.Application.Common.Security.Permissions;

public static partial class Permission
{
    public static class TodoList
    {
        public const string Create = "create:todo-list";
        public const string Update = "update:todo-list";
        public const string Get = "get:todo-list";
        public const string Delete = "delete:todo-list";

        public static readonly List<string> Module = [Create, Update, Get, Delete];
    }
}
