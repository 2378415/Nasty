using Nasty.Common.Model;
using SqlSugar;

namespace Nasty.PortalModule.Areas.Department.Model
{
	public class DepartmentModel : SaveModel
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// 编码
		/// </summary>
		public string? Code { get; set; }

        /// <summary>
        /// 父级部门Id
        /// </summary>
        public string? ParentId { get; set; }
    }
}
