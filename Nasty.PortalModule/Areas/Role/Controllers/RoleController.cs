using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nasty.Common.LoadParams;
using Nasty.Core.Attributes;
using Nasty.PortalModule.Areas.Permission.Model;
using Nasty.PortalModule.Areas.Role.Model;
using Nasty.PortalModule.Areas.User.Model;
using Nasty.PortalModule.Role;

namespace Nasty.PortalModule.Areas.Role
{
	[Area("Portal")]
	[Route("[Area]/[controller]/[action]"), ApiExplorerSettings(GroupName = "Portal")]
	[ApiController]
    [NastyAuthorize(AuthenticationSchemes = "Bearer,Cookies")]
    public class RoleController : ControllerBase
	{

		private readonly IRoleService m_RoleService;
		private readonly ILogger<RoleController> _logger;

		public RoleController(ILogger<RoleController> logger, IRoleService p_RoleService)
		{
			_logger = logger;
			m_RoleService = p_RoleService;
		}

		/// <summary>
		/// ��ѯ��ɫ
		/// </summary>
		/// <param name="params"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult GetRole([FromBody] SingleParams @params)
		{
			var data = m_RoleService.GetRole(@params.Id);
			return Ok(data);
		}


		/// <summary>
		/// �����ɫ
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SaveRole([FromBody] RoleModel model)
		{
			var data = m_RoleService.SaveRole(model);
			return Ok(data);
		}

		/// <summary>
		/// ����ɾ����ɫ
		/// </summary>
		/// <param name="params"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult DeleteRoles([FromBody] BatchParams<string> @params)
		{
			var data = m_RoleService.DeleteRoles(@params.Items);
			return Ok(data);
		}


        /// <summary>
        /// ��ҳ��ѯ��ɫ
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetRolePage([FromBody] GetRolePageParams @params)
        {
            var data = m_RoleService.GetRolePage(@params);
            return Ok(data);
        }

        /// <summary>
        /// �����û���ɫ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveRolePermission([FromBody] SaveRolePermissionModel model)
        {
            var data = m_RoleService.SaveRolePermission(model);
            return Ok(data);
        }
    }
}
