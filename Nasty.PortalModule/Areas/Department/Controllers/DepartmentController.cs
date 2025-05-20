using Microsoft.AspNetCore.Mvc;
using Nasty.Common.LoadParams;
using Nasty.Core.Attributes;
using Nasty.PortalModule.Areas.Department.Model;
using Nasty.PortalModule.Department;

namespace Nasty.PortalModule.Areas.Department.Controllers
{
    [Area("Portal")]
    [Route("[Area]/[controller]/[action]"), ApiExplorerSettings(GroupName = "Portal")]
    [ApiController]
    [NastyAuthorize(AuthenticationSchemes = "Bearer,Cookies")]
    public class DepartmentController : ControllerBase
    {

        private readonly IDepartmentService m_DepartmentService;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(ILogger<DepartmentController> logger, IDepartmentService p_DepartmentService)
        {
            _logger = logger;
            m_DepartmentService = p_DepartmentService;
        }

        /// <summary>
        /// 查询部门
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetDepartment([FromBody] SingleParams @params)
        {
            var data = m_DepartmentService.GetDepartment(@params.Id);
            return Ok(data);
        }

        /// <summary>
        /// 批量查询部门
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetDepartments([FromBody] GetDepartmentsParams @params)
        {
            var data = m_DepartmentService.GetDepartments(@params);
            return Ok(data);
        }

        /// <summary>
        /// 保存部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveDepartment([FromBody] DepartmentModel model)
        {
            var data = m_DepartmentService.SaveDepartment(model);
            return Ok(data);
        }

        /// <summary>
        /// 批量删除部门
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteDepartments([FromBody] BatchParams<string> @params)
        {
            var data = m_DepartmentService.DeleteDepartments(@params.Items);
            return Ok(data);
        }


        /// <summary>
        /// 分页查询部门
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetDepartmentPage([FromBody] GetDepartmentPageParams @params)
        {
            var data = m_DepartmentService.GetDepartmentPage(@params);
            return Ok(data);
        }
    }
}
