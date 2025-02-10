using Microsoft.Extensions.Configuration;

namespace Nasty.Common.Config
{
	public static class SuperConfig
	{
		private static IConfiguration? _configuration { get; set; }

		private static void TryInit()
		{
			if (_configuration != null) return;
			_configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
		}

		public static string Get(string key)
		{
			TryInit();
			return _configuration?.GetValue<string>(key) ?? string.Empty;
		}
	}
}
