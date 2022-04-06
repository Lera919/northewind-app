using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services;
using Northwind.Services.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp6.Models;

namespace WebApp6.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IMapper mapper;

        public EmployeesController(IMapper mapper, IEmployeeManagementService managementService, 
            IPhotoManagamentService managementPhotoService)
        {
            this.mapper = mapper;
            this.ManagementService = managementService ?? throw new ArgumentNullException(nameof(managementService));
            this.ManagementPhotoService = managementPhotoService ?? throw new ArgumentNullException(nameof(managementPhotoService));

        }


        IEmployeeManagementService ManagementService { get; set; }

        IPhotoManagamentService ManagementPhotoService { get; set; }

        // GET: Test
        public async Task<ActionResult> GetAll(int offset = 0, int limit = 10)
        {
            List<EmployeeViewModel> result = new List<EmployeeViewModel>();
            await foreach (var employee in this.ManagementService.GetEmployeeAsync(offset, limit))
            {
                result.Add(this.mapper.Map<EmployeeViewModel>(employee));

            }
            var pagingInfo = new PagingInfo
            {
                TotalItems = result.Count,
                CurrentPage = offset / PagingInfo.ItemsPerPage,
            };
            return View(new PaginationEmployeeViewModel { Collection = result, PagingInfo = pagingInfo }); ;
        }

        // GET: Test/Details/5
        public async Task<ActionResult> Details(int id)
        {
            (bool operation, Employee employee) = await this.ManagementService.TryGetEmployeeAsync(id);

            if (!operation)
            {
                return this.NotFound();
            }

            return View(this.mapper.Map<EmployeeViewModel>(employee));
        }

        // GET: Test/Create
        [Authorize(Roles = "Employee")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Test/Create
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind]Employee employee)
        {
            if (ModelState.IsValid)
            {
                await this.ManagementService.CreateEmployeeAsync(employee);
                return RedirectToAction(nameof(GetAll));
            }
            return View(this.mapper.Map<EmployeeViewModel>(employee));
        }

        // GET: Test/Edit/5
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult> Edit(int id)
        {
            var (result, employee) = await this.ManagementService.TryGetEmployeeAsync(id);
            if (result)
            {
                return View(this.mapper.Map<EmployeeViewModel>(employee));
            } 
            
            return this.BadRequest();
            
        }

        // POST: Test/Edit/5
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int employeeId, [Bind] Employee employee)
        {
            if(employeeId!= employee.EmployeeId)
            {
                this.BadRequest();
            }

            if (ModelState.IsValid)
            {
                await this.ManagementService.UpdateEmployeeAsync(employeeId, employee);
                return RedirectToAction(nameof(GetAll));
            }
            return View(this.mapper.Map<EmployeeViewModel>(employee));
        }


        //[HttpGet("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int id)
        {
            var (res, employee) = await this.ManagementService.TryGetEmployeeAsync(id);
            return res ? View(this.mapper.Map<EmployeeViewModel>(employee)) : this.NotFound();
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int employeeId)
        {
            var res = await this.ManagementService.DestroyEmployeeAsync(employeeId);
            if (res)
            {
                return RedirectToAction(nameof(GetAll));
            }
            else
            {
                return this.NotFound();
            }

        }

        public async Task<ActionResult<byte[]>> GetPhoto(int id)
        {
            (bool operation, byte[] bytes) = await this.ManagementPhotoService.TryGetPhotoAsync(id);

            if (!operation)
            {
                return this.NotFound();
            }

            return this.File(bytes[78..], "image/bmp");
        }

        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<byte[]>> UpdatePhoto(int id, IFormFile picture)
        {
            bool operation = await this.ManagementPhotoService.UpdatePhotoAsync(id, picture.OpenReadStream());

            if (!operation)
            {
                return this.NotFound();
            }

            return this.Ok();
        }
    }
}
