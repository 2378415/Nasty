using Nasty.Common.LoadParams;
using Nasty.Core.Repository;
using Nasty.Core.SuperExtension;
using Nasty.PortalModule.Areas.Permission.Model;
using Nasty.PortalModule.Areas.Role.Model;
using Nasty.PortalModule.Permission;
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

        public PageData<Role> GetRolePage(GetRolePageParams @params);

        public ResultData<string> SaveRolePermission(SaveRolePermissionModel model);

        public List<Role> GetRoles(GetRolesParams @params);
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
                var roles = Db.Queryable<Role>().In(ids).ToList();
                foreach (var role in roles)
                {
                    if (role.Type != RoleType.Normal) throw new Exception("系统/部门角色无法删除");
                    Db.Delete(role);
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

        public Role GetRole(string id)
        {
            return this.Db.Queryable<Role>().IncludesAllFirstLayer().InSingle(id);
        }

        public PageData<Role> GetRolePage(GetRolePageParams @params)
        {
            int totalPage = 0;
            int total = 0;
            var pageData = new PageData<Role>();

            var _SQLExpress = Db.Queryable<Role>();

            if (!string.IsNullOrEmpty(@params.Name)) _SQLExpress.Where((t) => t.Name.Contains(@params.Name));
            if (!string.IsNullOrEmpty(@params.Code)) _SQLExpress.Where((t) => t.Code.Contains(@params.Code));
            if (@params.Types != null && @params.Types.Count > 0) _SQLExpress.Where((t) => @params.Types.Contains(t.Type));

            _SQLExpress = _SQLExpress.OrderBy((t) => t.CreateTime, OrderByType.Desc);

            var data = _SQLExpress.ToPageList(@params.Current, @params.PageSize, ref total, ref totalPage);

            pageData.Total = total;
            pageData.TotalPage = totalPage;
            pageData.Data = data;

            pageData.Current = @params.Current;
            pageData.PageSize = @params.PageSize;
            return pageData;
        }

        public List<Role> GetRoles(GetRolesParams @params)
        {
            var _SQLExpress = Db.Queryable<Role>();
            if (!string.IsNullOrEmpty(@params.Name)) _SQLExpress.Where((t) => t.Name.Contains(@params.Name));
            if (!string.IsNullOrEmpty(@params.Code)) _SQLExpress.Where((t) => t.Code.Contains(@params.Code));
            if (@params.Types != null && @params.Types.Count > 0) _SQLExpress.Where((t) => @params.Types.Contains(t.Type));

            return _SQLExpress.ToList();
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

        public ResultData<string> SaveRolePermission(SaveRolePermissionModel model)
        {
            var result = new ResultData<string>();
            try
            {

                var user = Db.Queryable<Role>().InSingle(model.RoleId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Message = "角色不存在!";
                    return result;
                }

                if (model.PermissionIds.Count > 0)
                {
                    var permissions = Db.Queryable<Permission.Permission>().In(model.PermissionIds).ToList();
                    user.Permissions = permissions;
                    Db.UpdateNav(user).Include((t) => t.Permissions).ExecuteCommand();
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
