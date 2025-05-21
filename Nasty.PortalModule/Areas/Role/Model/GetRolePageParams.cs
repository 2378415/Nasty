using Nasty.Common.LoadParams;
using Nasty.PortalModule.Role;

namespace Nasty.PortalModule.Areas.Role.Model
{
    public class GetRolePageParams : PageParams
    {
        public string? Name { get; set; }

        public string? Code { get; set; }

        public List<RoleType?>? Types { get; set; }
    }
}
