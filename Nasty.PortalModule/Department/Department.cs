using Nasty.Common.Session;
using Nasty.Core.Entity;
using Nasty.PortalModule.Permission;
using Nasty.PortalModule.User;
using SqlSugar;

namespace Nasty.PortalModule.Department
{
    [SugarTable("SysDepartment")]
    public class Department : StandardEntity<Department>
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
        public string? ParentId { get; set; }

        /// <summary>
        /// 部门角色Id
        /// </summary>
        [SugarColumn(ColumnName = "RoleId")]
        public string? RoleId { get; set; }

        /// <summary>
        /// 是否末级部门
        /// </summary>
        [SugarColumn(ColumnName = "IsLeaf")]
        public bool? IsLeaf { get; set; }

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
                {
                    var isAny = db.Queryable<Department>().Where((t) => t.Code == this.Code).Any();
                    if (isAny) throw new Exception("编码重复");
                }

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

                {

                    if (!string.IsNullOrEmpty(this.ParentId))
                    {
                        db.Updateable<Department>()
                         .SetColumns((t) => new Department() { IsLeaf = false })
                         .Where((t) => t.Id == this.ParentId)
                         .ExecuteCommand();
                    }
                }


                this.IsLeaf = true;
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

            {
                if (!string.IsNullOrEmpty(this.ParentId))
                {
                    //父级部门是否存在其他子级部门并且子级部门不等于当前部门
                    var isAny = db.Queryable<Department>().Where((t) => t.ParentId == t.ParentId && t.Id != t.Id).Any();
                    if (!isAny)
                    {
                        db.Updateable<Department>()
                             .SetColumns((t) => new Department() { IsLeaf = true })
                             .Where((t) => t.Id == this.ParentId)
                             .ExecuteCommand();
                    }
                }
            }
        }
    }
}
