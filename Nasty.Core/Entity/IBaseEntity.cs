namespace Nasty.Core.Entity
{
	/// <summary>
	/// 实体类基类
	/// </summary>
	public interface IBaseEntity
	{
		public string? Id { get; set; }

		void OnPreAdd();

		void OnPreUpdate(bool isPre = true);

		void OnPreDelete();

		void OnAdded();

		void OnUpdated();

		void OnDeleted();

		public (string Table, Dictionary<string, object> Fields) GetUpdateFields();

		public string GetTableName();

		public void SetPropertyValue(string propertyName, object value);

		public object? GetPropertyValue(string propertyName);

		public IBaseEntity? Merge<T2>(T2 data) where T2 : class;
	}
}
