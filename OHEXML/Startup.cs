using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OHEXML.Common.LogHelper;
using OHEXML.Filter.ExceptionsFilter;
using OHEXML.Infrastructure.PolicyHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using static OHEXML.Common.EnumLIst.AppTypesEnum;
using System.Threading.Tasks;
using System.Text;
using OHEXML.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http.Connections;
using OHEXML.Entity.Context;
using OHEXML.Infrastructure.HostedServices.ScopedHostedService;
using OHEXML.Infrastructure.IdentityServer4;

namespace OHEXML
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        //此方法由运行时调用。使用此方法向容器添加服务。
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHttpContextAccessor(); /*添加获取TTP上下文对象服务*/
            services.AddHttpClient();  /*添加发送Http请求服务*/

            services.AddEFCore(Configuration)
                    .AddIdentityservice(Configuration)
                    .AddAccessTokenAuthentication(Configuration)
                    .AddCustomSwagger()
                    .AddCusomException()
                    .AddCustomAddCors()
                    .AddCustomSingleR()
                    .AddCustomBackgroundTask(Configuration);
        }
        //此方法由运行时调用。使用此方法配置HTTP请求管道。
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<GlobalExceptionMiddleware>();//添加全局异常处理中间介

            #region 配置Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            //根据AppTypes枚举加载多个swagger版本
            typeof(AppTypes).GetEnumNames().ToList().ForEach(Version =>
            {
                options.SwaggerEndpoint($"/swagger/{Version}/swagger.json", $"版本选择:{Version}");
                //路径awagger访问配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,
                options.RoutePrefix = string.Empty;
            })
            );
            #endregion

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString("/src"),
                OnPrepareResponse = (c) =>
                {
                    c.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                }
            });
            app.UseRouting();
            app.UseCors("AllowCors");
            app.UseIdentityServer();
            #region 配置认证授权中间件
            app.UseAuthentication();
            app.UseAuthorization();
            #endregion
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //推送服务
                endpoints.MapHub<ChatHub>("/ChatHub", options =>
                {
                    //指定 长轮询 传输和 WebSockets 传输
                    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                }).RequireCors(t => t.WithOrigins(new string[] { "http://127.0.0.1:8848" })
                   .AllowAnyHeader()
                   .WithMethods("GET", "POST")
                   .AllowCredentials()
                    );
            });
        }

        #region 集成Autofac 
        /// <summary>
        /// 自定义容器服务注册
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModuleRegister());
        }
        public class AutofacModuleRegister : Autofac.Module
        {
            protected override void Load(ContainerBuilder container)
            {
                #region 直接注册某一个类和接口
                container.RegisterType<PolicyHandler>().As<IAuthorizationHandler>().SingleInstance();
                container.RegisterType<OHEsystemContext>().As<DbContext>().AsImplementedInterfaces();
                container.RegisterType<ChatHub>().As<Hub>().SingleInstance();
                container.RegisterType<ChatClientServer>().As<IChatClientServer>().AsImplementedInterfaces();
                #endregion

                #region 带有接口层的服务注入
                //【1】获取注入项目绝对路径
                var basePath = AppContext.BaseDirectory;
                var servicesDllFile = Path.Combine(basePath, "OHEXML.Server.dll");
                var repositoryDllFile = Path.Combine(basePath, "OHEXML.Repository.dll");

                if (!(File.Exists(servicesDllFile) || File.Exists(repositoryDllFile)))
                {
                    var msg = "Repository.dll和service.dll 可能丢失!!检查 bin 文件夹，并拷贝。";
                    Log4NetHelper.LogErr(msg);
                    throw new Exception(msg);
                }

                // 【3】获取程序集服务，并注册
                //RegisterAssemblyTypes()在程序集中注册所有类型。返回结果: 注册生成器，允许配置注册。
                //AsImplementedInterfaces()指定将已扫描程序集中的类型注册为提供所有
                //InstancePerLifetimeScope()配置组件，以便每个依赖组件或调用解析（）在单个ILIFIETimeScope中，获得相同的共享实例。
                container.RegisterAssemblyTypes(Assembly.LoadFrom(servicesDllFile), Assembly.LoadFrom(repositoryDllFile))
                          //Server，且类型不能是抽象的
                          .Where(t => (t.FullName.EndsWith("Server") || t.FullName.EndsWith("Repository")) && !t.IsAbstract)
                          .AsImplementedInterfaces()
                          .InstancePerLifetimeScope()
                          .EnableInterfaceInterceptors();

                container.RegisterAssemblyTypes(Assembly.LoadFrom(repositoryDllFile))
                          .InstancePerLifetimeScope();
                #endregion
            }
        }
        #endregion
    }
    /// <summary>
    /// 自定义拓展方法类
    /// </summary>
    internal static class CustomExtensionsMethods
    {
        /// <summary>
        /// EFCore服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddEFCore(this IServiceCollection services, IConfiguration configuration)
        {
            //配置文档获取连接字符串注入DBContext
            services.AddDbContext<OHEsystemContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SQLConnection1"));
            });
            return services;
        }
        /// <summary>
        /// 自定义AccessToken验证规则
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAccessTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            //认证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.Authority = configuration["IdentityServer:Address"];//来源
                 options.RequireHttpsMetadata = false;
                 options.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         var accessToken = context.Request.Query["access_token"];
                         var path = context.HttpContext.Request.Path;
                         if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                         {
                             context.Token = accessToken;
                         }
                         return Task.CompletedTask;
                     }
                 };
                 options.Audience = "OHEXMLAPI";//受众
             });
            //授权
            services.AddAuthorization(options =>
            {
                typeof(AppTypes).GetEnumNames().ToList().ForEach(AppType =>
                {
                    //AddPolicy(string policyName ,Action<AuthorizationPolicyBuilder> configurePolicy)
                    options.AddPolicy(AppType, policy =>
                    {
                        AppTypes type = (AppTypes)Enum.Parse(typeof(AppTypes), AppType);
                        policy.Requirements.Add(new PolicyRequirement(new AppTypes []{ type }));
                    });
                });
            });
            return services;
        }
        /// <summary>
        /// 自定义Token验证规则
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            #region Token验证
            var validIssuer = configuration["Jwt:Issuer"];//签发者
            var validAudience = configuration["Jwt:Audience"];//接收者
            var expire = configuration["Jwt:ExpireMinutes"];//过期时间
            TimeSpan expiration = TimeSpan.FromMinutes(Convert.ToDouble(expire));
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"])); //HS256对称解密秘钥

            services.AddAuthentication(s =>
            {
                //【2】Authentication
                s.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                s.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                s.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(s =>
            {
                //【3】使用 Jwt bearer 
                s.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = validIssuer,
                    ValidAudience = validAudience,
                    IssuerSigningKey = securityKey,
                    ClockSkew = expiration,
                    ValidateLifetime = true
                };
                s.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        //Token expired
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>//【1】基于策略的授权可按照我们的策略处理程序进行权限的控制。
            {
                options.AddPolicy("Permission",//使用==》[Authorize("Permission")]
                   policy =>
                   {
                       policy.Requirements.Add(new PolicyRequirement());
                   });
            });
            #endregion

            return services;
        }
        /// <summary>
        /// Swagger服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerGeneratorOptions.ConflictingActionsResolver = (apis) => apis.First();
                #region 添加注释
                string xmlPath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "OHEXML.xml");
                c.IncludeXmlComments(xmlPath);
                #endregion

                #region 添加携带token验证按钮
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入Token",
                    Name = "Authorization",//劲舞团默认的参数名称
                    In = ParameterLocation.Header,//把token放在header中
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                       new OpenApiSecurityScheme{
                        Reference =new OpenApiReference{Type=ReferenceType.SecurityScheme,Id="Bearer"}
                        }
                       ,new string[]{}
                    }
                });
                #endregion

                #region 支持版本控制
                typeof(AppTypes).GetEnumNames().ToList().ForEach(Version =>
                    {
                        c.SwaggerDoc(Version, new OpenApiInfo
                        {
                            Title = $"{Version}:WebApi",
                            Version = Version,
                            Description = $"Sunflower:{Version}版本"
                        });
                    });
                #endregion

            });
            return services;
        }
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCusomException(this IServiceCollection services)
        {
            #region 配置异常处理过滤器
            //API
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(GlobalExceptionsFilter));
            });
            //MVC
            services.AddMvc(option =>
            {
                option.Filters.Add(typeof(GlobalExceptionsFilter));
            });
            #endregion
            return services;
        }
        /// <summary>
        /// 跨域服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomAddCors(this IServiceCollection services)
        {

            #region 指定的域名
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(MyAllowSpecificOrigins, builder =>
            //                      {
            //                          builder.WithOrigins("http://example.com", "http://www.contoso.com")
            //                                 .AllowAnyHeader()
            //                                 .AllowAnyMethod();
            //                      });
            //});
            #endregion

            #region 允许任何网站访问
            services.AddCors(options =>
            {
                options.AddPolicy("AllowCors", builder =>
                {
                    builder.SetIsOriginAllowed(origin => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            #endregion
            return services;
        }
        /// <summary>
        /// SingleR服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomSingleR(this IServiceCollection services)
        {
            //单个集线器==>ChatHub 用于替代中心提供的全局选项 AddSignalR
            services.AddSignalR().AddHubOptions<ChatHub>(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true; /*集线器方法中引发异常时，详细的异常消息将返回到客户端*/
                hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);/* 如果服务器未在此时间间隔内发送消息，则会自动发送 ping 消息，使连接保持打开状态*/
            })
                .AddJsonProtocol(options =>
                {
                    //不更改属性名称的大小写
                    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                });
            return services;
        }
        /// <summary>
        /// 后台托管服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomBackgroundTask(this IServiceCollection services, IConfiguration configuration)
        {
            #region 任务队列
            //services.AddSingleton<MonitorLoop>();
            //services.AddHostedService<QueuedServiceCenter>();
            //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>(ctx =>
            //{
            //    //int.TryParse()将数字的字符串表示形式转换为其等效的32位有符号整数
            //    if (!int.TryParse(configuration["QueueCapacity"], out int queueCapacity))
            //    {
            //        queueCapacity = 100;/*排队容量*/
            //    }
            //    return new BackgroundTaskQueue(queueCapacity);
            //});
            #endregion

            #region 作用域
            services.AddHostedService<ScopedServiceCenter>();
            #endregion
            return services;
        }
        /// <summary>
        /// Hangfire任务调度服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddHangfireService(this IServiceCollection services, IConfiguration configuration)
        {
            // services.AddHangfire(configuration => configuration
            //.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //.UseSimpleAssemblyNameTypeSerializer()
            //.UseRecommendedSerializerSettings()
            //.UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
            //{
            //    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            //    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            //    QueuePollInterval = TimeSpan.Zero,
            //    UseRecommendedIsolationLevel = true,
            //    DisableGlobalLocks = true
            //}));
            //services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));
            //services.AddHangfireServer();
            return services;
        }
        /// <summary>
        /// IdentityServer服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityservice(this IServiceCollection services, IConfiguration configuration)
        {
            #region 配置IdentityServer服务
            services.AddIdentityServer(options =>
            {
                options.IssuerUri = configuration.GetSection("IdentityServer").GetValue<string>("Address");
            })
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(Config.ApiSources)//注册资源
                .AddInMemoryClients(Config.Clients)//注册模式
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();//验证规则
            #endregion
            return services;
        }
    }
}
