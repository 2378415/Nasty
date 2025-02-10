using Nasty.Core.Entity;
using SqlSugar;

namespace Nasty.PortalModule.User
{
	[SugarTable("SysUserRole")]

	public class UserRole
	{
		[SugarColumn(IsPrimaryKey = true, ColumnName = "UserId")]
		public required string UserId { get; set; }

		[SugarColumn(IsPrimaryKey = true, ColumnName = "RoleId")]
		public required string RoleId { get; set; }
	}
}
