using Nasty.Core.Entity;
using SqlSugar;

namespace Nasty.PortalModule.User
{
	[SugarTable("SysRolePermission")]

	public class RolePermission
	{
		[SugarColumn(IsPrimaryKey = true, ColumnName = "RoleId")]
		public string? RoleId { get; set; }

		[SugarColumn(IsPrimaryKey = true, ColumnName = "PermissionId")]
		public string? PermissionId { get; set; }
	}
}
