using Nasty.Common.Model;
using SqlSugar;

namespace Nasty.PortalModule.Areas.Permission.Model
{
	public class PermissionModel : SaveModel
	{
		/// <summary>
		/// 名称
		/// </summary>
		public required string Name { get; set; }

		/// <summary>
		/// 编码
		/// </summary>
		public required string Code { get; set; }

		/// <summary>
		/// 组Id
		/// </summary>
		public required string GroupId { get; set; }
	}
}
