using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nasty.Common.LoadParams;
using Nasty.Core.Attributes;
using Nasty.PortalModule.Areas.Permission.Model;
using Nasty.PortalModule.Permission;

namespace Nasty.PortalModule.Areas.Permission
{
	[Area("Portal")]
	[Route("[Area]/[controller]/[action]"), ApiExplorerSettings(GroupName = "Portal")]
	[ApiController]
    [NastyAuthorize(AuthenticationSchemes = "Cookies")]
    public class PermissionController : ControllerBase
	{

		private readonly IPermissionService m_PermissionService;
		private readonly ILogger<PermissionController> _logger;

		public PermissionController(ILogger<PermissionController> logger, IPermissionService p_PermissionService)
		{
			_logger = logger;
			m_PermissionService = p_PermissionService;
		}

		/// <summary>
		/// 查询权限
		/// </summary>
		/// <param name="params"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult GetPermission([FromBody] SingleParams @params)
		{
			var data = m_PermissionService.GetPermission(@params.Id);
			return Ok(data);
		}


		/// <summary>
		/// 保存权限
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SavePermission([FromBody] PermissionModel model)
		{
			var data = m_PermissionService.SavePermission(model);
			return Ok(data);
		}

		/// <summary>
		/// 批量删除权限
		/// </summary>
		/// <param name="params"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult DeletePermissions([FromBody] BatchParams @params)
		{
			var data = m_PermissionService.DeletePermissions(@params.Ids);
			return Ok(data);
		}



		/// <summary>
		/// 查询权限分组
		/// </summary>
		/// <param name="params"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult GetPermissionGroup([FromBody] SingleParams @params)
		{
			var data = m_PermissionService.GetPermissionGroup(@params.Id);
			return Ok(data);
		}


		/// <summary>
		/// 保存权限分组
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SavePermissionGroup([FromBody] PermissionGroupModel model)
		{
			var data = m_PermissionService.SavePermissionGroup(model);
			return Ok(data);
		}

		/// <summary>
		/// 批量删除权限分组
		/// </summary>
		/// <param name="params"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult DeletePermissionGroups([FromBody] BatchParams @params)
		{
			var data = m_PermissionService.DeletePermissionGroups(@params.Ids);
			return Ok(data);
		}
	}
}
