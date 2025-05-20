using Nasty.Core.Entity;
using Nasty.PortalModule.User;
using SqlSugar;

namespace Nasty.PortalModule.Role
{
	[SugarTable("SysRole")]
	public class Role : StandardEntity<Role>
    {
		/// <summary>
		/// 名称
		/// </summary>
		[SugarColumn(ColumnName = "Name")]
		public string? Name { get; set; }

		/// <summary>
		/// 编码
		/// </summary>
		[SugarColumn(ColumnName = "Code")]
		public string? Code { get; set; }

		/// <summary>
		/// 用户权限
		/// </summary>
		[Navigate(typeof(RolePermission), nameof(RolePermission.RoleId), nameof(RolePermission.PermissionId))]
		public List<Permission.Permission>? Permissions { get; set; }
	}
}
