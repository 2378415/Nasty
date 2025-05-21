using Nasty.Core.Entity;
using SqlSugar;

namespace Nasty.PortalModule.User
{
	[SugarTable("SysUserRole")]

	public class UserRole
	{
		[SugarColumn(IsPrimaryKey = true, ColumnName = "UserId")]
		public string? UserId { get; set; }

		[SugarColumn(IsPrimaryKey = true, ColumnName = "RoleId")]
		public string? RoleId { get; set; }
	}
}
