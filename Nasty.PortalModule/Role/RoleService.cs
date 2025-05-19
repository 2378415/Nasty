using Microsoft.AspNetCore.Mvc;
using Nasty.Common.LoadParams;
using Nasty.Common.Registry;
using Nasty.PortalModule.Areas.Permission.Model;
using Nasty.PortalModule.Areas.Role.Model;
using Nasty.PortalModule.Areas.User.Model;
using NetTaste;

namespace Nasty.PortalModule.Role
{
	public interface IRoleService : IAutofacRegister
	{
		public Role GetRole(string id);

		public List<Role> GetRoles(GetRolesParams @params);

        public ResultData<Role> SaveRole(RoleModel model);

		public ResultData<string> DeleteRoles(List<string> ids);

        public PageData<Role> GetRolePage(GetRolePageParams @params);

        public ResultData<string> SaveRolePermission(SaveRolePermissionModel model);
    }

	public class RoleService : IRoleService
	{
		public required IRoleRepository RoleRepository { get; set; }

		public ResultData<string> DeleteRoles(List<string> ids)
		{
			return RoleRepository.DeleteRoles(ids);
		}

        public PageData<Role> GetRolePage(GetRolePageParams @params)
        {
            return RoleRepository.GetRolePage(@params);
        }

        public Role GetRole(string id)
		{
			return RoleRepository.GetRole(id);
		}

		
		public ResultData<Role> SaveRole(RoleModel model)
		{
			return RoleRepository.SaveRole(model);
		}

        public ResultData<string> SaveRolePermission(SaveRolePermissionModel model)
        {
            return RoleRepository.SaveRolePermission(model);
        }

        public List<Role> GetRoles(GetRolesParams @params)
        {
            return RoleRepository.GetRoles(@params);
        }
    }
}
