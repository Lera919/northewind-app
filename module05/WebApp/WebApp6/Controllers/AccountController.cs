using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Northwind.Authentication.Context;
using Northwind.Authentication.Models;
using Northwind.Services;
using Northwind.Services.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp6.Models;
using WebAppModule6.Context;
using WebAppModule6.Entities;

namespace WebApp6.Controllers
{
    public class AccountController : Controller
    {
        private readonly NorthwindUsersContext context;
        private readonly IEmployeeManagementService employeeManagmentService;
        private readonly ICustomerManagmentService customerManagmentService;
        public AccountController(NorthwindUsersContext context, IEmployeeManagementService employeeManagmentService, ICustomerManagmentService customerManagmentService)
        {
            this.customerManagmentService = customerManagmentService;
            this.context = context;
            this.employeeManagmentService = employeeManagmentService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            RegistrationViewModel registrationViewModel = new RegistrationViewModel
            {
                Roles = GetSelectListItems(this.context.Roles),
            };
           

            return View(registrationViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                string identifier = model.Email.Substring(0, model.Email.LastIndexOf('@'));
                if (user is null)
                {
                    
                    Role userRole = await context.Roles.FirstOrDefaultAsync(r => r.Id.ToString() == model.Role);
                    
                    var (isFound, northeindUserId) = await TryGetNorthwindId(identifier, userRole.Name);
                    if (!isFound)
                    {
                        northeindUserId = await CreateNewNorthwindUser(identifier, userRole.Name);
                    }
                    user = new User
                    {
                        Email = model.Email,
                        Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                        NorthwindId = northeindUserId.Trim(),
                    };



                    if (userRole != null)
                    {
                        user.Role = userRole;
                    }

                    context.Users.Add(user);

                    await context.SaveChangesAsync();

                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid login or password.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User user = await context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == viewModel.Email/* && u.Password == model.Password*/);

                if (user != null && BCrypt.Net.BCrypt.Verify(viewModel.Password, user.Password))
                {
                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid login or password.");
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Logoff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name),
                // new Claim("NorthwindId", user.NorthwindId)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<Role> elements)
        {
            // Create an empty list to hold result of the operation
            var selectList = new List<SelectListItem>();

            // For each string in the 'elements' variable, create a new SelectListItem object
            // that has both its Value and Text properties set to a particular value.
            // This will result in MVC rendering each item as:
            //     <option value="State Name">State Name</option>
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.Id.ToString(),
                    Text = element.Name
                });
            }

            return selectList;
        }

        private async Task<string> CreateNewNorthwindUser(string identifier, string role)
        {
            string northwindId;
            if (role.Equals("Employee", StringComparison.InvariantCultureIgnoreCase))
            {
                northwindId =  (await this.employeeManagmentService.CreateEmployeeAsync(new Employee
                {
                    FirstName = identifier,
                    LastName = "",
                }
                    )).ToString();
            }
            else
            {
                northwindId = await (this.customerManagmentService.CreateCustomerAsync(new Customer
                {
                    CustomerId = new Bogus.Randomizer().String(5).ToUpperInvariant(),
                   ContactName = identifier,
                   CompanyName = "",
                }
                    ));
            }
          return northwindId;

        }

        private async Task<(bool result, string id)> TryGetNorthwindId (string identifier, string role)
        {
            if (role.Equals("Employee", StringComparison.InvariantCultureIgnoreCase))
            {
                var (_, user) = await this.employeeManagmentService.TryGetByNameAsync(identifier);
                return user is null ? (false, null) : (true, user.EmployeeId.ToString());
            }
            else
            {
                var (_, user) = await this.customerManagmentService.TryGetCustomerAsync(identifier);
                return user is null ? (false, null) : (true, user.CustomerId);
            }

        }
    }
}
