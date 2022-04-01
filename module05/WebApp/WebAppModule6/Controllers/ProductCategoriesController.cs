using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services;
using Northwind.Services.Products;
using WebAppModule6.Entities;

namespace NorthwindWebApp.Controllers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    public class ProductCategoriesController : Controller
    {
        public ProductCategoriesController(IProductsCategoryManagmentService managementService, IProductPictureManagementService managementPictureService)
        {
            this.ManagementService = managementService ?? throw new ArgumentNullException(nameof(managementService));
            this.ManagementPictureService = managementPictureService ?? throw new ArgumentNullException(nameof(managementPictureService));
        }

        IProductsCategoryManagmentService ManagementService { get; set; }

        IProductPictureManagementService ManagementPictureService { get; set; }

        [HttpGet]
        [HttpGet("{limit}/{offset}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryEntity))]
        public async IAsyncEnumerable<ProductCategory> GetAll(int offset = 0, int limit = 10)
        {
            await foreach (var product in this.ManagementService.GetCategoriesAsync(offset, limit))
            {
                yield return product;
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductCategory))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            (bool operation, ProductCategory category) = await this.ManagementService.TryGetCategoryAsync(id);

            if (!operation)
            {
                return this.NotFound();
            }

            return this.Ok(category);
        }

        [HttpGet("{id}/picture")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<byte[]>> GetPicture(int id)
        {
            (bool operation, byte[] bytes) = await this.ManagementPictureService.TryGetPictureAsync(id);

            if (!operation)
            {
                return this.NotFound();
            }

            return this.File(bytes[78..], "image/bmp");
        }

        [HttpDelete("{id}/picture")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> DeletePicture(int id)
        {
            var result = await this.ManagementPictureService.DestroyPictureAsync(id);

            if (!result)
            {
                return this.NotFound();
            }

            return result;
        }

        [HttpPut("{id}/picture")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<byte[]>> UpdatePicture([FromRoute]int id, IFormFile picture)
        {
            if (picture is null)
            {
                throw new ArgumentNullException(nameof(picture));
            }

            bool operation = await this.ManagementPictureService.UpdatePictureAsync(id, picture.OpenReadStream());

            if (!operation)
            {
                return this.NotFound();
            }

            return this.Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Create(ProductCategory category)
        {
            var res = await this.ManagementService.CreateCategoryAsync(category);
            var routeValues = new { id = res};
            category.CategoryId = res;
            return this.CreatedAtAction(nameof(Create), routeValues, category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await this.ManagementService.DestroyCategoryAsync(id);
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
        public async Task<IActionResult> Update(int id, ProductCategory productCategory)
        {
            var (result, _) = await this.ManagementService.TryGetCategoryAsync(id);
            if (!result)
            {
                return this.BadRequest();
            }
            var res = await this.ManagementService.UpdateCategoryAsync(id, productCategory);
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
