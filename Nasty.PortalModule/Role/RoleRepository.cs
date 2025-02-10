using Nasty.Common.LoadParams;
using Nasty.Core.Repository;
using Nasty.Core.SuperExtension;
using Nasty.PortalModule.Areas.Role.Model;
using Nasty.PortalModule.User;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Nasty.PortalModule.Role
{
	public interface IRoleRepository : IRepository<Role>
	{
		public Role GetRole(string id);

		public ResultData<Role> SaveRole(RoleModel model);

		public ResultData<string> DeleteRoles(List<string> ids);
	}

	public class RoleRepository : SqlRepository<Role>, IRoleRepository
	{
		public RoleRepository(SqlSugarClient db) : base(db)
		{
		}

		public ResultData<string> DeleteRoles(List<string> ids)
		{
			var result = new ResultData<string>();
			try
			{
				Db.Delete<Role>(ids);

				result.IsSuccess = true;
				return result;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Message = ex.Message;
				return result;
			}
		}

		public Role GetRole(string id)
		{
			return this.Db.Queryable<Role>().IncludesAllFirstLayer().InSingle(id);
		}

		public ResultData<Role> SaveRole(RoleModel model)
		{
			var result = new ResultData<Role>();
			try
			{
				var data = Db.Save<Role>(model);

				if (model.PermissionIds != null && model.PermissionIds.Count > 0 && data != null)
				{
					var objs = Db.Queryable<Permission.Permission>().In(model.PermissionIds).ToList();
					data.Permissions = objs;
					Db.UpdateNav(data).Include((t) => t.Permissions).ExecuteCommand();
				}

				result.IsSuccess = true;
				return result;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Message = ex.Message;
				return result;
			}
		}
	}
}
