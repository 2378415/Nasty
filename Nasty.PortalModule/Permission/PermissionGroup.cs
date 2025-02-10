using Nasty.Core.Entity;
using Nasty.PortalModule.User;
using SqlSugar;

namespace Nasty.PortalModule.Permission
{
	[SugarTable("SysPermissionGroup")]
	public class PermissionGroup : BaseEntity
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
		/// 权限
		/// </summary>
		[Navigate(NavigateType.OneToMany, nameof(Permission.GroupId))]
		public List<Permission>? Permissions { get; set; }
	}
}
