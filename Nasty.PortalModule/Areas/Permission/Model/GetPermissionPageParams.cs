using Nasty.Common.LoadParams;

namespace Nasty.PortalModule.Areas.Permission.Model
{
    public class GetPermissionPageParams : PageParams
    {
        public string? Name { get; set; }

        public string? Code { get; set; }
    }
}
