using Microsoft.AspNetCore.Mvc;
using Nasty.Common.Authorization;
using Nasty.Common.LoadParams;
using Nasty.Common.Registry;
using Nasty.Common.Session;
using Nasty.PortalModule.Areas.User.Model;
using System.Security.Principal;

namespace Nasty.PortalModule.User
{
	public interface IUserService : IAutofacRegister
	{
		public User GetUser(string id);

		public ResultData<UserInfo> Login(LoginParams @params);

		public ResultData<string> InitSA(string password);

		public ResultData<string> SaveUserRole(SaveUserRoleModel model);
	}

	public class UserService : IUserService
	{
		public required IUserRepository UserRepository { get; set; }

		public User GetUser(string id)
		{
			return UserRepository.GetUser(id);
		}

		public ResultData<UserInfo> Login(LoginParams @params)
		{
			var result = new ResultData<UserInfo>();
			var user = UserRepository.GetUserByAccount(@params.Account, Core.Tools.Md5(@params.Password));
			if (user == null)
			{
				result.IsSuccess = false;
				return result;
			}

			user = UserRepository.GetDeepUser(user.Id ?? string.Empty);
			var roles = ((user.Roles?.Select(x => x.Code).ToArray()) as string[]) ?? Array.Empty<string>();
			var permissions = (user.Roles?.Where(x => x.Permissions != null).Select(x => x.Code).ToArray() as string[]) ?? Array.Empty<string>();


			var info = new UserInfo();
			info.Id = user.Id;
			info.Token = Auth.MakeToken(@params.Account, roles, permissions, 3);
			info.Account = user.Account;
			info.Name = user.Name;
			info.Roles = roles;
			info.Permissions = permissions;

			result.Data = info;
			result.IsSuccess = true;

			if (result.IsSuccess && !string.IsNullOrEmpty(user.Account))
			{
				SessionManager.SetUserInfo(user.Account, info);
				Auth.MakeCookie(@params.Account, roles, permissions, 3);
			}

			return result;
		}

		public ResultData<string> InitSA(string password)
		{
			return UserRepository.InitSA(password);
		}

		public ResultData<string> SaveUserRole(SaveUserRoleModel model)
		{
			return UserRepository.SaveUserRole(model);
		}
	}
}
