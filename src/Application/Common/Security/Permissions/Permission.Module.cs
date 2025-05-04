namespace RuanFa.Shop.Application.Common.Security.Permissions;
public static partial class Permission
{
    public static readonly List<string> Administrator =[       
        .. TodoList.Module,
        .. TodoItem.Module,
    ];

    public static readonly List<string> User = [
        .. TodoList.Module,
        .. TodoItem.Module,
    ];
}
