using Nasty.Common.Registry;
using Nasty.Core.Entity;

namespace Nasty.Core.Repository
{
	public interface IRepository<TRoot> : IAutofacRegister where TRoot : class, IBaseEntity
	{
		/// <summary>
		/// 将对象添加到仓储
		/// </summary>
		/// <param name="obj"></param>
		void Add(TRoot obj);

		/// <summary>
		/// 修改对象在仓储中的信息
		/// </summary>
		/// <param name="obj"></param>
		void Update(TRoot obj);

		/// <summary>
		/// 从仓储中删除对象
		/// </summary>
		/// <param name="obj"></param>
		void Delete(TRoot obj, bool isLogic);

		/// <summary>
		/// 根据编号查找对象
		/// </summary>
		/// <param name="id"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		TRoot Find(string id);
	}
}
