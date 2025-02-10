

using Nasty.Common.Model;

namespace Nasty.PortalModule.Areas.Role.Model
{
	public class RoleModel : SaveModel
	{
		public string? Name { get; set; }

		public string? Code { get; set; }

		public List<string>? PermissionIds { get; set; }
	}
}
