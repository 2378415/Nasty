using Nasty.Common.Session;
using Nasty.Core.Entity;
using Nasty.PortalModule.Department;
using Nasty.PortalModule.User;
using SqlSugar;

namespace Nasty.PortalModule.Role
{
    [SugarTable("SysRole")]
    public class Role : StandardEntity<Role>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "Name")]
        public string? Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [SugarColumn(ColumnName = "Code")]
        public string? Code { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "Type")]
        public RoleType? Type { get; set; }

        /// <summary>
        /// 用户权限
        /// </summary>
        [Navigate(typeof(RolePermission), nameof(RolePermission.RoleId), nameof(RolePermission.PermissionId))]
        public List<Permission.Permission>? Permissions { get; set; }


        public override void OnPreAdd()
        {
            base.OnPreAdd();
            if(this.Type == null) this.Type = RoleType.Normal;
        }

        public override void OnPreDelete()
        {
            base.OnPreDelete();

            var db = AppSession.CurrentDb.Value;

            {
                if (this.Type == RoleType.System) throw new Exception("系统角色无法删除");
            }

            {
                db.Deleteable<RolePermission>().Where((t) => t.RoleId == this.Id).ExecuteCommand();
            }
        }

    }


    public enum RoleType : byte
    {
        /// <summary>
        /// 系统角色
        /// </summary>
        System = 1,
        /// <summary>
        /// 部门角色
        /// </summary>
        Department = 2,
        /// <summary>
        /// 普通角色
        /// </summary>
        Normal = 3,
    }
}
