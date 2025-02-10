using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nasty.Common.LoadParams;
using Nasty.Core.Attributes;
using Nasty.PortalModule.Areas.User.Model;
using Nasty.PortalModule.User;

namespace Nasty.PortalModule.Areas.User
{
	[Area("Portal")]
	[Route("[Area]/[controller]/[action]"), ApiExplorerSettings(GroupName = "Portal")]
	[ApiController]
	[NastyAuthorize(AuthenticationSchemes = "Cookies")]
	public class UserController : ControllerBase
	{

		private readonly IUserService m_UserService;
		private readonly ILogger<UserController> _logger;

		public UserController(ILogger<UserController> logger, IUserService p_UserService)
		{
			_logger = logger;
			m_UserService = p_UserService;
		}

		/// <summary>
		/// ��ѯ�û�
		/// </summary>
		/// <param name="params"></param>
		/// <returns></returns>
		[HttpPost]

		public IActionResult GetUser([FromBody] SingleParams @params)
		{
			var data = m_UserService.GetUser(@params.Id);
			return Ok(data);
		}

		/// <summary>
		/// ��¼
		/// </summary>
		/// <param name="params"></param>
		/// <returns></returns>
		[HttpPost]
		[AllowAnonymous] // ������֤
		public IActionResult Login([FromBody] LoginParams @params)
		{
			var data = m_UserService.Login(@params);
			return Ok(data);
		}

		/// <summary>
		/// ��ʼ��SA�˺�
		/// </summary>
		/// <param name="params">������</param>
		/// <returns></returns>
		[HttpPost]
		[AllowAnonymous] // ������֤
		public IActionResult InitSA([FromBody] ArgParams @params)
		{
			var data = m_UserService.InitSA(@params.Arg);
			return Ok(data);
		}

		/// <summary>
		/// �����û���ɫ
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SaveUserRole([FromBody] SaveUserRoleModel model)
		{
			var data = m_UserService.SaveUserRole(model);
			return Ok(data);
		}
	}
}
