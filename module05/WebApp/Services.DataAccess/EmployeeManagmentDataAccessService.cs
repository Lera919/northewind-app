using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.DataAccess.Employees;
using Northwind.Services.Employees;
using WebAppModule6.Entities;

namespace Northwind.Services.DataAccess
{
    public class EmployeeManagmentDataAccessService : IEmployeeManagementService
    {
        private readonly IMapper mapper;
        public EmployeeManagmentDataAccessService(NorthwindDataAccessFactory factory, IMapper mapper)
        {
            this.mapper = mapper;
            this.Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        private NorthwindDataAccessFactory Factory { get; set; }

        public async Task<int> CreateEmployeeAsync(Employee employee)
        => await this.Factory.GetEmployeeDataAccessObject().InsertEmploteeAsync(this.mapper.Map<EmployeeTransferObject>(employee));

        public async Task<bool> DestroyEmployeeAsync(int employeeId)
        => await this.Factory.GetEmployeeDataAccessObject().DeleteEmploteeAsync(employeeId);

        public async IAsyncEnumerable<Employee> GetEmployeeAsync(int offset, int limit)
        {
            await foreach (var employee in this.Factory.GetEmployeeDataAccessObject().SelectEmploteeAsync(offset, limit))
            {
                yield return this.mapper.Map<Employee>(employee);
            }
        }

        public async IAsyncEnumerable<Employee> LookupEmployeeByNameAsync(IList<string> names)
        {
            await foreach (var employee in this.Factory.GetEmployeeDataAccessObject().SelectEmploteeByNameAsync(names))
            {
                yield return this.mapper.Map<Employee>(employee);
            }
        }

        public Task<(bool, Employee)> TryGetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool result, Employee employee)> TryGetEmployeeAsync(int employeeId)
        {
            var employee = await this.Factory.GetEmployeeDataAccessObject().FindEmploteeAsync(employeeId);
            var result = employee is null ? null : this.mapper.Map<Employee>(employee);
            return (result is not null, result);
        }

        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            employee.EmployeeId = employeeId;
            return await this.Factory.GetEmployeeDataAccessObject().UpdateEmploteeAsync(this.mapper.Map<EmployeeTransferObject>(employee));
        }
    }
}
