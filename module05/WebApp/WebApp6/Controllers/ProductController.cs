using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp6.Models;

namespace WebApp6.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IMapper mapper;
        public ProductController(IProductManagementService managementService, IProductsCategoryManagmentService categoryService, IMapper mapper)
        {
            this.mapper = mapper;
            this.ProductManagementService = managementService;
            this.CategoryManagementService = categoryService;
        }

        IProductManagementService ProductManagementService { get; set; }
        IProductsCategoryManagmentService CategoryManagementService { get; set; }
        // GET: ProductController
        public async Task<IActionResult> GetAll(int page = 1, string category = null)
        {
            List<ProductViewModel> result = new List<ProductViewModel>();
            await foreach (var product in this.ProductManagementService.LookupProductsByCategoryNameAsync(category is null ? (string[])null : new string[] { category }))
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

        // GET: ProductController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            (bool operation, Product product) = await this.ProductManagementService.TryGetProductAsync(id);

            if (!operation)
            {
                return this.NotFound();
            }

            return View(this.mapper.Map<ProductViewModel>(product));
        }

        // GET: ProductController/Create
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult> Create()
        {
            ProductViewModel productViewModel = new ProductViewModel
            {
                Categories = await GetSelectListItems(this.CategoryManagementService.GetAllCategoriesAsync()),
            };


            return View(productViewModel);
        }

        // POST: ProductController/Create
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await this.ProductManagementService.CreateProductAsync(product);
                return RedirectToAction(nameof(GetAll));
            }
            return View(this.mapper.Map<EmployeeViewModel>(product));
        }

        // GET: ProductController/Edit/5
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult> Edit(int id)
        {
            var (result, product) = await this.ProductManagementService.TryGetProductAsync(id);
            if (result)
            {
                return View(this.mapper.Map<EmployeeViewModel>(product));
            }

            return this.BadRequest();
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int productId, [Bind] Product product)
        {
            if (productId != product.ProductId)
            {
                this.BadRequest();
            }

            if (ModelState.IsValid)
            {
                await this.ProductManagementService.UpdateProductAsync(productId, product);
                return RedirectToAction(nameof(GetAll));
            }
            return View(this.mapper.Map<ProductViewModel>(product));
        }

        // GET: ProductController/Delete/5
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult> Delete(int id)
        {
            var (res, product) = await this.ProductManagementService.TryGetProductAsync(id);
            return res ? View(this.mapper.Map<ProductViewModel>(product)) : this.NotFound();
        }

        // POST: ProductController/Delete/5
        [Authorize(Roles = "Employee")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int productId)
        {
            var res = await this.ProductManagementService.DestroyProductAsync(productId);
            if (res)
            {
                return RedirectToAction(nameof(GetAll));
            }
            else
            {
                return this.NotFound();
            }

        }

        private async Task<IEnumerable<SelectListItem>> GetSelectListItems(IAsyncEnumerable<ProductCategory> elements)

        {
            // Create an empty list to hold result of the operation
            var selectList = new List<SelectListItem>();

            // For each string in the 'elements' variable, create a new SelectListItem object
            // that has both its Value and Text properties set to a particular value.
            // This will result in MVC rendering each item as:
            //     <option value="State Name">State Name</option>
            await foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.CategoryId.ToString(),
                    Text = element.CategoryName
                });
            }

            return selectList;
        }
    }
}
