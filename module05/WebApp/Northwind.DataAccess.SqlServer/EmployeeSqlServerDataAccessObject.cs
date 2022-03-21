// <copyright file="EmployeeSqlServerDataAccessObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.DataAccess.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Northwind.DataAccess.Products;

    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind products.
    /// </summary>
    public sealed class EmployeeSqlServerDataAccessObject : IEmployeeDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeSqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public EmployeeSqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteEmploteeAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            using var sqlCommand = new SqlCommand("DeleteEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
            sqlCommand.Parameters.Add("@employeeID", SqlDbType.Int);
            sqlCommand.Parameters["@employeeID"].Value = employeeId;
            await this.connection.OpenAsync().ConfigureAwait(false);
            var res = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

            return res > 0;
        }

        /// <inheritdoc/>
        public async Task<EmployeeTransferObject> FindEmploteeAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            const string idParameter = "@employeeId";

            using var command = new SqlCommand("FindEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(idParameter, SqlDbType.Int);
            command.Parameters[idParameter].Value = employeeId;

            await this.connection.OpenAsync().ConfigureAwait(false);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            if (!reader.HasRows)
            {
                throw new EmployeeNoFoundException(employeeId);
            }

            await reader.ReadAsync().ConfigureAwait(false);
            var res = CreateEmployee(reader);

            await this.connection.CloseAsync().ConfigureAwait(false);
            return res;
        }

        /// <inheritdoc/>
        public async Task<int> InsertEmploteeAsync(EmployeeTransferObject employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            using var sqlCommand = new SqlCommand("InsertEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(employee, sqlCommand);
            await this.connection.OpenAsync().ConfigureAwait(false);

            var res = (int)await sqlCommand.ExecuteScalarAsync().ConfigureAwait(false);

            return res;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<EmployeeTransferObject> SelectEmploteeAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            using var command = new SqlCommand("SelectEmployeeOffset", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add("@limit", SqlDbType.Int);
            command.Parameters["@limit"].Value = limit;
            command.Parameters.Add("@offset", SqlDbType.Int);
            command.Parameters["@offset"].Value = offset;

            await this.connection.OpenAsync().ConfigureAwait(false);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    yield return CreateEmployee(reader);
                }
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<EmployeeTransferObject> SelectEmploteeByNameAsync(ICollection<string> names)
        {
            if (names == null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            if (names.Count < 1)
            {
                throw new ArgumentException("Collection is empty.", nameof(names));
            }

            using var command = new SqlCommand("SelectEmployeeByName", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add("@name", SqlDbType.NVarChar, 100);
            command.Parameters["@name"].Value = string.Join("', '", names);
            await this.connection.OpenAsync().ConfigureAwait(false);
            await this.connection.OpenAsync().ConfigureAwait(false);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                yield return CreateEmployee(reader);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmploteeAsync(EmployeeTransferObject employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            using var sqlCommand = new SqlCommand("UpdateEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(employee, sqlCommand);

            sqlCommand.Parameters.Add("@employeeID", SqlDbType.Int);
            sqlCommand.Parameters["@employeeID"].Value = employee.Id;

            await this.connection.OpenAsync().ConfigureAwait(false);

            return await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
        }

        private static void AddSqlParameters(EmployeeTransferObject employee, SqlCommand command)
        {
            const string employeeLastNameParameter = "@lastName";
            command.Parameters.Add(employeeLastNameParameter, SqlDbType.NVarChar, 20);
            command.Parameters[employeeLastNameParameter].Value = employee.LastName;

            const string employeeFirstNameParameter = "@firstName";
            command.Parameters.Add(employeeFirstNameParameter, SqlDbType.NVarChar, 10);
            command.Parameters[employeeFirstNameParameter].Value = employee.FirstName;

            const string titleParameter = "@title";
            command.Parameters.Add(titleParameter, SqlDbType.NVarChar, 30);
            command.Parameters[titleParameter].IsNullable = true;
            command.Parameters[titleParameter].Value = (object)employee.Title ?? DBNull.Value;

            const string titleOfCourtesyParameter = "@titleOfCourtesy";
            command.Parameters.Add(titleOfCourtesyParameter, SqlDbType.NVarChar, 25);
            command.Parameters[titleOfCourtesyParameter].IsNullable = true;
            command.Parameters[titleOfCourtesyParameter].Value = (object)employee.TitleOfCourtesy ?? DBNull.Value;

            const string birthDateParameter = "@birthDate";
            command.Parameters.Add(birthDateParameter, SqlDbType.DateTime, 8);
            command.Parameters[birthDateParameter].IsNullable = true;
            command.Parameters[birthDateParameter].Value = (object)employee.BirthDate ?? DBNull.Value;

            const string hireDateParameter = "@hireDate";
            command.Parameters.Add(hireDateParameter, SqlDbType.DateTime, 8);
            command.Parameters[hireDateParameter].IsNullable = true;
            command.Parameters[hireDateParameter].Value = (object)employee.HireDate ?? DBNull.Value;

            const string addressParameter = "@address";
            command.Parameters.Add(addressParameter, SqlDbType.NVarChar, 60);
            command.Parameters[addressParameter].IsNullable = true;
            command.Parameters[addressParameter].Value = (object)employee.Address ?? DBNull.Value;

            const string cityParameter = "@city";
            command.Parameters.Add(cityParameter, SqlDbType.NVarChar, 15);
            command.Parameters[cityParameter].IsNullable = true;
            command.Parameters[cityParameter].Value = (object)employee.City ?? DBNull.Value;

            const string regionParameter = "@region";
            command.Parameters.Add(regionParameter, SqlDbType.NVarChar, 15);
            command.Parameters[regionParameter].IsNullable = true;
            command.Parameters[regionParameter].Value = (object)employee.Region ?? DBNull.Value;

            const string postalCodeParameter = "@postalCode";
            command.Parameters.Add(postalCodeParameter, SqlDbType.NVarChar, 10);
            command.Parameters[postalCodeParameter].IsNullable = true;
            command.Parameters[postalCodeParameter].Value = (object)employee.PostalCode ?? DBNull.Value;

            const string countryParameter = "@country";
            command.Parameters.Add(countryParameter, SqlDbType.NVarChar, 15);
            command.Parameters[countryParameter].IsNullable = true;
            command.Parameters[countryParameter].Value = (object)employee.Country ?? DBNull.Value;

            command.Parameters.Add("@homePhone", SqlDbType.NVarChar, 24);
            command.Parameters["@homePhone"].IsNullable = true;
            command.Parameters["@homePhone"].Value = (object)employee.HomePhone ?? DBNull.Value;

            const string extensionParameter = "@extension";
            command.Parameters.Add(extensionParameter, SqlDbType.NVarChar, 4);
            command.Parameters[extensionParameter].IsNullable = true;
            command.Parameters[extensionParameter].Value = (object)employee.Extension ?? DBNull.Value;

            const string photoParameter = "@photo";
            command.Parameters.Add(photoParameter, SqlDbType.Image);
            command.Parameters[photoParameter].IsNullable = true;
            command.Parameters[photoParameter].Value = (object)employee.Photo ?? DBNull.Value;

            const string notesParameter = "@notes";
            command.Parameters.Add(notesParameter, SqlDbType.NText);
            command.Parameters[notesParameter].IsNullable = true;
            command.Parameters[notesParameter].Value = (object)employee.Notes ?? DBNull.Value;

            const string reportsToParameter = "@reportsTo";
            command.Parameters.Add(reportsToParameter, SqlDbType.Int);
            command.Parameters[reportsToParameter].IsNullable = true;
            command.Parameters[reportsToParameter].Value = (object)employee.ReportsTo ?? DBNull.Value;

            const string photoPathParameter = "@photoPath";
            command.Parameters.Add(photoPathParameter, SqlDbType.NVarChar);
            command.Parameters[photoPathParameter].IsNullable = true;
            command.Parameters[photoPathParameter].Value = (object)employee.PhotoPath ?? DBNull.Value;
        }

        private static EmployeeTransferObject CreateEmployee(SqlDataReader reader)
        {
            var id = (int)reader["EmployeeID"];
            var lastName = (string)reader["LastName"];
            var firstName = (string)reader["FirstName"];
            var title = reader["Title"] != DBNull.Value ? (string)reader["Title"] : null;
            var titleOfCourtesy = reader["TitleOfCourtesy"] != DBNull.Value ? (string)reader["TitleOfCourtesy"] : null;
            DateTime? birthDate = reader["BirthDate"] != DBNull.Value ? (DateTime)reader["BirthDate"] : (DateTime?)null;
            DateTime? hireDate = reader["HireDate"] != DBNull.Value ? (DateTime)reader["HireDate"] : (DateTime?)null;
            var address = reader["Address"] != DBNull.Value ? (string)reader["Address"] : null;
            var city = reader["City"] != DBNull.Value ? (string)reader["City"] : null;
            var region = reader["Region"] != DBNull.Value ? (string)reader["Region"] : null;
            var postalCode = reader["PostalCode"] != DBNull.Value ? (string)reader["PostalCode"] : null;
            var country = reader["Country"] != DBNull.Value ? (string)reader["Country"] : null;
            var homePhone = reader["HomePhone"] != DBNull.Value ? (string)reader["HomePhone"] : null;
            var extension = reader["Extension"] != DBNull.Value ? (string)reader["Extension"] : null;
            var photo = reader["Photo"] != DBNull.Value ? (byte[])reader["Photo"] : null;
            var notes = reader["Notes"] != DBNull.Value ? (string)reader["Notes"] : null;
            int? reportsTo = reader["ReportsTo"] != DBNull.Value ? (int)reader["ReportsTo"] : (int?)null;
            var photoPath = reader["PhotoPath"] != DBNull.Value ? (string)reader["PhotoPath"] : null;

            return new EmployeeTransferObject
            {
                Id = id,
                LastName = lastName,
                FirstName = firstName,
                Title = title,
                TitleOfCourtesy = titleOfCourtesy,
                BirthDate = birthDate,
                HireDate = hireDate,
                Address = address,
                City = city,
                Region = region,
                PostalCode = postalCode,
                Country = country,
                HomePhone = homePhone,
                Extension = extension,
                Photo = photo,
                Notes = notes,
                ReportsTo = reportsTo,
                PhotoPath = photoPath,
            };
        }
    }
}
