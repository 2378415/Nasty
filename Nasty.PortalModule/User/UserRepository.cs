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

		public ResultData<string> InitSA(string password);

		public User GetUserByAccount(string account);

		public User GetUserByAccount(string account, string password);

		public ResultData<string> SaveUserRole(SaveUserRoleModel model);

		public User GetDeepUser(string id);
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
			return this.Db.Queryable<User>().IncludesAllFirstLayer().Includes((t) => t.Roles, (s) => s.Permissions).InSingle(id);
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
					Password = Tools.Md5(password),
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
	}
}
