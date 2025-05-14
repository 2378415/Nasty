namespace Nasty.Common.Model
{
	public class UserInfoModel
	{
		public string? Id { get; set; }

		public string? Name { get; set; }

		public string? Account { get; set; }

		public string? Avatar { get; set; }

        public string? Token { get; set; }

		public string[]? Roles { get; set; }

		public string[]? Permissions { get; set; }
	}
}
