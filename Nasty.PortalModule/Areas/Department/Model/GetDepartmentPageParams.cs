using Nasty.Common.LoadParams;

namespace Nasty.PortalModule.Areas.Department.Model
{
    public class GetDepartmentPageParams : PageParams
    {
        public string? Name { get; set; }

        public string? Code { get; set; }
    }
}
