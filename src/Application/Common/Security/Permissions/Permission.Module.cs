namespace RuanFa.Shop.Application.Common.Security.Permissions;
public static partial class Permission
{
    public static readonly List<string> Administrator =[       
        .. TodoList.Module,
        .. TodoItem.Module,
        .. AttributeGroup.Module,
        .. Attribute.Module,
        .. AttributeOption.Module,
    ];

    public static readonly List<string> User = [
        .. TodoList.Module,
        .. TodoItem.Module,
        .. AttributeGroup.ViewMoudle,
        .. Attribute.ViewMoudle,
        .. AttributeOption.ViewMoudle
    ];
}
