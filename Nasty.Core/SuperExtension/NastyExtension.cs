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
using StackExchange.Redis;
using Nasty.Common.Security;
using Amazon.S3;

namespace Nasty.Core.SuperExtension
{
    public static class NastyExtension
    {
        private static List<string> _swaggerNames { get; set; } = new List<string>();

        private static List<IModuleRegister> _modules = new List<IModuleRegister>();

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
                options.AddPolicy("AllowAll", builder => { 
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader(); });
            });


            services.AddMvc((mvc) =>
            {
                var actionFilterTypes = _modules.SelectMany(m => m.ActionFilterTypes).ToList();
                foreach (var type in actionFilterTypes)
                {
                    mvc.Filters.Add(type);
                }
            })
            .AddJsonOptions(options =>
            {
                //使用原始数据输出，不使用小驼峰命名
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddFileS3();
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
                var commonModule = new Nasty.Common.CommonModule();
                _modules.Add(commonModule);
                builder.RegisterModule(commonModule);

                var coreModule = new Nasty.Core.CoreModule();
                _modules.Add(coreModule);
                builder.RegisterModule(new Nasty.Core.CoreModule());

                //注册其他模块
                foreach (var module in modules)
                {
                    _modules.Add(module);

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


        private static void AddFileS3(this IServiceCollection services)
        {
            var url = SuperConfig.Get("FileS3:Url");
            if (string.IsNullOrEmpty(url))
            {
                Tools.WriteLine("未配置S3存储地址");
                return;
            }

            var accessKey = SuperConfig.Get("FileS3:AccessKey");
            var secretKey = SuperConfig.Get("FileS3:SecretKey");    

            // 从配置文件中读取 S3 配置
            var s3Config = new AmazonS3Config
            {
                ServiceURL = url, // 第三方存储系统的服务地址
                ForcePathStyle = true // 强制路径样式，适用于大多数第三方 S3 存储
            };

            // 注册 AmazonS3Client 为单例
            services.AddSingleton<IAmazonS3>(sp =>
            {
                return new AmazonS3Client(accessKey, secretKey, s3Config);
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
            // 在Startup.cs中注册
            var middlewares = app.Services.GetServices<IMiddleware>();
            foreach (var middleware in middlewares)
            {
                app.UseMiddleware(middleware.GetType());
            }

            //允许跨域
            app.UseCors("AllowAll");

            //注册服务定位器
            ServiceLocator.Registry(app.Services);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseNastySwagger();
        }


        private static DbType GetDbType()
        {
            var type = SuperConfig.Get("ConnectionString:Type");
            switch (type)
            {
                case "MySql": return DbType.MySql;
                case "SqlServer": return DbType.SqlServer;
                case "Sqlite": return DbType.Sqlite;
                case "Oracle": return DbType.Oracle;
                case "PostgreSQL": return DbType.PostgreSQL;
                case "Dm": return DbType.Dm;
                case "Kdbndp": return DbType.Kdbndp;
                case "Oscar": return DbType.Oscar;
                case "MySqlConnector": return DbType.MySqlConnector;
                case "Access": return DbType.Access;
                case "OpenGauss": return DbType.OpenGauss;
                case "QuestDB": return DbType.QuestDB;
                case "HG": return DbType.HG;
                case "ClickHouse": return DbType.ClickHouse;
                case "GBase": return DbType.GBase;
                case "Odbc": return DbType.Odbc;
                case "OceanBaseForOracle": return DbType.OceanBaseForOracle;
                case "TDengine": return DbType.TDengine;
                case "GaussDB": return DbType.GaussDB;
                case "OceanBase": return DbType.OceanBase;
                case "Tidb": return DbType.Tidb;
                case "Vastbase": return DbType.Vastbase;
                case "PolarDB": return DbType.PolarDB;
                case "Doris": return DbType.Doris;
                case "Xugu": return DbType.Xugu;
                case "GoldenDB": return DbType.GoldenDB;
                case "Custom": return DbType.Custom;
                default: return DbType.SqlServer;
            }

        }

        private static void RegistryDb(ContainerBuilder builder)
        {
            //配置雪花工作ID
            var workId = SuperConfig.Get("SnowFlakeWorkId");
            if (string.IsNullOrEmpty(workId)) throw new Exception("未配置雪花WorkId");
            SnowFlakeSingle.WorkId = int.Parse(workId);
            Tools.WriteLine($"雪花WorkId：{workId}");

            var parameter = new NamedParameter("config", new ConnectionConfig
            {
                ConnectionString = SuperConfig.Get("ConnectionString:BaseDb"),
                DbType = GetDbType(),
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
