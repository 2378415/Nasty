using Nasty.Common.LoadParams;
using Nasty.Core.Repository;
using Nasty.Core.SuperExtension;
using Nasty.PortalModule.Areas.Permission.Model;
using Nasty.PortalModule.User;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Nasty.PortalModule.Permission
{
	public interface IPermissionRepository : IRepository<Permission>
	{
		public Permission GetPermission(string id);

		public ResultData<Permission> SavePermission(PermissionModel model);

		public ResultData<string> DeletePermissions(List<string> ids);

		public PermissionGroup GetPermissionGroup(string id);

		public List<PermissionGroup> GetPermissionGroups(GetPermissionGroupsParams @params);


        public ResultData<PermissionGroup> SavePermissionGroup(PermissionGroupModel model);

		public ResultData<string> DeletePermissionGroups(List<string> ids);

		public PageData<PermissionGroup> GetPermissionGroupPage(GetPermissionGroupPageParams @params);

        public PageData<Permission> GetPermissionPage(GetPermissionPageParams @params);

		public List<Permission> GetPermissions(GetPermissionsParams @params);
    }

	public class PermissionRepository : SqlRepository<Permission>, IPermissionRepository
	{
		public PermissionRepository(SqlSugarClient db) : base(db)
		{
		}

		public ResultData<string> DeletePermissions(List<string> ids)
		{
			var result = new ResultData<string>();
			try
			{
				Db.Delete<Permission>(ids);

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

		public Permission GetPermission(string id)
		{
			return this.Db.Queryable<Permission>().IncludesAllFirstLayer().InSingle(id);
		}

		public ResultData<Permission> SavePermission(PermissionModel model)
		{
			var result = new ResultData<Permission>();
			try
			{
				var data = Db.Save<Permission>(model);
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


		public ResultData<string> DeletePermissionGroups(List<string> ids)
		{
			var result = new ResultData<string>();
			try
			{
				Db.Delete<PermissionGroup>(ids);

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

		public PermissionGroup GetPermissionGroup(string id)
		{
			return this.Db.Queryable<PermissionGroup>().IncludesAllFirstLayer().InSingle(id);
		}

		public ResultData<PermissionGroup> SavePermissionGroup(PermissionGroupModel model)
		{
			var result = new ResultData<PermissionGroup>();
			try
			{
				var data = Db.Save<PermissionGroup>(model);
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

        public PageData<PermissionGroup> GetPermissionGroupPage(GetPermissionGroupPageParams @params)
        {
            int totalPage = 0;
            int total = 0;
            var pageData = new PageData<PermissionGroup>();

            var _SQLExpress = Db.Queryable<PermissionGroup>().IncludesAllFirstLayer();

            if (!string.IsNullOrEmpty(@params.Name)) _SQLExpress.Where((t) => t.Name.Contains(@params.Name));
            if (!string.IsNullOrEmpty(@params.Code)) _SQLExpress.Where((t) => t.Code.Contains(@params.Code));
            _SQLExpress = _SQLExpress.OrderBy((t) => t.CreateTime, OrderByType.Desc);

            var data = _SQLExpress.ToPageList(@params.Current, @params.PageSize, ref total, ref totalPage);

            pageData.Total = total;
            pageData.TotalPage = totalPage;
            pageData.Data = data;

            pageData.Current = @params.Current;
            pageData.PageSize = @params.PageSize;
            return pageData;
        }

        public PageData<Permission> GetPermissionPage(GetPermissionPageParams @params)
        {
            int totalPage = 0;
            int total = 0;
            var pageData = new PageData<Permission>();

            var _SQLExpress = Db.Queryable<Permission>().IncludesAllFirstLayer();

            if (!string.IsNullOrEmpty(@params.Name)) _SQLExpress.Where((t) => t.Name.Contains(@params.Name));
            if (!string.IsNullOrEmpty(@params.Code)) _SQLExpress.Where((t) => t.Code.Contains(@params.Code));
            _SQLExpress = _SQLExpress.OrderBy((t) => t.CreateTime, OrderByType.Desc);

            var data = _SQLExpress.ToPageList(@params.Current, @params.PageSize, ref total, ref totalPage);

            pageData.Total = total;
            pageData.TotalPage = totalPage;
            pageData.Data = data;

            pageData.Current = @params.Current;
            pageData.PageSize = @params.PageSize;
            return pageData;
        }

        public List<PermissionGroup> GetPermissionGroups(GetPermissionGroupsParams @params)
        {
			var _SQLExpress = Db.Queryable<PermissionGroup>();
            if (!string.IsNullOrEmpty(@params.Name)) _SQLExpress.Where((t) => t.Name.Contains(@params.Name));

			return _SQLExpress.ToList();
        }

        public List<Permission> GetPermissions(GetPermissionsParams @params)
        {
            var _SQLExpress = Db.Queryable<Permission>();
            if (!string.IsNullOrEmpty(@params.GroupId)) _SQLExpress.Where((t) => t.GroupId.Contains(@params.GroupId));

            return _SQLExpress.ToList();
        }
    }
}
