using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using WebApp6.Models;

namespace WebApp6.Controllers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    public class Products : Controller
    {
        private readonly IMapper mapper;
        public Products(IProductManagementService managementService, IProductsCategoryManagmentService categoryService, IMapper mapper)
        {
            this.mapper = mapper;
            this.ProductManagementService = managementService;
            this.CategoryManagementService = categoryService;
        }

        IProductManagementService ProductManagementService { get; set; }
        IProductsCategoryManagmentService CategoryManagementService { get; set; }

        //[HttpGet]
        //[HttpGet("{offset}/{limit}")]
        //[HttpGet("{offset}/{limit}/{category}")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        //public async Task<IActionResult> GetAll(int offset = 0, int limit = 10)
        //{
        //    List<ProductViewModel> result = new List<ProductViewModel>();
        //    await foreach (var product in this.ProductManagementService.GetProductsAsync(offset, limit))
        //    {
        //        result.Add(this.mapper.Map<ProductViewModel>(product));

        //    }
        //    var pagingInfo = new PagingInfo
        //    {
        //        TotalItems = result.Count,
        //        CurrentPage = offset / PagingInfo.ItemsPerPage,
        //    };
        //    return View(new PaginationProductViewModel { Collection = result, PagingInfo = pagingInfo }); ;
        //}

        [HttpGet]
        //[HttpGet("{offset}/{limit}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        public async Task<IActionResult> GetAll(int page = 1, string category = null)
        {
            List<ProductViewModel> result = new List<ProductViewModel>();
            await foreach (var product in this.ProductManagementService.LookupProductsByCategoryNameAsync(category is null ? (string[]) null : new string[] { category}))
            {
                result.Add(this.mapper.Map<ProductViewModel>(product));

            }
            var pagingInfo = new PagingInfo
            {
                TotalItems = result.Count,
                CurrentPage = page,
            };
            result = result.Skip((page - 1) * PagingInfo.ItemsPerPage).Take(PagingInfo.ItemsPerPage).ToList();
            return View(new PaginationProductViewModel { Collection = result, PagingInfo = pagingInfo, CurrentCategory = category }); ;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            (bool operation, Product Product) = await this.ProductManagementService.TryGetProductAsync(id);

            if (!operation)
            {
                return this.NotFound();
            }

            return this.Ok(Product);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var res = await this.ProductManagementService.CreateProductAsync(product);

            product.ProductId = res;
            return this.CreatedAtAction(nameof(Create), new { id = res }, product);
        }

        [Authorize(Roles = "Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await this.ProductManagementService.DestroyProductAsync(id);
            if (res)
            {
                return this.Ok(id);
            }
            else
            {
                return this.NotFound();
            }
        }

        [Authorize(Roles = "Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            var (result, _) = await this.ProductManagementService.TryGetProductAsync(id);
            if (!result)
            {
                return this.BadRequest();
            }
            var res = await this.ProductManagementService.UpdateProductAsync(id, product);
            if (res)
            {
                return this.Ok(id);
            }
            else
            {
                return this.NotFound();
            }
        }
    }
}
