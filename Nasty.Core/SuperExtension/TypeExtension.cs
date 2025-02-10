using AutoMapper.Internal;
using SqlSugar;
using System;
using System.Linq;
using System.Reflection;

namespace Nasty.Core.SuperExtension
{
	public static class TypeExtension
	{
		/// <summary>
		/// 返回不可空类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type? GetSingleType(this Type type)
		{
			try
			{
				var obj = Tools.IsCollectionType(type) ? type.GetGenericArguments().First() : type;
				return obj.IsNullableType() ? Nullable.GetUnderlyingType(obj) : obj;
			}
			catch
			{
				return null;
			}
		}
	}
}
