using Nasty.Common.Model;
using SqlSugar;

namespace Nasty.PortalModule.Areas.Department.Model
{
	public class SaveDepartmentUserModel
    {
        public required string DepartmentId { get; set; }

        public required List<string> UserIds { get; set; }
    }
}
