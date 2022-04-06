using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp6.Infrastructure;
using WebApp6.Models;

namespace WebApp6.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductManagementService service;

        public CartController(IProductManagementService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl)
        {
            return View(new CartViewModel
            {
                Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart(),
                ReturnUrl = returnUrl ?? "/"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(int productId, string returnUrl)
        {
            var (_,product) = await service.TryGetProductAsync(productId);
            var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            cart.AddItem(product, 1);
            HttpContext.Session.SetJson("cart", cart);
            return View(new CartViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }
    }
}
