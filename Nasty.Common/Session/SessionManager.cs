using Nasty.Common.Model;
using Nasty.Common.Redis;
using Newtonsoft.Json;

namespace Nasty.Common.Session
{
	public static class SessionManager
	{
		public static UserInfoModel? GetUserInfo(string account)
		{
			var key = $"{account}:UserInfo";
			var value = RedisStore.GetDatabase()?.StringGet(key).ToString();

			if (!string.IsNullOrEmpty(value))
			{
				return JsonConvert.DeserializeObject<UserInfoModel>(value ?? string.Empty);
			}

			return null;
		}

		public static void SetUserInfo(string account, UserInfoModel info)
		{
			var key = $"{account}:UserInfo";
			var value = JsonConvert.SerializeObject(info);
			RedisStore.GetDatabase(0)?.StringSet(key, value, TimeSpan.MaxValue);
		}
	}
}
