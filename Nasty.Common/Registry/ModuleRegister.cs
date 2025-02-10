using Autofac;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;
using Module = Autofac.Module;
using Autofac.Core;

namespace Nasty.Common.Registry
{
	/// <summary>
	/// 自动注册基类
	/// </summary>
	public abstract class ModuleRegister : Module, IModuleRegister
	{
		public virtual string SwaggerName { get; set; } = string.Empty;

		protected override void Load(ContainerBuilder builder)
		{
			var assembly = this.GetType().Assembly;

			// 注册所有继承IAutofacRegister
			builder.RegisterAssemblyTypes(assembly)
				.Where(t => typeof(IAutofacRegister).IsAssignableFrom(t) && t != typeof(IAutofacRegister))
				.AsImplementedInterfaces()
				.PropertiesAutowired()
				.InstancePerLifetimeScope();
		}
	}

	public interface IModuleRegister : IModule
	{
		public string SwaggerName { get; set; }
	}
}
