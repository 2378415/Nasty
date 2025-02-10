using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nasty.Common.Authorization;
using Nasty.Common.Redis;
using Nasty.Common.Registry;
using Nasty.Common.Session;
using Nasty.Core.Attributes;
using Nasty.Common.Config;
using SqlSugar;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nasty.Core.SuperExtension
{
	public static class NastyExtension
	{
		private static List<string> _swaggerNames { get; set; } = new List<string>();

		/// <summary>
		/// 注册Nasty,在AddNastyHost之后调用
		/// </summary>
		/// <param name="services"></param>
		public static void AddNasty(this IServiceCollection services)
		{
			//注册Redis
			RedisStore.Registry();

			// 设置控制器允许跨域
			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
			});

			services.AddMvc((mvc) =>
			{
				mvc.Filters.AddService<IActionFilter>();
			})
			.AddJsonOptions(options =>
			{
				//使用原始数据输出，不使用小驼峰命名
				options.JsonSerializerOptions.PropertyNamingPolicy = null;
			});

			services.AddNastySwagger();
			services.AddNastyAuthorization();
		}

		/// <summary>
		/// 进行模块注册
		/// </summary>
		/// <param name="host"></param>
		/// <param name="args"></param>
		/// <param name="modules"></param>
		/// <param name="action"></param>
		public static void AddNastyHost(this ConfigureHostBuilder host, string[] args, List<IModuleRegister> modules, Action<ContainerBuilder>? action = null)
		{
			//使用Autofac注册服务
			host.ConfigureDefaults(args).UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
			{
				//注册数据库
				RegistryDb(builder);

				//注册模块
				builder.RegisterModule(new Nasty.Common.CommonModule());
				builder.RegisterModule(new Nasty.Core.CoreModule());

				//注册其他模块
				foreach (var module in modules)
				{
					if (!string.IsNullOrEmpty(module.SwaggerName))
					{
						_swaggerNames.Add(module.SwaggerName);
					}
					builder.RegisterModule(module);
				}

				//额外注册
				if (action != null) action(builder);
			}));
		}

		/// <summary>
		/// 自动根据模块填充Swagger，在AddNastyHost之后调用
		/// </summary>
		/// <param name="services"></param>
		private static void AddNastySwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				foreach (var name in _swaggerNames)
				{
					c.SwaggerDoc(name, new OpenApiInfo { Title = $"{name} API", Version = "v1" });
				}

				c.DocInclusionPredicate((docName, apiDesc) =>
				{
					//分组名与docName相同即可
					return docName == apiDesc.GroupName;
				});


				//添加token验证
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description = "请输入token,格式为 Bearer xxxxxxxx",
					Name = "Authorization", //标头名
					In = ParameterLocation.Header,  //表示头部
					Type = SecuritySchemeType.ApiKey,
					BearerFormat = "JWT",  //token
					Scheme = "Bearer"
				});
				//添加验证条件
				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[] { } //不设权限
                    }
				});
			});
		}


		/// <summary>
		/// 添加权限配置
		/// </summary>
		/// <param name="services"></param>
		private static void AddNastyAuthorization(this IServiceCollection services)
		{
			var org = SuperConfig.Get("Organization");
			var key = Encoding.UTF8.GetBytes(SuperConfig.Get("SecurityKey"));
			var validationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = org, // 你的颁发者
				ValidAudience = org, // 接收者的标识符
				IssuerSigningKey = new SymmetricSecurityKey(key)
			};

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			 .AddJwtBearer(jwtOptions =>
			 {
				 jwtOptions.TokenValidationParameters = validationParameters;
			 })
			 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
			 {
				 // 配置 Cookie 认证
				 options.Cookie.Name = "_TicketAuth";
				 options.Cookie.HttpOnly = true;
				 options.Cookie.SameSite = SameSiteMode.Strict; // 防止 CSRF 攻击
				 options.TicketDataFormat = new CookieJwtDataFormat(SecurityAlgorithms.HmacSha256, validationParameters);
			 });
		}

		private static void UseNastySwagger(this WebApplication app)
		{
			//使用Swagger
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				foreach (var name in _swaggerNames)
				{
					c.SwaggerEndpoint($"/swagger/{name}/swagger.json", name);
				}

				c.DocExpansion(DocExpansion.None);
			});
		}

		public static void UseNasty(this WebApplication app)
		{
			//允许跨域
			app.UseCors("AllowAll");

			//注册服务定位器
			ServiceLocator.Registry(app.Services);

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseNastySwagger();
		}

		private static void RegistryDb(ContainerBuilder builder)
		{
			var parameter = new NamedParameter("config", new ConnectionConfig
			{
				ConnectionString = SuperConfig.Get("ConnectionString"),
				DbType = DbType.SqlServer,
				InitKeyType = InitKeyType.Attribute,
				IsAutoCloseConnection = true
			});

			//注册SqlSugarClient
			builder.RegisterType<SqlSugarClient>().WithParameter(parameter).InstancePerLifetimeScope()
				.OnActivated(delegate (IActivatedEventArgs<SqlSugarClient> e)
				{
					e.Instance.Aop.OnLogExecuting = delegate (string sql, SugarParameter[] pars)
					{
						Console.WriteLine(sql + "\r\n" + e.Instance.Utilities.SerializeObject(pars.ToDictionary((SugarParameter it) => it.ParameterName, (SugarParameter it) => it.Value)));
						Console.WriteLine();
					};
				});

		}
	}
}
