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
        //�˷���������ʱ���á�ʹ�ô˷�����������ӷ���
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHttpContextAccessor(); /*��ӻ�ȡTTP�����Ķ������*/
            services.AddHttpClient();  /*��ӷ���Http�������*/

            services.AddEFCore(Configuration)
                    .AddIdentityservice(Configuration)
                    .AddAccessTokenAuthentication(Configuration)
                    .AddCustomSwagger()
                    .AddCusomException()
                    .AddCustomAddCors()
                    .AddCustomSingleR()
                    .AddCustomBackgroundTask(Configuration);
        }
        //�˷���������ʱ���á�ʹ�ô˷�������HTTP����ܵ���
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<GlobalExceptionMiddleware>();//���ȫ���쳣�����м��

            #region ����Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            //����AppTypesö�ټ��ض��swagger�汾
            typeof(AppTypes).GetEnumNames().ToList().ForEach(Version =>
            {
                options.SwaggerEndpoint($"/swagger/{Version}/swagger.json", $"�汾ѡ��:{Version}");
                //·��awagger�������ã�����Ϊ�գ���ʾֱ���ڸ�������localhost:8001�����ʸ��ļ�,
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
            #region ������֤��Ȩ�м��
            app.UseAuthentication();
            app.UseAuthorization();
            #endregion
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //���ͷ���
                endpoints.MapHub<ChatHub>("/ChatHub", options =>
                {
                    //ָ�� ����ѯ ����� WebSockets ����
                    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                }).RequireCors(t => t.WithOrigins(new string[] { "http://127.0.0.1:8848" })
                   .AllowAnyHeader()
                   .WithMethods("GET", "POST")
                   .AllowCredentials()
                    );
            });
        }

        #region ����Autofac 
        /// <summary>
        /// �Զ�����������ע��
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
                #region ֱ��ע��ĳһ����ͽӿ�
                container.RegisterType<PolicyHandler>().As<IAuthorizationHandler>().SingleInstance();
                container.RegisterType<OHEsystemContext>().As<DbContext>().AsImplementedInterfaces();
                container.RegisterType<ChatHub>().As<Hub>().SingleInstance();
                container.RegisterType<ChatClientServer>().As<IChatClientServer>().AsImplementedInterfaces();
                #endregion

                #region ���нӿڲ�ķ���ע��
                //��1����ȡע����Ŀ����·��
                var basePath = AppContext.BaseDirectory;
                var servicesDllFile = Path.Combine(basePath, "OHEXML.Server.dll");
                var repositoryDllFile = Path.Combine(basePath, "OHEXML.Repository.dll");

                if (!(File.Exists(servicesDllFile) || File.Exists(repositoryDllFile)))
                {
                    var msg = "Repository.dll��service.dll ���ܶ�ʧ!!��� bin �ļ��У���������";
                    Log4NetHelper.LogErr(msg);
                    throw new Exception(msg);
                }

                // ��3����ȡ���򼯷��񣬲�ע��
                //RegisterAssemblyTypes()�ڳ�����ע���������͡����ؽ��: ע������������������ע�ᡣ
                //AsImplementedInterfaces()ָ������ɨ������е�����ע��Ϊ�ṩ����
                //InstancePerLifetimeScope()����������Ա�ÿ�������������ý��������ڵ���ILIFIETimeScope�У������ͬ�Ĺ���ʵ����
                container.RegisterAssemblyTypes(Assembly.LoadFrom(servicesDllFile), Assembly.LoadFrom(repositoryDllFile))
                          //Server�������Ͳ����ǳ����
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
    /// �Զ�����չ������
    /// </summary>
    internal static class CustomExtensionsMethods
    {
        /// <summary>
        /// EFCore����
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddEFCore(this IServiceCollection services, IConfiguration configuration)
        {
            //�����ĵ���ȡ�����ַ���ע��DBContext
            services.AddDbContext<OHEsystemContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SQLConnection1"));
            });
            return services;
        }
        /// <summary>
        /// �Զ���AccessToken��֤����
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAccessTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            //��֤
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.Authority = configuration["IdentityServer:Address"];//��Դ
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
                 options.Audience = "OHEXMLAPI";//����
             });
            //��Ȩ
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
        /// �Զ���Token��֤����
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            #region Token��֤
            var validIssuer = configuration["Jwt:Issuer"];//ǩ����
            var validAudience = configuration["Jwt:Audience"];//������
            var expire = configuration["Jwt:ExpireMinutes"];//����ʱ��
            TimeSpan expiration = TimeSpan.FromMinutes(Convert.ToDouble(expire));
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"])); //HS256�Գƽ�����Կ

            services.AddAuthentication(s =>
            {
                //��2��Authentication
                s.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                s.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                s.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(s =>
            {
                //��3��ʹ�� Jwt bearer 
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

            services.AddAuthorization(options =>//��1�����ڲ��Ե���Ȩ�ɰ������ǵĲ��Դ���������Ȩ�޵Ŀ��ơ�
            {
                options.AddPolicy("Permission",//ʹ��==��[Authorize("Permission")]
                   policy =>
                   {
                       policy.Requirements.Add(new PolicyRequirement());
                   });
            });
            #endregion

            return services;
        }
        /// <summary>
        /// Swagger����
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerGeneratorOptions.ConflictingActionsResolver = (apis) => apis.First();
                #region ���ע��
                string xmlPath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "OHEXML.xml");
                c.IncludeXmlComments(xmlPath);
                #endregion

                #region ���Я��token��֤��ť
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "������Token",
                    Name = "Authorization",//������Ĭ�ϵĲ�������
                    In = ParameterLocation.Header,//��token����header��
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

                #region ֧�ְ汾����
                typeof(AppTypes).GetEnumNames().ToList().ForEach(Version =>
                    {
                        c.SwaggerDoc(Version, new OpenApiInfo
                        {
                            Title = $"{Version}:WebApi",
                            Version = Version,
                            Description = $"Sunflower:{Version}�汾"
                        });
                    });
                #endregion

            });
            return services;
        }
        /// <summary>
        /// �쳣����
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCusomException(this IServiceCollection services)
        {
            #region �����쳣���������
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
        /// �������
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomAddCors(this IServiceCollection services)
        {

            #region ָ��������
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

            #region �����κ���վ����
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
        /// SingleR����
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomSingleR(this IServiceCollection services)
        {
            //����������==>ChatHub ������������ṩ��ȫ��ѡ�� AddSignalR
            services.AddSignalR().AddHubOptions<ChatHub>(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true; /*�����������������쳣ʱ����ϸ���쳣��Ϣ�����ص��ͻ���*/
                hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);/* ���������δ�ڴ�ʱ�����ڷ�����Ϣ������Զ����� ping ��Ϣ��ʹ���ӱ��ִ�״̬*/
            })
                .AddJsonProtocol(options =>
                {
                    //�������������ƵĴ�Сд
                    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                });
            return services;
        }
        /// <summary>
        /// ��̨�йܷ���
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomBackgroundTask(this IServiceCollection services, IConfiguration configuration)
        {
            #region �������
            //services.AddSingleton<MonitorLoop>();
            //services.AddHostedService<QueuedServiceCenter>();
            //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>(ctx =>
            //{
            //    //int.TryParse()�����ֵ��ַ�����ʾ��ʽת��Ϊ���Ч��32λ�з�������
            //    if (!int.TryParse(configuration["QueueCapacity"], out int queueCapacity))
            //    {
            //        queueCapacity = 100;/*�Ŷ�����*/
            //    }
            //    return new BackgroundTaskQueue(queueCapacity);
            //});
            #endregion

            #region ������
            services.AddHostedService<ScopedServiceCenter>();
            #endregion
            return services;
        }
        /// <summary>
        /// Hangfire������ȷ���
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
        /// IdentityServer����
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityservice(this IServiceCollection services, IConfiguration configuration)
        {
            #region ����IdentityServer����
            services.AddIdentityServer(options =>
            {
                options.IssuerUri = configuration.GetSection("IdentityServer").GetValue<string>("Address");
            })
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(Config.ApiSources)//ע����Դ
                .AddInMemoryClients(Config.Clients)//ע��ģʽ
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();//��֤����
            #endregion
            return services;
        }
    }
}
