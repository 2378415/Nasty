namespace Nasty.PortalModule.Areas.Permission.Model
{
    public class SaveRolePermissionModel
    {
        public required string RoleId { get; set; }

        public required List<string> PermissionIds { get; set; }
    }
}
