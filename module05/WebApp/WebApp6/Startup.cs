using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Northwind.Authentication.Context;
using Northwind.DataAccess;
using Northwind.Services;
using Northwind.Services.Blogging;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore;
using Northwind.Services.EntityFrameworkCore.Blogging;
using Northwind.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppModule6.Context;

namespace WebApp6
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
            services.AddControllersWithViews();
            services.AddScoped<MemoryDataCreator>();
            services.AddDbContext<NorthwindUsersContext>(options => options.UseSqlServer(Configuration.GetConnectionString("USERS")));

            // установка конфигурации подключения
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });
            services.AddDbContext<NorthwindContext>(opt =>
                 opt.UseSqlServer("data source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Database=Northwind;"));
            services.AddScoped<IMapper>(mapper => new MapperConfiguration
            (m => { m.AddProfile(new MappingProfiler()); }).CreateMapper());
            services.AddScoped<NorthwindDataAccessFactory>(factory => new SqlServerDataAccessFactory(new System.Data.SqlClient.SqlConnection(this.Configuration.GetConnectionString("connection"))));
            services.AddScoped<IProductManagementService, ProductManagementService>();
            services.AddScoped<IProductsCategoryManagmentService, ProductCategoriesManagmentService>();
            services.AddScoped<IProductPictureManagementService, PictureManagmentService>();
            services.AddScoped<IPhotoManagamentService, PhotoManagmentService>();
            services.AddScoped<IEmployeeManagementService, EmployeeManagementService>();
            services.AddScoped<IBloggingService, BloggingService>();
            services.AddDbContext<BloggingContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("NORTHWIND_BLOGGING")).
                UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MemoryDataCreator creator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();    // аутентификация
            app.UseAuthorization();     // авторизация

            creator.SeedDate();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                     name: "default",
                     pattern: "{controller=Employee}/{action=GetAll}/{id?}");
            });
        }
    }
}
