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
        public virtual IEnumerable<Type> ActionFilterTypes { get; set; } = Array.Empty<Type>();

        protected override void Load(ContainerBuilder builder)
        {
            var assembly = this.GetType().Assembly;

            // 注册所有继承IAutofacRegister
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(IAutofacRegister).IsAssignableFrom(t) && t != typeof(IAutofacRegister))
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerLifetimeScope();


            // 自动注册所有实现IMiddleware的类型
            var middlewareTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IAutofacMiddleware).IsAssignableFrom(t));

            foreach (var type in middlewareTypes)
            {
                builder.RegisterType(type).AsSelf().As<IMiddleware>().InstancePerLifetimeScope();
            }

            ActionFilterTypes = assembly.GetTypes()
                                        .Where(t => typeof(IAutofacActionFilter).IsAssignableFrom(t) && t.IsClass && !t.IsInterface && !t.IsAbstract)
                                        .ToList();
        }
    }

    public interface IModuleRegister : IModule
    {
        public string SwaggerName { get; set; }

        public IEnumerable<Type> ActionFilterTypes { get; set; }
    }
}
