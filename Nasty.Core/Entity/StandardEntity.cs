using Nasty.Common.Session;
using SqlSugar;

namespace Nasty.Core.Entity
{
    public abstract class StandardEntity : BaseEntity
    {
        [SugarColumn(ColumnName = "CreateTime", IsOnlyIgnoreUpdate = true)]
        public DateTime? CreateTime { get; set; }

        [SugarColumn(ColumnName = "CreateUser", IsOnlyIgnoreUpdate = true)]
        public string? CreateUser { get; set; }

        [SugarColumn(ColumnName = "CreateUserId", IsOnlyIgnoreUpdate = true)]
        public string? CreateUserId { get; set; }

        [SugarColumn(ColumnName = "CreateDept", IsOnlyIgnoreUpdate = true)]
        public string? CreateDept { get; set; }

        [SugarColumn(ColumnName = "CreateDeptId", IsOnlyIgnoreUpdate = true)]
        public string? CreateDeptId { get; set; }


        [SugarColumn(ColumnName = "UpdateTime")]
        public DateTime? UpdateTime { get; set; }

        [SugarColumn(ColumnName = "UpdateUser")]
        public string? UpdateUser { get; set; }

        [SugarColumn(ColumnName = "UpdateUserId")]
        public string? UpdateUserId { get; set; }

        [SugarColumn(ColumnName = "UpdateDept")]
        public string? UpdateDept { get; set; }

        [SugarColumn(ColumnName = "UpdateDeptId")]
        public string? UpdateDeptId { get; set; }


        public override void OnPreAdd()
        {
            if (string.IsNullOrEmpty(this.Id)) this.Id = SnowFlakeSingle.Instance.NextId().ToString();
            this.CreateTime = DateTime.Now;
            var info = AppSession.CurrentUser.Value;
            if (info != null)
            {
                this.CreateUser = info.Name;
                this.CreateUserId = info.Id;
            }

            base.OnPreAdd();
        }

        public override void OnPreUpdate(bool isPre = true)
        {
            this.UpdateTime = DateTime.Now;
            var info = AppSession.CurrentUser.Value;
            if (info != null)
            {
                this.UpdateUser = info.Name;
                this.UpdateUserId = info.Id;
            }

            base.OnPreUpdate(isPre);
        }
    }
}
