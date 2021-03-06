using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;

namespace UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            
            //services.AddRazorPages();     //zjt: tohle nev�m, zda nebude chyb�t!!!!
            //services.AddHttpContextAccessor();
            //services.AddSingleton<UI.Models.GlobalHelper>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            
            //services.AddKendo();

          

            services.AddAuthentication("CookieAuthentication")
                 .AddCookie("CookieAuthentication", config =>
                 {
                     config.Cookie.HttpOnly = true;
                     config.Cookie.SecurePolicy= CookieSecurePolicy.SameAsRequest;
                     config.Cookie.SameSite = SameSiteMode.None;
                     config.SlidingExpiration = true;
                     config.ExpireTimeSpan = TimeSpan.FromHours(24);
                     config.Cookie.Name = "CormenCloudCore";
                     config.ReturnUrlParameter = "returnurl";
                     config.LoginPath = "/Login/UserLogin";

                     //config.Events.OnValidatePrincipal = Program.PrincipalValidator.ValidateAsync;
                 });

            // Set requirements for security
            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequireUppercase = true;
            //    options.Password.RequireLowercase = true;
            //    options.Lockout.MaxFailedAccessAttempts = 5;

            //    // Default User settings.
            //    options.User.AllowedUserNameCharacters ="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            //    options.User.RequireUniqueEmail = true;                
            //});

            services.Configure<Microsoft.Extensions.WebEncoders.WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(System.Text.Unicode.UnicodeRanges.All);
            });

            services.AddControllers();      //kv�li telerik reporting
            services.AddControllersWithViews();
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;      //kv�li telerik reporting
            });

            services.AddRazorPages().AddNewtonsoftJson();   //kv�li telerik reporting

            var conf = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var strLogFolder = conf.GetSection("Folders")["Log"];
            if (string.IsNullOrEmpty(strLogFolder))
            {
                strLogFolder = System.IO.Directory.GetCurrentDirectory() + "\\Logs";
            }


            var execAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var versionTime = new System.IO.FileInfo(execAssembly.Location).LastWriteTime;

            services.AddSingleton<BL.RunningApp>(x => new BL.RunningApp() {
                ConnectString = conf.GetSection("ConnectionStrings")["AppConnection"]
                , AppName = conf.GetSection("App")["Name"]
                , AppVersion = conf.GetSection("App")["Version"]
                ,AppBuild= "build: "+BO.BAS.ObjectDateTime2String(versionTime)                
                ,UploadFolder = conf.GetSection("Folders")["Upload"]
                , TempFolder = conf.GetSection("Folders")["Temp"]
                , LogFolder = strLogFolder
                ,ReportFolder = conf.GetSection("Folders")["Report"]
                ,ApiLogin=conf.GetSection("Api")["Login"]
            }) ;

            services.AddSingleton<BL.TheColumnsProvider>();
            services.AddSingleton<BL.ThePeriodProvider>();

            services.TryAddSingleton<IReportServiceConfiguration>(sp =>
            new ReportServiceConfiguration
            {
                ReportingEngineConfiguration = ConfigurationHelper.ResolveConfiguration(sp.GetService<IWebHostEnvironment>()),
                HostAppId = "ReportingCore3App",
                Storage = new Telerik.Reporting.Cache.File.FileStorage()                
            });

            services.AddScoped<BO.RunningUser, BO.RunningUser>();            
            services.AddScoped<BL.Factory,BL.Factory>();


            //services.AddScoped<BL.TheColumnsProvider, BL.TheColumnsProvider>();


            services.AddHostedService<UI.TheRobot>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            //app.UseDeveloperExceptionPage();


            //app.UseDatabaseErrorPage();
            //toto d�t zp�t:
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseExceptionHandler("/Home/Error");

            //app.UseHsts();
            //app.UseStatusCodePages("text/plain", "Cormen Cloud, status code page, status code: {0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseAuthorization();

          

          

            app.UseRequestLocalization();
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("cs-CZ");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("cs-CZ");



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");                
                //endpoints.MapRazorPages();

            });

            //loggerFactory.AddFile(Configuration.GetSection("Logging"));
            loggerFactory.AddFile("Logs/info-{Date}.log", LogLevel.Information);
            loggerFactory.AddFile("Logs/debug-{Date}.log", LogLevel.Debug);
            loggerFactory.AddFile("Logs/error-{Date}.log", LogLevel.Error);
            

            
            
            
        }
    }
}
