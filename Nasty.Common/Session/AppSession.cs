using Nasty.Common.Model;
using SqlSugar;

namespace Nasty.Common.Session
{
	/// <summary>
	/// 当前控制器生命周期数据共享，使用AsyncLocal实现
	/// </summary>
	public class AppSession
	{
		/// <summary>
		/// 当前生命周期用户
		/// </summary>
		public static AsyncLocal<UserInfoModel> CurrentUser = new AsyncLocal<UserInfoModel>();

		/// <summary>
		/// 当前生命周期数据库
		/// </summary>
		public static AsyncLocal<SqlSugarClient> CurrentDb = new AsyncLocal<SqlSugarClient>();

		/// <summary>
		/// 当前生命周期HttpContext
		/// </summary>
		public static AsyncLocal<HttpContext> CurrentHttp = new AsyncLocal<HttpContext>();
	}
}
