using RuanFa.Shop.Domain.Catalogs.AggregateRoots;

namespace RuanFa.Shop.Tests.Shared.Constants;

public static partial class DataConstants
{
    public static class AttributeGroups
    {
        public const string GeneralGroupName = "General";
        public const string SpecsGroupName = "Specifications";
        public const string DisplayGroupName = "Display Options";

        public static readonly AttributeGroup GeneralGroup = AttributeGroup.Create(GeneralGroupName).Value;
        public static readonly AttributeGroup SpecsGroup = AttributeGroup.Create(SpecsGroupName).Value;
        public static readonly AttributeGroup DisplayGroup = AttributeGroup.Create(DisplayGroupName).Value;

        public static readonly List<AttributeGroup> DefaultGroups = new()
        {
            GeneralGroup,
            SpecsGroup,
            DisplayGroup
        };
    }
}
