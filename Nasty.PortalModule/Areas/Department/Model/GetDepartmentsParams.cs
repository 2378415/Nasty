using Nasty.Common.LoadParams;

namespace Nasty.PortalModule.Areas.Department.Model
{
    public class GetDepartmentsParams
    {
        public string? Code { get; set; }

        public string? Name { get; set; }

        public string? ParentId { get; set; }
    }
}
