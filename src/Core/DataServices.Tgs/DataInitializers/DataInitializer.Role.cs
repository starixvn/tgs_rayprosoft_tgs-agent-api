using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    public partial class DataInitializer
    {
        internal interface IRole
        {
            Guid Id { get; }
            string Name { get; }
            string Description { get; }
            RoleType RoleType { get; }
            Guid? ResellerId { get; }
            Guid? LicenseeId { get; }
            Guid? BrandId { get; }
            bool IsRoot { get; }
        }

        internal static class Roles
        {
            internal static Guid AdminRoleId = new("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44E");
            internal static Guid ResellerRoleId = new("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44A");
            internal static Guid LicenseeRoleId = new("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44C");
            internal static Guid TgsBrandRoleId = new("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44B");
            internal static Guid GpiBrandRoleId = new("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE46B");
        }


        internal class AdminRole : IRole
        {
            Guid IRole.Id => Roles.AdminRoleId;
            string IRole.Name => "超級管理員";
            string IRole.Description => "It is used for admin users.";
            RoleType IRole.RoleType => RoleType.Admin;
            Guid? IRole.ResellerId => null;
            Guid? IRole.LicenseeId => null;
            Guid? IRole.BrandId => null;
            bool IRole.IsRoot => true;
        }


        internal class ResellerAdminRole : IRole
        {
            Guid IRole.Id => Roles.ResellerRoleId;
            string IRole.Name => "代理商";
            string IRole.Description => "It is used for reseller admin.";
            RoleType IRole.RoleType => RoleType.Reseller;
            Guid? IRole.ResellerId => new Guid("B5A2C708-AB70-4316-9F4D-5D2AD0E77460");
            Guid? IRole.LicenseeId => null;
            Guid? IRole.BrandId => null;
            bool IRole.IsRoot => true;
        }

        internal class LicenseeAdminRole : IRole
        {
            Guid IRole.Id => Roles.LicenseeRoleId;
            string IRole.Name => "營運商";
            string IRole.Description => "It is used for licensee admin.";
            RoleType IRole.RoleType => RoleType.Licensee;
            Guid? IRole.ResellerId => new Guid("B5A2C708-AB70-4316-9F4D-5D2AD0E77460");
            Guid? IRole.LicenseeId => new Guid("66C5FEF5-217B-4929-9A08-72C2479A8F4E");
            Guid? IRole.BrandId => null;
            bool IRole.IsRoot => true;
        }

        internal class TgsBrandRole : IRole
        {
            Guid IRole.Id => Roles.TgsBrandRoleId;
            string IRole.Name => "TGS品牌商";
            string IRole.Description => "It is used for brands.";
            RoleType IRole.RoleType => RoleType.Brand;
            Guid? IRole.ResellerId => new Guid("B5A2C708-AB70-4316-9F4D-5D2AD0E77460");
            Guid? IRole.LicenseeId => new Guid("66C5FEF5-217B-4929-9A08-72C2479A8F4E");
            Guid? IRole.BrandId => new Guid("D71AC5ED-B7D9-49F2-A129-8709FA4CC02A");
            bool IRole.IsRoot => true;
        }

        internal class GpiBrandRole : IRole
        {
            Guid IRole.Id => Roles.GpiBrandRoleId;
            string IRole.Name => "Gpi品牌商";
            string IRole.Description => "It is used for brands.";
            RoleType IRole.RoleType => RoleType.Brand;
            Guid? IRole.ResellerId => new Guid("B5A2C708-AB70-4316-9F4D-5D2AD0E77460");
            Guid? IRole.LicenseeId => new Guid("66C5FEF5-217B-4929-9A08-72C2479A8F4E");
            Guid? IRole.BrandId => new Guid("4E86719F-243F-46F6-B010-5C758B7285AF");
            bool IRole.IsRoot => true;
        }
    }
}