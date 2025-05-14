using Microsoft.AspNetCore.Mvc;
using Nasty.Common.Authorization;
using Nasty.Common.LoadParams;
using Nasty.Common.Model;
using Nasty.Common.Registry;
using Nasty.Common.Session;
using Nasty.PortalModule.Areas.User.Model;
using System.Security.Principal;

namespace Nasty.PortalModule.User
{
    public interface IUserService : IAutofacRegister
    {
        public User GetUser(string id);

        public ResultData<User> SaveUser(UserModel model);

        public ResultData<UserInfoModel> Login(LoginParams @params);

        public ResultData<UserInfoModel> GetCurrentUserInfo();

        public PageData<User> GetUserPage(GetUserPageParams @params);

        public ResultData<string> InitSA(string password);

        public ResultData<string> SaveUserRole(SaveUserRoleModel model);

        public ResultData<string> DeleteUser(string id);
    }

    public class UserService : IUserService
    {
        public required IUserRepository UserRepository { get; set; }


        public ResultData<string> DeleteUser(string id)
        {
            return UserRepository.DeleteUser(id);
        }

        public User GetUser(string id)
        {
            return UserRepository.GetUser(id);
        }

        public ResultData<User> SaveUser(UserModel model)
        {
            return UserRepository.SaveUser(model);
        }

        public ResultData<UserInfoModel> GetCurrentUserInfo()
        {
            var result = new ResultData<UserInfoModel>();
            var info = AppSession.CurrentUser.Value;
            result.Data = info;
            result.IsSuccess = true;
            return result;
        }

        public ResultData<UserInfoModel> Login(LoginParams @params)
        {
            var result = new ResultData<UserInfoModel>();
            var user = UserRepository.GetUserByAccount(@params.Account, Core.Tools.Md5(@params.Password));
            if (user == null)
            {
                result.IsSuccess = false;
                return result;
            }

            var info = MakeUserInfo(user.Id);
            result.Data = info;
            result.IsSuccess = true;
            return result;
        }

        private UserInfoModel MakeUserInfo(string id)
        {
            var user = UserRepository.GetDeepUser(id);
            var roles = ((user.Roles?.Select(x => x.Code).ToArray()) as string[]) ?? Array.Empty<string>();
            var permissions = (user.Roles?.Where(x => x.Permissions != null).Select(x => x.Code).ToArray() as string[]) ?? Array.Empty<string>();


            var info = new UserInfoModel();
            info.Id = user.Id;
            info.Token = Auth.MakeToken(user.Account, roles, permissions, 3);
            info.Account = user.Account;
            info.Name = user.Name;
            info.Avatar = user.Avatar;
            info.Roles = roles;
            info.Permissions = permissions;


            if (!string.IsNullOrEmpty(user.Account))
            {
                SessionManager.SetUserInfo(user.Account, info);
                Auth.MakeCookie(user.Account, roles, permissions, 3);
            }

            return info;
        }

        public ResultData<string> InitSA(string password)
        {
            return UserRepository.InitSA(password);
        }

        public ResultData<string> SaveUserRole(SaveUserRoleModel model)
        {
            return UserRepository.SaveUserRole(model);
        }

        public PageData<User> GetUserPage(GetUserPageParams @params)
        {
            return UserRepository.GetUserPage(@params);
        }
    }
}
