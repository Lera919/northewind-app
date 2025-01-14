﻿using AutoMapper;
using Northwind.DataAccess.Employees;
using Northwind.DataAccess.Products;
using Northwind.Services;
using Northwind.Services.Blogging;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore.Blogging.Entities;
using Northwind.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp6.Models;
using WebAppModule6.Entities;

namespace WebApp6
{
    public class MappingProfiler : Profile
    {
        public MappingProfiler()
        {
            this.CreateMap<Employee, EmployeeViewModel>();
            this.CreateMap<EmployeeEntity, EmployeeTransferObject>();
            this.CreateMap<EmployeeTransferObject, EmployeeEntity>();

            this.CreateMap<Product, ProductViewModel>();
            this.CreateMap<Product, ProductTransferObject>();
            this.CreateMap<ProductTransferObject, Product>();
            this.CreateMap<ProductCategory, ProductCategoryTransferObject>();
            this.CreateMap<ProductCategoryTransferObject, ProductCategory>();
            this.CreateMap<Product, ProductEntity>();
            this.CreateMap<ProductEntity, Product>().ForMember(x => x.CategoryName,
          x => x.MapFrom(m => m.Category.CategoryName));
            this.CreateMap<ProductCategory, CategoryEntity>();
            this.CreateMap<CategoryEntity, ProductCategory>();
            this.CreateMap<EmployeeEntity, Employee>();
            this.CreateMap<Employee, EmployeeEntity>();
            this.CreateMap<ProductEntity, ProductTransferObject>();
            this.CreateMap<ProductTransferObject, ProductEntity>();
            this.CreateMap<CategoryEntity, ProductCategoryTransferObject>();
            this.CreateMap<ProductCategoryTransferObject, CategoryEntity>();

            this.CreateMap<CustomerEntity, Customer>();
            this.CreateMap<Customer, CustomerEntity>();

            this.CreateMap<BlogArticleEntity, BlogArticle>();
            this.CreateMap<BlogArticle, BlogArticleEntity>().ForMember(x => x.AuthorId, x => x.MapFrom(m => m.Author.EmployeeId));
            this.CreateMap<BlogArticle, ArticleViewModel>()
                .ForMember(x => x.AuthorName, x => x.MapFrom(m => m.Author.FirstName)).
                ForMember(x => x.AuthorId, x => x.MapFrom(m=> m.Author.EmployeeId));

            this.CreateMap<BlogComment, BlogCommentViewModel>();

        }
    }
}
