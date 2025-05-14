using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Nasty.Common.Config;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Nasty.Common.Authorization
{
    public class CookieJwtDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string _algorithm;
        private readonly TokenValidationParameters _validationParameters;

        public CookieJwtDataFormat(string algorithm, TokenValidationParameters validationParameters)
        {
            _algorithm = algorithm;
            _validationParameters = validationParameters;
        }

        public string Protect(AuthenticationTicket data, DateTime expires)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SuperConfig.Get("SecurityKey")));
            var creds = new SigningCredentials(key, _algorithm);

            var org = SuperConfig.Get("Organization");
			var token = new JwtSecurityToken(
                issuer: org,
                audience: org,
                claims: data.Principal.Claims,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string Protect(AuthenticationTicket data)
        {
            return Protect(data, DateTime.Now.AddDays(3)); // 默认过期时间为3天
        }

        public string Protect(AuthenticationTicket data, string? purpose)
        {
            return Protect(data);
        }

        public AuthenticationTicket Unprotect(string? protectedText)
        {
            if (string.IsNullOrEmpty(protectedText)) throw new ArgumentNullException(nameof(protectedText));

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(protectedText, _validationParameters, out var validToken);

            return new AuthenticationTicket(principal, new AuthenticationProperties(), CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public AuthenticationTicket Unprotect(string? protectedText, string? purpose)
        {
            return Unprotect(protectedText);
        }
    }
}
