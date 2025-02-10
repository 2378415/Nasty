using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Diagnostics;

namespace Nasty.Core.Attributes
{

	/// <summary>
	/// 权限标签,默认交集处理权限,但是角色与权限同时存在,则必须同时满足交集
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class NastyAuthorizeAttribute : Attribute, IAuthorizationFilter
	{
		/// <summary>
		/// 权限标志
		/// </summary>
		public string? AuthenticationSchemes { get; set; }

		/// <summary>
		/// 角色-英文逗号分隔
		/// </summary>
		public string? Roles { get; set; }

		/// <summary>
		/// 权限-英文逗号分隔
		/// </summary>
		public string? Permissions { get; set; }

		/// <summary>
		/// 是否并集处理权限,所有权限通过即可
		/// </summary>
		public bool IsUnion { get; set; } = false;


		private HashSet<string> _roles
		{
			get
			{
				if (string.IsNullOrEmpty(this.Roles)) return new HashSet<string>();
				return this.Roles.Split(',').ToHashSet();
			}
		}

		private HashSet<string> _permissions
		{
			get
			{
				if (string.IsNullOrEmpty(this.Permissions)) return new HashSet<string>();
				return this.Permissions.Split(',').ToHashSet();
			}
		}



		public NastyAuthorizeAttribute()
		{

		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			// 检查是否存在 AllowAnonymous 属性
			var allowAnonymous = context.ActionDescriptor.EndpointMetadata
				.OfType<AllowAnonymousAttribute>()
				.Any();

			// 忽略授权
			if (allowAnonymous) return;

			// 检查认证方案
			if (!string.IsNullOrEmpty(AuthenticationSchemes))
			{
				var schemes = AuthenticationSchemes.Split(',');
				bool isAuthenticated = false;

				foreach (var scheme in schemes)
				{
					var authResult = context.HttpContext.AuthenticateAsync(scheme.Trim()).Result;
					if (authResult.Succeeded)
					{
						isAuthenticated = true;
						break;
					}
				}

				if (!isAuthenticated)
				{
					context.Result = new UnauthorizedResult();
					return;
				}
			}

			var userPermissions = context.HttpContext.User.Claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToHashSet();
			var userRoles = context.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet();

			if (this.IsUnion)
			{
				//并集权限处理，如果标签Permissions或者Roles中存在，则必须同时拥有权限和角色
				var isPermissionAny = (_permissions.Count == 0) ? true : _permissions.IsSubsetOf(userPermissions);
				var isRoleAny = (_roles.Count == 0) ? true : _roles.IsSubsetOf(userRoles);

				if (!(isPermissionAny && isRoleAny))
				{
					context.Result = new UnauthorizedResult();
					return;
				}
			}
			else
			{
				//交集权限处理，如果标签Permissions或者Roles中存在，则必须同时拥有权限和角色
				var isPermissionAny = (_permissions.Count == 0) ? true : userPermissions.Intersect(_permissions).Any();
				var isRoleAny = (_roles.Count == 0) ? true : userRoles.Intersect(_roles).Any();

				if (!(isPermissionAny && isRoleAny))
				{
					context.Result = new UnauthorizedResult();
					return;
				}
			}

			// 权限验证成功，继续处理请求
			context.Result = null;
		}
	}
}
