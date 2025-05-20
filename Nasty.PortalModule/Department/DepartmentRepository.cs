using Nasty.Common.LoadParams;
using Nasty.Core.Repository;
using Nasty.Core.SuperExtension;
using Nasty.PortalModule.Areas.Department.Model;
using Nasty.PortalModule.Department;
using SqlSugar;

namespace Nasty.PortalModule.Department
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        public Department GetDepartment(string id);

        public ResultData<Department> SaveDepartment(DepartmentModel model);

        public ResultData<string> DeleteDepartments(List<string> ids);

        public PageData<Department> GetDepartmentPage(GetDepartmentPageParams @params);

        public List<Department> GetDepartments(GetDepartmentsParams @params);
    }

    public class DepartmentRepository : SqlRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(SqlSugarClient db) : base(db)
        {
            //db.CodeFirst.InitTables(typeof(Department));
            //db.CodeFirst.InitTables(typeof(DepartmentUser));
        }

        public ResultData<string> DeleteDepartments(List<string> ids)
        {
            var result = new ResultData<string>();
            try
            {
                Db.Delete<Department>(ids);

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

        public Department GetDepartment(string id)
        {
            return this.Db.Queryable<Department>().InSingle(id);
        }

        public ResultData<Department> SaveDepartment(DepartmentModel model)
        {
            var result = new ResultData<Department>();
            try
            {
                var data = Db.Save<Department>(model);
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

        public PageData<Department> GetDepartmentPage(GetDepartmentPageParams @params)
        {
            int totalPage = 0;
            int total = 0;
            var pageData = new PageData<Department>();

            var _SQLExpress = Db.Queryable<Department>().IncludesAllFirstLayer();

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


        public List<Department> GetDepartments(GetDepartmentsParams @params)
        {
            var _SQLExpress = Db.Queryable<Department>();
            if (!string.IsNullOrEmpty(@params.ParentId))
                _SQLExpress.Where((t) => t.ParentId == @params.ParentId);
            else
                _SQLExpress.Where((t) => string.IsNullOrEmpty(t.ParentId));
            
            return _SQLExpress.ToList();
        }
    }
}
