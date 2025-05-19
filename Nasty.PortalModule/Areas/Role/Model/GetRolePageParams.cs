using Nasty.Common.LoadParams;

namespace Nasty.PortalModule.Areas.Role.Model
{
    public class GetRolePageParams : PageParams
    {
        public string? Name { get; set; }

        public string? Code { get; set; }
    }
}
