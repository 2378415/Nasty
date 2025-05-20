using Nasty.Common.Model;
using SqlSugar;

namespace Nasty.PortalModule.Areas.User.Model
{
    public class UserModel : SaveModel
    {
        public string? Name { get; set; }

        public string? Avatar { get; set; }

        public string? Account { get; set; }

        public string? Password { get; set; }
    }
}
