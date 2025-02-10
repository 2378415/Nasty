using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Nasty.Common.Config;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Nasty.Common.Session;

namespace Nasty.Common.Authorization
{
	public static class Auth
	{
		/// <summary>
		/// 构建token
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static string MakeToken(string userId, string[] roles, string[] permissions, int days)
		{
			// 创建用户声明
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, userId)
			};

			// 添加角色声明
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			// 添加权限声明
			foreach (var permission in permissions)
			{
				claims.Add(new Claim("Permission", permission));
			}

			// 创建签名凭证
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SuperConfig.Get("SecurityKey")));
			var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			// 设置 token 过期时间
			var expires = DateTime.Now.AddDays(days);

			var org = SuperConfig.Get("Organization");
			// 创建 JWT token
			var token = new JwtSecurityToken(
				issuer: org,
				audience: org,
				claims: claims,
				expires: expires,
				signingCredentials: signingCredentials
			);

			// 返回生成的 token
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		/// <summary>
		/// 构建cookie
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static void MakeCookie(string userId, string[] roles, string[] permissions, int days)
		{
			if (AppSession.CurrentHttp.Value == null) return;

			// 创建用户声明
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, userId)
			};

			// 添加角色声明
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			// 添加权限声明
			foreach (var permission in permissions)
			{
				claims.Add(new Claim("Permission", permission));
			}

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var authProperties = new AuthenticationProperties
			{
				// 配置认证属性，例如是否持久化 Cookie
				IsPersistent = true,
				ExpiresUtc = DateTimeOffset.UtcNow.AddHours(days) // 设置 Cookie 过期时间
			};

			var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), authProperties, CookieAuthenticationDefaults.AuthenticationScheme);

			var org = SuperConfig.Get("Organization");
			var cookieJwtDataFormat = new CookieJwtDataFormat(SecurityAlgorithms.HmacSha256, new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = org, // 你的颁发者
				ValidAudience = org, // 接收者的标识符
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SuperConfig.Get("SecurityKey")))
			});

			var token = cookieJwtDataFormat.Protect(ticket, DateTime.UtcNow.AddHours(days));
			AppSession.CurrentHttp.Value.Response.Cookies.Append("_TicketAuth", token, new CookieOptions
			{
				HttpOnly = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTimeOffset.UtcNow.AddHours(days)
			});
		}
	}
}
