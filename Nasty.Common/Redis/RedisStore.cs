using Microsoft.Extensions.Configuration;
using Nasty.Common.Config;
using StackExchange.Redis;

namespace Nasty.Common.Redis
{
	public class RedisStore
	{
		private static Lazy<ConnectionMultiplexer>? LazyConnection;

		public static void Registry()
		{
			var redisConfig = SuperConfig.Get("Redis") ?? string.Empty;
			var options = ConfigurationOptions.Parse(redisConfig, true);
			options.ResolveDns = true;
			LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
		}

		public static IDatabase? GetDatabase(int index = 0)
		{
			if (LazyConnection == null) return null;
			var db = LazyConnection.Value.GetDatabase(index);
			return db;
		}
	}
}
