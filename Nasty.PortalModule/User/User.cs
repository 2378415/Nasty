using Nasty.Common.Session;
using Nasty.Core;
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
    public class User : StandardEntity<User>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "Name")]
        public string? Name { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [SugarColumn(ColumnName = "Avatar")]
        public string? Avatar { get; set; }

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
            var db = AppSession.CurrentDb.Value;

            var user = db.Queryable<User>().Where((t) => t.Account == this.Account).First();
            if (user != null) throw new Exception("用户账号重复");

            if (string.IsNullOrEmpty(this.Password)) throw new Exception("密码不能为空");
            this.Password = Tools.Md5(this.Password ?? string.Empty);
            base.OnPreAdd();
        }


        public override void OnPreDelete()
        {
            base.OnPreDelete();

            var db = AppSession.CurrentDb.Value;

            {
                db.Deleteable<UserRole>().Where((t) => t.UserId == this.Id).ExecuteCommand();
            }
        }
    }
}
