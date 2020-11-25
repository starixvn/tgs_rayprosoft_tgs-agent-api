using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.DataInitializers
{
    public partial class DataInitializer
    {
        internal interface IRole
        {
            Guid Id { get; }
            string Name { get; }
            string Description { get; }
            RoleType RoleType { get; }
        }

        internal static class Roles
        {
            internal static Guid Admin = new Guid("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44E");
        }


        internal class AdminRole : IRole
        {
            Guid IRole.Id => new Guid("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44E");
            string IRole.Name => "超級管理員";
            string IRole.Description => "It is used for admin users.";
            RoleType IRole.RoleType => RoleType.Admin;
        }


        internal class ResellerAdminRole : IRole
        {
            Guid IRole.Id => new Guid("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44A");
            string IRole.Name => "管理員";
            string IRole.Description => "It is used for reseller admin.";
            RoleType IRole.RoleType => RoleType.Reseller;
        }

        internal class BrandRole : IRole
        {
            Guid IRole.Id => new Guid("7AA2E39B-9DA9-4E2E-9BDE-B74FDC5DE44B");
            string IRole.Name => "產品經理";
            string IRole.Description => "It is used for brands.";
            RoleType IRole.RoleType => RoleType.Licensee;
        }
    }
}