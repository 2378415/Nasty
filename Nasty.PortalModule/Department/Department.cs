using Nasty.Common.Session;
using Nasty.Core.Entity;
using Nasty.PortalModule.User;
using SqlSugar;

namespace Nasty.PortalModule.Department
{
    [SugarTable("SysDepartment")]
    public class Department : StandardEntity
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
        /// 父级部门Id
        /// </summary>
        [SugarColumn(ColumnName = "ParentId")]
        public string? ParentId{ get; set; }

        /// <summary>
        /// 部门角色Id
        /// </summary>
        [SugarColumn(ColumnName = "RoleId")]
        public string? RoleId { get; set; }

        /// <summary>
        /// 部门角色
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(Department.RoleId), nameof(Department.Role.Id))]
        public Role.Role? Role { get; set; }

        /// <summary>
        /// 部门用户
        /// </summary>
        [Navigate(typeof(DepartmentUser), nameof(DepartmentUser.DepartmentId), nameof(DepartmentUser.UserId))]
        public List<User.User>? Users { get; set; }


        public override void OnPreAdd()
        {
            base.OnPreAdd();
            var db = AppSession.CurrentDb.Value;

            try
            {
                var role = new Role.Role()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Name,
                    Code = Code,
                };

                db.Insertable(role).ExecuteCommand();

                this.RoleId = role.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("创建部门角色失败", ex);
            }
        }


        public override void OnPreDelete()
        {
            base.OnPreDelete();

            var db = AppSession.CurrentDb.Value;
            db.Deleteable<Role.Role>().Where((t) => t.Id == this.RoleId).ExecuteCommand();
        }
    }
}
