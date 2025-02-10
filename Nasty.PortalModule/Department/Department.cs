using Nasty.Common.Session;
using Nasty.Core.Entity;
using Nasty.PortalModule.User;
using SqlSugar;

namespace Nasty.PortalModule.Department
{
	[SugarTable("SysDepartment")]
	public class Department : BaseEntity
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

			var role = new Role.Role()
			{
				Id = Guid.NewGuid().ToString(),
				Name = Name,
			};

			if (AppSession.CurrentDb.Value != null) AppSession.CurrentDb.Value.Insertable(role).ExecuteCommand();
		}


		public override void OnPreDelete()
		{
			base.OnPreDelete();

			//var db = AppSession.CurrentDB.Value;
			//db.Deleteable<UserRole>().Where((t) => t.UserId == this.Id).ExecuteCommand();
		}
	}
}
