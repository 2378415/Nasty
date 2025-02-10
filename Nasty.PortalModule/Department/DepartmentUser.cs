using Nasty.Core.Entity;
using Nasty.PortalModule.User;
using SqlSugar;

namespace Nasty.PortalModule.Department
{
	[SugarTable("SysDepartmentUser")]

	public class DepartmentUser
	{
		[SugarColumn(IsPrimaryKey = true, ColumnName = "DepartmentId")]
		public required string DepartmentId { get; set; }

		[SugarColumn(IsPrimaryKey = true, ColumnName = "UserId")]
		public required string UserId { get; set; }
	}
}
