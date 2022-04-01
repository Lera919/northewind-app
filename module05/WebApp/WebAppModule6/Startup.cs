using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Northwind.DataAccess;
using Northwind.Services;
using Northwind.Services.Blogging;
using Northwind.Services.DataAccess;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore;
using Northwind.Services.EntityFrameworkCore.Blogging;
using Northwind.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppModule6.Context;

namespace NorthwindWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<MemoryDataCreator>();
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
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
