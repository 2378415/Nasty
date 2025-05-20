using Nasty.Common.LoadParams;

namespace Nasty.PortalModule.Areas.Department.Model
{
    public class GetDepartmentUserPageParams : PageParams
    {
        public required string DepartmentId { get; set; }

        public string? Name { get; set; }

        public string? Account { get; set; }
    }
}
