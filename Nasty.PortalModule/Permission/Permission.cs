﻿using Nasty.Common.Session;
using Nasty.Core.Entity;
using Nasty.PortalModule.User;
using SqlSugar;

namespace Nasty.PortalModule.Permission
{
	[SugarTable("SysPermission")]
	public class Permission : StandardEntity<Permission>
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
		/// 组Id
		/// </summary>
		[SugarColumn(ColumnName = "GroupId")]
		public string? GroupId { get; set; }

        /// <summary>
        /// 所属分组
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(Permission.GroupId))]
        public PermissionGroup? Group { get; set; }

        public override void OnPreAdd()
		{
			base.OnPreAdd();

            var db = AppSession.CurrentDb.Value;
            var isAny = db.Queryable<PermissionGroup>().Where((t) => t.Code == this.Code).Any();
            if (isAny) throw new Exception("编码重复");
        }
	}
}
