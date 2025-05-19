using Microsoft.AspNetCore.Mvc;
using Nasty.Common.LoadParams;
using Nasty.Common.Registry;
using Nasty.PortalModule.Areas.Permission.Model;
using NetTaste;

namespace Nasty.PortalModule.Permission
{
	public interface IPermissionService : IAutofacRegister
	{
		public Permission GetPermission(string id);

		public ResultData<Permission> SavePermission(PermissionModel model);

		public ResultData<string> DeletePermissions(List<string> ids);


		public PermissionGroup GetPermissionGroup(string id);

		public ResultData<PermissionGroup> SavePermissionGroup(PermissionGroupModel model);

		public ResultData<string> DeletePermissionGroups(List<string> ids);


        public PageData<PermissionGroup> GetPermissionGroupPage(GetPermissionGroupPageParams @params);
    }

	public class PermissionService : IPermissionService
	{
		public required IPermissionRepository PermissionRepository { get; set; }

		public ResultData<string> DeletePermissions(List<string> ids)
		{
			return PermissionRepository.DeletePermissions(ids);
		}

		public Permission GetPermission(string id)
		{
			return PermissionRepository.GetPermission(id);
		}

        public PageData<PermissionGroup> GetPermissionGroupPage(GetPermissionGroupPageParams @params)
        {
            return PermissionRepository.GetPermissionGroupPage(@params);
        }

        public ResultData<Permission> SavePermission(PermissionModel model)
		{
			return PermissionRepository.SavePermission(model);
		}

		ResultData<string> IPermissionService.DeletePermissionGroups(List<string> ids)
		{
			return PermissionRepository.DeletePermissionGroups(ids);
		}

		PermissionGroup IPermissionService.GetPermissionGroup(string id)
		{
			return PermissionRepository.GetPermissionGroup(id);
		}

		ResultData<PermissionGroup> IPermissionService.SavePermissionGroup(PermissionGroupModel model)
		{
			return PermissionRepository.SavePermissionGroup(model);
		}
	}
}
