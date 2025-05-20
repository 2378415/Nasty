using Nasty.Common.Model;
using SqlSugar;

namespace Nasty.PortalModule.Areas.Permission.Model
{
	public class PermissionGroupModel : SaveModel
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// 编码
		/// </summary>
		public string? Code { get; set; }
	}
}
