
using Nasty.Core.Entity;
using SqlSugar;
using System.Linq.Expressions;
using System;
using Nasty.Common.Model;

namespace Nasty.Core.SuperExtension
{
	public static class SqlSugarExtension
	{

		/// <summary>
		/// 创建，调用该方法尽量开启事务，因为Pre方法可能会进行仓储操作
		/// </summary>
		/// <param name="client"></param>
		/// <param name="entity"></param>
		public static void Add<T>(this SqlSugarClient client, T entity) where T : class, IBaseEntity, new()
		{
			entity.OnPreAdd();

			client.Insertable(entity).ExecuteCommand();
			//client.UpdateNav(entity).IncludesAllFirstLayer().ExecuteCommand();

			entity.OnAdded();
		}

		/// <summary>
		/// 批量创建，调用该方法尽量开启事务
		/// </summary>
		/// <param name="client"></param>
		/// <param name="entitys"></param>
		public static void Add<T>(this SqlSugarClient client, IEnumerable<T> entitys) where T : class, IBaseEntity, new()
		{
			foreach (var entity in entitys)
			{
				client.Add(entity);
			}
		}


		/// <summary>
		/// 批量创建，调用该方法尽量开启事务
		/// </summary>
		/// <param name="client"></param>
		/// <param name="entitys"></param>
		public static void Add<T>(this SqlSugarClient client, IEnumerable<ISaveModel> models) where T : class, IBaseEntity, new()
		{
			foreach (var model in models)
			{
				T? entity = null;
				entity = BaseEntity.Merge(entity, model);
				if (entity != null) client.Add(entity);
			}
		}

		/// <summary>
		/// 修改，调用该方法尽量开启事务，因为Pre方法可能会进行仓储操作
		/// </summary>
		/// <param name="client"></param>
		/// <param name="entity"></param>
		/// <param name="isPre"></param>
		public static void Update<T>(this SqlSugarClient client, T entity, bool isPre = true) where T : class, IBaseEntity, new()
		{
			//触发修改事件
			entity.OnPreUpdate(isPre);
			//获取需要更新的数据，空的数据不更新，避免覆盖
			var data = entity.GetUpdateFields();
			//进行更新
			client.Updateable(data.Fields).AS(data.Table).WhereColumns("id").ExecuteCommand();

			entity.OnUpdated();
		}


		/// <summary>
		/// 批量修改，调用该方法尽量开启事务
		/// </summary>
		/// <param name="client"></param>
		/// <param name="entitys"></param>
		public static void Update<T>(this SqlSugarClient client, IEnumerable<T> entitys) where T : class, IBaseEntity, new()
		{
			foreach (var entity in entitys)
			{
				client.Update(entity);
			}
		}

		/// <summary>
		/// 修改，调用该方法尽量开启事务，因为Pre方法可能会进行仓储操作
		/// </summary>
		/// <param name="client"></param>
		/// <param name="entity"></param>
		/// <param name="isPre"></param>
		public static void Update<T>(this SqlSugarClient client, IEnumerable<ISaveModel> models) where T : class, IBaseEntity, new()
		{
			foreach (var model in models)
			{
				var entity = client.Queryable<T>().InSingle(model.Id);
				if (entity != null)
				{
					entity = entity.Merge(model) as T;
					if (entity != null) client.Update(entity);
				}
			}
		}

		/// <summary>
		/// 删除，调用该方法尽量开启事务，因为Pre方法可能会进行仓储操作
		/// </summary>
		/// <param name="client"></param>
		/// <param name="entity"></param>
		/// <param name="isLogic"></param>
		public static void Delete<T>(this SqlSugarClient client, T entity, bool isLogic = false) where T : class, IBaseEntity, new()
		{
			entity.OnPreDelete();

			if (isLogic)
			{
				//逻辑删除不触发Pre事件
				client.Update(entity, false);
			}
			else
			{
				client.DeleteableByObject(entity).ExecuteCommand();
			}

			entity.OnDeleted();
		}

		/// <summary>
		/// 删除，调用该方法尽量开启事务，因为Pre方法可能会进行仓储操作
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <param name="isLogic"></param>
		public static void Delete<T>(this SqlSugarClient client, string id, bool isLogic = false) where T : class, IBaseEntity, new()
		{
			var entity = client.Query<T>(id);
			client.Delete(entity, isLogic);
		}

		/// <summary>
		/// 批量删除，调用该方法尽量开启事务
		/// </summary>
		/// <param name="client"></param>
		/// <param name="entitys"></param>
		/// <param name="isLogic"></param>
		public static void Delete<T>(this SqlSugarClient client, IEnumerable<T> entitys, bool isLogic = false) where T : class, IBaseEntity, new()
		{
			foreach (var entity in entitys)
			{
				client.Delete(entity, isLogic);
			}
		}

		/// <summary>
		/// 批量删除，调用该方法尽量开启事务
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="client"></param>
		/// <param name="ids"></param>
		/// <param name="isLogic"></param>
		public static void Delete<T>(this SqlSugarClient client, IEnumerable<string> ids, bool isLogic = false) where T : class, IBaseEntity, new()
		{
			var entitys = client.Query<T>(ids);
			foreach (var entity in entitys)
			{
				client.Delete(entity, isLogic);
			}
		}


		/// <summary>
		/// 查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static T Query<T>(this SqlSugarClient client, string id) where T : class, IBaseEntity, new()
		{
			return client.Queryable<T>().InSingle(id);
		}

		/// <summary>
		/// 批量查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="client"></param>
		/// <param name="ids"></param>
		/// <returns></returns>

		public static IEnumerable<T> Query<T>(this SqlSugarClient client, IEnumerable<string> ids) where T : class, IBaseEntity, new()
		{
			return client.Queryable<T>().In((t) => t.Id, ids.ToArray()).ToList();
		}


		public static T? Save<T>(this SqlSugarClient client, ISaveModel model) where T : class, IBaseEntity, new()
		{
			if (string.IsNullOrEmpty(model.Id)) model.Id = Guid.NewGuid().ToString();
			var obj = client.Queryable<T>().InSingle(model.Id);

			var data = BaseEntity.Merge(obj, model);

			if (obj != null)
			{
				if (data != null) client.Update(data);
			}
			else
			{
				if (data != null) client.Add(data);
			}

			return data;
		}
	}
}
