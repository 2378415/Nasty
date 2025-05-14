using Nasty.Core.Entity;
using Nasty.PortalModule.User;
using SqlSugar;

namespace Nasty.PortalModule.Permission
{
	[SugarTable("SysPermission")]
	public class Permission : StandardEntity
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
		/// 组Id
		/// </summary>
		[SugarColumn(ColumnName = "GroupId")]
		public string? GroupId { get; set; }


		public override void OnPreAdd()
		{
			base.OnPreAdd();
		}
	}
}
