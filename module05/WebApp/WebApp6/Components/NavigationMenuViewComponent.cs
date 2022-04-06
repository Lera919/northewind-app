using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp6.Models
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private readonly IProductsCategoryManagmentService service;

        public NavigationMenuViewComponent(IProductsCategoryManagmentService service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            List<string> result = new List<string>();
            await foreach (var category in service.GetAllCategoriesAsync())
            {
                result.Add(category.CategoryName);
            }
            return View(result.Distinct());
        }
    }

}
