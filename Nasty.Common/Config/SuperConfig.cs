using Microsoft.Extensions.Configuration;

namespace Nasty.Common.Config
{
	public static class SuperConfig
	{
		private static IConfiguration? _configuration { get; set; }

        private static string? _bucketName;

        private static void TryInit()
		{
			if (_configuration != null) return;
			_configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
		}

        public static string GetBucketName()
        {
            if (_bucketName == null)
            {
                _bucketName = Get("FileS3:BucketName");
            }
            return _bucketName;
        }

        public static string Get(string key)
		{
			TryInit();
			return _configuration?.GetValue<string>(key) ?? string.Empty;
		}

        public static T? Get<T>(string key)
        {
            TryInit();
            var res = _configuration.GetSection(key).Get<T>();
            return res;
        }
    }
}
