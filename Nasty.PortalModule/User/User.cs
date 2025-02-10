using Nasty.Core.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nasty.PortalModule.User
{
	[SugarTable("SysUser")]
	public class User : BaseEntity
	{
		/// <summary>
		/// 名称
		/// </summary>
		[SugarColumn(ColumnName = "Name")]
		public string? Name { get; set; }

		/// <summary>
		/// 账号
		/// </summary>
		[SugarColumn(ColumnName = "Account")]
		public string? Account { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		[SugarColumn(ColumnName = "Password")]
		public string? Password { get; set; }

		/// <summary>
		/// 用户角色
		/// </summary>
		[Navigate(typeof(UserRole), nameof(UserRole.UserId), nameof(UserRole.RoleId))]
		public List<Role.Role>? Roles { get; set; }


		public override void OnPreAdd()
		{
			base.OnPreAdd();
			
		}


		public override void OnPreDelete()
		{
			base.OnPreDelete();

			//var db = AppSession.CurrentDB.Value;
			//db.Deleteable<UserRole>().Where((t) => t.UserId == this.Id).ExecuteCommand();
		}
	}
}
