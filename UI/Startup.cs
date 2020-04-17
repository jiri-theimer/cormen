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
using UI.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            //services.AddHttpContextAccessor();
            //services.AddSingleton<UI.Models.GlobalHelper>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            // Set requirements for security
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Default User settings.
                options.User.AllowedUserNameCharacters ="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;                
            });




            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/Identity/Account/Login"; // Set here login path.
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // set here access denied path.
                options.SlidingExpiration = true; // resets cookie expiration if more than half way through lifespan
                options.ExpireTimeSpan = TimeSpan.FromDays(1); // cookie validation time
                options.Cookie.Name = "CormenCloudCore";
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
            //toto dát zpìt:
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseDatabaseErrorPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            loggerFactory.AddFile("Logs/info-{Date}.log", LogLevel.Information);
            loggerFactory.AddFile("Logs/debug-{Date}.log", LogLevel.Debug);
            loggerFactory.AddFile("Logs/error-{Date}.log", LogLevel.Error);


            var conf = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            BL.RunningApp.SetConnectString(conf.GetSection("ConnectionStrings")["AppConnection"]);
            BL.RunningApp.SetFolders(conf.GetSection("Folders")["Upload"], conf.GetSection("Folders")["Temp"]);


        }
    }
}
