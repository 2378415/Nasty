using Microsoft.AspNetCore.Mvc;
using Nasty.Common.LoadParams;
using Nasty.Common.Registry;
using Nasty.PortalModule.Areas.Role.Model;
using NetTaste;

namespace Nasty.PortalModule.Role
{
	public interface IRoleService : IAutofacRegister
	{
		public Role GetRole(string id);

		public ResultData<Role> SaveRole(RoleModel model);

		public ResultData<string> DeleteRoles(List<string> ids);
	}

	public class RoleService : IRoleService
	{
		public required IRoleRepository RoleRepository { get; set; }

		public ResultData<string> DeleteRoles(List<string> ids)
		{
			return RoleRepository.DeleteRoles(ids);
		}

		public Role GetRole(string id)
		{
			return RoleRepository.GetRole(id);
		}

		
		public ResultData<Role> SaveRole(RoleModel model)
		{
			return RoleRepository.SaveRole(model);
		}
	}
}
