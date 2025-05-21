using Nasty.Common.LoadParams;
using Nasty.Core;
using Nasty.Core.Repository;
using Nasty.Core.SuperExtension;
using Nasty.PortalModule.Areas.User.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Nasty.PortalModule.User
{
    public interface IUserRepository : IRepository<User>
    {
        public User GetUser(string id);

        public ResultData<User> SaveUser(UserModel model);

        public ResultData<string> InitSA(string password);

        public User GetUserByAccount(string account);

        public User GetUserByAccount(string account, string password);

        public ResultData<string> SaveUserRole(SaveUserRoleModel model);

        public User GetDeepUser(string id);

        public PageData<User> GetUserPage(GetUserPageParams @params);

        public ResultData<string> DeleteUser(string id);
    }

    public class UserRepository : SqlRepository<User>, IUserRepository
    {
        public UserRepository(SqlSugarClient db) : base(db)
        {
            //db.CodeFirst.InitTables(typeof(User));
            //db.CodeFirst.InitTables(typeof(UserRole));
            //db.CodeFirst.InitTables(typeof(Role.Role));
            //db.CodeFirst.InitTables(typeof(RolePermission));
            //db.CodeFirst.InitTables(typeof(Permission.Permission));
            //db.CodeFirst.InitTables(typeof(Permission.PermissionGroup));
        }

        public User GetUser(string id)
        {
            return this.Db.Queryable<User>().IncludesAllFirstLayer().InSingle(id);
        }

        public User GetUserByAccount(string account)
        {
            return this.Db.Queryable<User>().Where(it => it.Account == account).IncludesAllFirstLayer().First();
        }

        public User GetUserByAccount(string account, string password)
        {
            return this.Db.Queryable<User>().Where(it => it.Account == account && it.Password == password).First();
        }

        public User GetDeepUser(string id)
        {
            return this.Db.Queryable<User>().IncludesAllFirstLayer()
                .Includes((t) => t.Roles, (s) => s.Permissions)
                .Includes((t) => t.Departments, (s) => s.Role, (r) => r.Permissions)
                .InSingle(id);
        }

        public ResultData<string> InitSA(string password)
        {
            var result = new ResultData<string>();
            try
            {
                var isAny = Db.Queryable<User>().Where((t) => t.Account == "sa").Any();
                if (isAny)
                {
                    result.IsSuccess = false;
                    result.Message = "sa账号已存在!";
                    return result;
                }


                var user = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "超级管理员",
                    Account = "sa",
                    Password = password,
                };

                this.Add(user);
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

        public ResultData<string> SaveUserRole(SaveUserRoleModel model)
        {
            var result = new ResultData<string>();
            try
            {

                var user = Db.Queryable<User>().InSingle(model.UserId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Message = "账号不存在!";
                    return result;
                }

                if (model.RoleIds.Count > 0)
                {
                    var roles = Db.Queryable<Role.Role>().In(model.RoleIds).ToList();
                    user.Roles = roles;
                    Db.UpdateNav(user).Include((t) => t.Roles).ExecuteCommand();
                }

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

        PageData<User> IUserRepository.GetUserPage(GetUserPageParams @params)
        {
            int totalPage = 0;
            int total = 0;
            var pageData = new PageData<User>();

            var _SQLExpress = Db.Queryable<User>().IncludesAllFirstLayer();

            if (!string.IsNullOrEmpty(@params.Name)) _SQLExpress.Where((t) => t.Name.Contains(@params.Name));

            _SQLExpress = _SQLExpress.OrderBy((t) => t.CreateTime, OrderByType.Desc);

            var data = _SQLExpress.ToPageList(@params.Current, @params.PageSize, ref total, ref totalPage);

            pageData.Total = total;
            pageData.TotalPage = totalPage;
            pageData.Data = data;

            pageData.Current = @params.Current;
            pageData.PageSize = @params.PageSize;
            return pageData;
        }

        public ResultData<User> SaveUser(UserModel model)
        {
            var result = new ResultData<User>();
            try
            {
                var user = Db.Save<User>(model);
                result.IsSuccess = true;
                result.Data = user;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public ResultData<string> DeleteUser(string id)
        {
            var result = new ResultData<string>();
            try
            {
                Db.Delete<User>(id);
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
    }
}
