using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nasty.Common.Security;
using Nasty.Common.LoadParams;
using Nasty.Core.Attributes;
using Nasty.PortalModule.Areas.User.Model;
using Nasty.PortalModule.User;
using Nasty.Common.Attributes;

namespace Nasty.PortalModule.Areas.User
{
    [Area("Portal")]
    [Route("[Area]/[controller]/[action]"), ApiExplorerSettings(GroupName = "Portal")]
    [ApiController]
    [NastyAuthorize(AuthenticationSchemes = "Bearer,Cookies")]
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
        /// 查询用户
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetUser([FromBody] SingleParams @params)
        {
            //            {
            //                "Arg": "5fFmS1xnEX4BPe7Mnou3Eg=="
            //}
            var data = m_UserService.GetUser(@params.Id);

            //var result = new ResultData<Nasty.PortalModule.User.User>()
            //{
            //    Data = new Nasty.PortalModule.User.User()
            //    {
            //        Id = @params.Id,
            //    },
            //    IsSuccess = true
            //};
            return Ok(data);
        }


        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveUser([FromBody] UserModel @params)
        {
            var data = m_UserService.SaveUser(@params);
            return Ok(data);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteUser([FromBody] SingleParams @params)
        {
            var data = m_UserService.DeleteUser(@params.Id);
            return Ok(data);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous] // 忽略验证
        public IActionResult Login([FromBody] LoginParams @params)
        {
            var data = m_UserService.Login(@params);
            return Ok(data);
        }

        [HttpPost]
        public IActionResult GetCurrentUserInfo()
        {
            var data = m_UserService.GetCurrentUserInfo();
            return Ok(data);
        }

        [HttpPost]
        public IActionResult GetUserPage([FromBody] GetUserPageParams @params)
        {
            var data = m_UserService.GetUserPage(@params);
            return Ok(data);
        }

        /// <summary>
        /// 初始化SA账号
        /// </summary>
        /// <param name="params">传密码</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous] // 忽略验证
        public IActionResult InitSA([FromBody] ArgParams @params)
        {
            var data = m_UserService.InitSA(@params.Arg);
            return Ok(data);
        }

        /// <summary>
        /// 保存用户角色
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
