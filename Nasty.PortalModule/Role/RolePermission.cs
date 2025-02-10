using Nasty.Core.Entity;
using SqlSugar;

namespace Nasty.PortalModule.User
{
	[SugarTable("SysRolePermission")]

	public class RolePermission
	{
		[SugarColumn(IsPrimaryKey = true, ColumnName = "RoleId")]
		public required string RoleId { get; set; }

		[SugarColumn(IsPrimaryKey = true, ColumnName = "PermissionId")]
		public required string PermissionId { get; set; }
	}
}
