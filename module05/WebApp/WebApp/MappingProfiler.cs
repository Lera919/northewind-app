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
using NorthwindWebApp.Entities;

namespace NorthwindWebApp
{
    public class MappingProfiler : Profile
    {
        public MappingProfiler()
        {
            this.CreateMap<Employee, EmployeeTransferObject>();
            this.CreateMap<EmployeeTransferObject, Employee>();
            this.CreateMap<Product, ProductTransferObject>();
            this.CreateMap<ProductTransferObject, Product>();
            this.CreateMap<Category, ProductCategoryTransferObject>();
            this.CreateMap<ProductCategoryTransferObject, Category>();
            
            this.CreateMap<BlogArticleEntity, BlogArticle>();
            this.CreateMap<BlogArticle, BlogArticleEntity>();
        }
    }
}
