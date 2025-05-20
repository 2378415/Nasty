using Nasty.Common.Session;
using Nasty.Core.Entity;
using Nasty.PortalModule.User;
using SqlSugar;

namespace Nasty.PortalModule.Permission
{
    [SugarTable("SysPermissionGroup")]
    public class PermissionGroup : StandardEntity<PermissionGroup>
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
        /// 权限
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(Permission.GroupId))]
        public List<Permission>? Permissions { get; set; }


        public override void OnPreAdd()
        {
            base.OnPreAdd();

            var db = AppSession.CurrentDb.Value;
            var isAny = db.Queryable<PermissionGroup>().Where((t) => t.Code == this.Code).Any();
            if (isAny) throw new Exception("编码重复");

            db.Deleteable<Permission>((t) => t.GroupId == this.Id).ExecuteCommand();
        }
    }
}
