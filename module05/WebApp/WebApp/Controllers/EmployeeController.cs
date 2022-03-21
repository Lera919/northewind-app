using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services;
using Northwind.Services.Employees;
using Northwind.Services.Products;
using NorthwindWebApp.Entities;

namespace NorthwindWebApp.Controllers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        public EmployeeController(IEmployeeManagementService managementService, IPhotoManagamentService managementPictureService)
        {
            this.ManagementService = managementService ?? throw new ArgumentNullException(nameof(managementService));
            this.ManagementPhotoService = managementPictureService ?? throw new ArgumentNullException(nameof(managementPictureService));
        }

        IEmployeeManagementService ManagementService { get; set; }

        IPhotoManagamentService ManagementPhotoService { get; set; }

        [HttpGet]
        [HttpGet("{limit}/{offset}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Employee))]
        public async IAsyncEnumerable<Employee> GetAll(int offset = 0, int limit = 10)
        {
            await foreach (var employee in this.ManagementService.GetEmployeeAsync(offset, limit))
            {
                yield return employee;
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            (bool operation, Employee employee) = await this.ManagementService.TryGetEmployeeAsync(id);

            if (!operation)
            {
                return this.NotFound();
            }

            return this.Ok(employee);
        }

        [HttpGet("{id}/photo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<byte[]>> GetPhoto(int id)
        {
            (bool operation, byte[] bytes) = await this.ManagementPhotoService.TryGetPhotoAsync(id);

            if (!operation)
            {
                return this.NotFound();
            }

            return this.File(bytes[78..], "image/bmp");
        }

        [HttpDelete("{id}/photo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> DeletePhoto(int id)
        {
            var result = await this.ManagementPhotoService.DestroyPhotoAsync(id);

            if (!result)
            {
                return this.NotFound();
            }

            return result;
        }

        [HttpPut("{id}/photo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<byte[]>> UpdatePhoto(int id, IFormFile picture)
        {
            bool operation = await this.ManagementPhotoService.UpdatePhotoAsync(id, picture.OpenReadStream());

            if (!operation)
            {
                return this.NotFound();
            }

            return this.Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            var res = await this.ManagementService.CreateEmployeeAsync(employee);

            employee.EmployeeId = res;
            return this.CreatedAtAction(nameof(Create), new { id = res }, employee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await this.ManagementService.DestroyEmployeeAsync(id);
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
        public async Task<IActionResult> Update(int id, Employee employee)
        {
            var (result, _) = await this.ManagementService.TryGetEmployeeAsync(id);
            if (!result)
            {
                return this.BadRequest();
            }
            var res = await this.ManagementService.UpdateEmployeeAsync(id, employee);
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
