using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess.Employees;
using Northwind.DataAccess.Products;
using Northwind.Services.Blogging;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore.Blogging.Entities;
using Northwind.Services.Products;
using WebApp.Models;
using WebAppModule6.Entities;

namespace NorthwindWebApp
{
    public class MappingProfiler : Profile
    {
        public MappingProfiler()
        {
            this.CreateMap<Employee, EmployeeViewModel>();
            this.CreateMap<EmployeeEntity, EmployeeTransferObject>();
            this.CreateMap<EmployeeTransferObject, EmployeeEntity>();

           // this.CreateMap<Product, ProductViewModel>();
            this.CreateMap<Product, ProductTransferObject>();
            this.CreateMap<ProductTransferObject, Product>();
            this.CreateMap<ProductCategory, ProductCategoryTransferObject>();
            this.CreateMap<ProductCategoryTransferObject, ProductCategory>();
            this.CreateMap<Product, ProductEntity>();
            this.CreateMap<ProductEntity, Product>();
            this.CreateMap<ProductCategory, CategoryEntity>();
            this.CreateMap<CategoryEntity, ProductCategory>();
            this.CreateMap<EmployeeEntity, Employee>();
            this.CreateMap<Employee, EmployeeEntity>();
            this.CreateMap<ProductEntity, ProductTransferObject>();
            this.CreateMap<ProductTransferObject, ProductEntity>();
            this.CreateMap<CategoryEntity, ProductCategoryTransferObject>();
            this.CreateMap<ProductCategoryTransferObject, CategoryEntity>();

            this.CreateMap<BlogArticleEntity, BlogArticle>();
            this.CreateMap<BlogArticle, BlogArticleEntity>();
        }
    }
}
