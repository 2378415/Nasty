using Nasty.PortalModule.User;

namespace Nasty.PortalModule.Areas.User.Model
{
	public class SaveUserRoleModel
	{
		public required string UserId { get; set; }

		public required List<string> RoleIds{ get; set; }
	}
}
