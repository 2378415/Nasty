using Microsoft.Extensions.DependencyInjection;

namespace Nasty.Common.Registry
{
    public class ServiceLocator
    {
        private static IServiceProvider? _serviceProvider { get; set; }

        public static void Registry(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T? Get<T>() where T : class
        {
            if (_serviceProvider == null) return null;
            return _serviceProvider.GetService<T>();
        }
    }
}
