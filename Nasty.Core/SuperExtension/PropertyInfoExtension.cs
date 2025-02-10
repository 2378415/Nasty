using Nasty.Core.Entity;
using SqlSugar;
using System.Reflection;

namespace Nasty.Core.SuperExtension
{
	public static class PropertyInfoExtension
	{
		public static Type? GetSingleType(this PropertyInfo info)
		{
			try
			{
				return Tools.IsCollectionType(info.PropertyType) ? info.PropertyType.GetGenericArguments().First() : info.PropertyType;
			}
			catch
			{
				return null;
			}
		}

	}
}
