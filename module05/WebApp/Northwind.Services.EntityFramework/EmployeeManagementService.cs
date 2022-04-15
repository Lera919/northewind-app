// <copyright file="EmployeeManagementService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.Services.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Bogus;
    using Microsoft.EntityFrameworkCore;
    using Northwind.DataAccess;
    using Northwind.Services;
    using Northwind.Services.EntityFrameworkCore.Blogging;
    using WebAppModule6.Context;
    using WebAppModule6.Entities;

    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly NorthwindContext context;
        private readonly BloggingContext bloggingContext;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeManagementService"/> class.
        /// </summary>
        /// <param name="contex">Northwind context.</param>
        /// <param name="bloggingContext">Blogging context.</param>
        public EmployeeManagementService(NorthwindContext contex, BloggingContext bloggingContext, IMapper mapper)
        {
            this.context = contex;
            this.bloggingContext = bloggingContext;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            var createdEmployee = this.context.Employees.Add(this.mapper.Map<EmployeeEntity>(employee)).Entity;
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            var createdEmployeeid = createdEmployee.EmployeeId;
            return createdEmployeeid;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyEmployeeAsync(int employeeId)
        {
            var employee = await this.context.Employees.FindAsync(employeeId).ConfigureAwait(false);
            var result = employee is not null;
            if (result)
            {
                this.context.EmployeeTerritories.RemoveRange(this.context.EmployeeTerritories.Where(terr => terr.EmployeeId == employeeId));
                this.context.Employees.Remove(this.mapper.Map<EmployeeEntity>(employee));
                var employeesArticles = this.bloggingContext.Articles.Where(article => article.AuthorId == employeeId);
                this.bloggingContext.RemoveRange(employeesArticles);
                await this.context.SaveChangesAsync().ConfigureAwait(false);
                await this.bloggingContext.SaveChangesAsync().ConfigureAwait(false);
            }

            return result;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Employee> GetEmployeeAsync(int offset, int limit)
        {
            await foreach (var employee in this.context.Employees.Skip(offset).Take(limit).OrderBy(em => em.EmployeeId).AsAsyncEnumerable())
            {
                yield return this.mapper.Map<Employee>(employee);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Employee> LookupEmployeeByNameAsync(IList<string> names)
        {
            await foreach (var employee in this.context.Employees.Where(em => names.Contains(em.FirstName)).OrderBy(em => em.EmployeeId).AsAsyncEnumerable())
            {
                yield return this.mapper.Map<Employee>(employee);
            }
        }

        public async Task<(bool, Employee)> TryGetByNameAsync(string name)
        {
            var employee = this.mapper.Map<Employee>(this.context.Employees.SingleOrDefault(x => x.FirstName == name));
            return (employee is not null, employee);
        }


        /// <inheritdoc/>
        public async Task<(bool result, Employee employee)> TryGetEmployeeAsync(int employeeId)
        {
            var employee = await this.context.Employees.FindAsync(employeeId).ConfigureAwait(false);
            return (employee != null, this.mapper.Map<Employee>(employee));
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            this.context.Employees.Update(this.mapper.Map<EmployeeEntity>(employee));
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            return true;

        }
    }
}
