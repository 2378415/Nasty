using Nasty.Core.Entity;
using Nasty.Core.SuperExtension;
using SqlSugar;

namespace Nasty.Core.Repository
{
	public abstract class SqlRepository<TRoot> : IRepository<TRoot> where TRoot : class, IBaseEntity, new()
	{
		public SqlSugarClient Db { get; set; }

		public SqlRepository(SqlSugarClient db)
		{
			this.Db = db;
		}

		public void Add(TRoot obj)
		{
			Db.Add(obj);
		}

		public void Delete(TRoot obj, bool isLogic)
		{
			Db.Delete(obj, isLogic);
		}

		public TRoot Find(string id)
		{
			return Db.Queryable<TRoot>().InSingle(id);
		}

		public void Update(TRoot obj)
		{
			Db.Update(obj);
		}
	}
}
