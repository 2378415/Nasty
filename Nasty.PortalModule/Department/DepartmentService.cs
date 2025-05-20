using Nasty.Common.LoadParams;
using Nasty.Common.Registry;
using Nasty.PortalModule.Areas.Department.Model;
using Nasty.PortalModule.Department;

namespace Nasty.PortalModule.Department
{
    public interface IDepartmentService : IAutofacRegister
    {
        public Department GetDepartment(string id);

        public List<Department> GetDepartments(GetDepartmentsParams @params);

        public ResultData<Department> SaveDepartment(DepartmentModel model);

        public ResultData<string> DeleteDepartments(List<string> ids);

        public PageData<Department> GetDepartmentPage(GetDepartmentPageParams @params);
    }

    public class DepartmentService : IDepartmentService
    {
        public required IDepartmentRepository DepartmentRepository { get; set; }

        public ResultData<string> DeleteDepartments(List<string> ids)
        {
            return DepartmentRepository.DeleteDepartments(ids);
        }

        public Department GetDepartment(string id)
        {
            return DepartmentRepository.GetDepartment(id);
        }

      
        public PageData<Department> GetDepartmentPage(GetDepartmentPageParams @params)
        {
            return DepartmentRepository.GetDepartmentPage(@params);
        }

        public ResultData<Department> SaveDepartment(DepartmentModel model)
        {
            return DepartmentRepository.SaveDepartment(model);
        }
        public List<Department> GetDepartments(GetDepartmentsParams @params)
        {
            return DepartmentRepository.GetDepartments(@params);
        }
    }
}
