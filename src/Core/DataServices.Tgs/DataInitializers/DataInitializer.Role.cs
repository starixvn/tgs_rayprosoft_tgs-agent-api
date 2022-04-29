using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.DataInitializers;

public partial class DataInitializer
{
    internal interface IRole
    {
        Guid? BrandId { get; }

        string Description { get; }

        Guid Id { get; }

        bool IsRoot { get; }

        Guid? LicenseeId { get; }

        string Name { get; }

        Guid? ResellerId { get; }

        RoleType RoleType { get; }
    }

    internal static class Roles
    {
        internal static Guid AdminRoleId = new("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44E");
        internal static Guid GpiBrandRoleId = new("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE46B");
        internal static Guid LicenseeRoleId = new("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44C");
        internal static Guid ResellerRoleId = new("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44A");
        internal static Guid TgsBrandRoleId = new("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44B");
    }

    internal class AdminRole : IRole
    {
        Guid? IRole.BrandId => null;

        string IRole.Description => "It is used for admin users.";

        Guid IRole.Id => Roles.AdminRoleId;

        bool IRole.IsRoot => true;

        Guid? IRole.LicenseeId => null;

        string IRole.Name => "超級管理員";

        Guid? IRole.ResellerId => null;

        RoleType IRole.RoleType => RoleType.Admin;
    }

    internal class GpiBrandRole : IRole
    {
        Guid? IRole.BrandId => new Guid("4E86719F-243F-46F6-B010-5C758B7285AF");

        string IRole.Description => "It is used for brands.";

        Guid IRole.Id => Roles.GpiBrandRoleId;

        bool IRole.IsRoot => true;

        Guid? IRole.LicenseeId => new Guid("66C5FEF5-217B-4929-9A08-72C2479A8F4E");

        string IRole.Name => "Gpi品牌商";

        Guid? IRole.ResellerId => new Guid("B5A2C708-AB70-4316-9F4D-5D2AD0E77460");

        RoleType IRole.RoleType => RoleType.Brand;
    }

    internal class LicenseeAdminRole : IRole
    {
        Guid? IRole.BrandId => null;

        string IRole.Description => "It is used for licensee admin.";

        Guid IRole.Id => Roles.LicenseeRoleId;

        bool IRole.IsRoot => true;

        Guid? IRole.LicenseeId => new Guid("66C5FEF5-217B-4929-9A08-72C2479A8F4E");

        string IRole.Name => "營運商";

        Guid? IRole.ResellerId => new Guid("B5A2C708-AB70-4316-9F4D-5D2AD0E77460");

        RoleType IRole.RoleType => RoleType.Licensee;
    }

    internal class ResellerAdminRole : IRole
    {
        Guid? IRole.BrandId => null;

        string IRole.Description => "It is used for reseller admin.";

        Guid IRole.Id => Roles.ResellerRoleId;

        bool IRole.IsRoot => true;

        Guid? IRole.LicenseeId => null;

        string IRole.Name => "代理商";

        Guid? IRole.ResellerId => new Guid("B5A2C708-AB70-4316-9F4D-5D2AD0E77460");

        RoleType IRole.RoleType => RoleType.Reseller;
    }

    internal class TgsBrandRole : IRole
    {
        Guid? IRole.BrandId => new Guid("D71AC5ED-B7D9-49F2-A129-8709FA4CC02A");

        string IRole.Description => "It is used for brands.";

        Guid IRole.Id => Roles.TgsBrandRoleId;

        bool IRole.IsRoot => true;

        Guid? IRole.LicenseeId => new Guid("66C5FEF5-217B-4929-9A08-72C2479A8F4E");

        string IRole.Name => "TGS品牌商";

        Guid? IRole.ResellerId => new Guid("B5A2C708-AB70-4316-9F4D-5D2AD0E77460");

        RoleType IRole.RoleType => RoleType.Brand;
    }
}
