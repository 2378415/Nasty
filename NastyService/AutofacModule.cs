using Autofac;
using Autofac.Core.Registration;
using Nasty.Common.Registry;

namespace NastyService
{
	public static class AutofacModule
	{
		public static List<IModuleRegister> GetModules()
		{
			var modules = new List<IModuleRegister>
			{
				new Nasty.PortalModule.PortalModule()
			};

			return modules;
		}
	}
}
