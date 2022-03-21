using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;
using NorthwindWebApp.Entities;

namespace NorthwindWebApp.Controllers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        public ProductController(IProductManagementService managementService)
        {
            this.ManagementService = managementService;
        }

        IProductManagementService ManagementService { get; set; }

        [HttpGet]
        [HttpGet("{offset}/{limit}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        public async IAsyncEnumerable<Product> GetAll(int offset = 0, int limit = 10)
        {
            await foreach (var product in this.ManagementService.GetProductsAsync(offset, limit))
            {
                yield return product;
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            (bool operation, Product Product) = await this.ManagementService.TryGetProductAsync(id);

            if (!operation)
            {
                return this.NotFound();
            }

            return this.Ok(Product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var res = await this.ManagementService.CreateProductAsync(product);

            product.ProductId = res;
            return this.CreatedAtAction(nameof(Create), new { id = res }, product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await this.ManagementService.DestroyProductAsync(id);
            if (res)
            {
                return this.Ok(id);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            var (result, _) = await this.ManagementService.TryGetProductAsync(id);
            if (!result) 
            { 
                return this.BadRequest();
            }
            var res = await this.ManagementService.UpdateProductAsync(id, product);
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
