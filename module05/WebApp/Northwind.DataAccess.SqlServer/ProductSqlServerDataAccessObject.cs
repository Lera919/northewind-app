// <copyright file="ProductSqlServerDataAccessObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.DataAccess.Products
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind products.
    /// </summary>
    public sealed class ProductSqlServerDataAccessObject : IProductDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public ProductSqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<int> InsertProductAsync(ProductTransferObject product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            using var sqlCommand = new SqlCommand("InsertProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(product, sqlCommand);

            await this.connection.OpenAsync().ConfigureAwait(false);
            var res = (int)await sqlCommand.ExecuteScalarAsync().ConfigureAwait(false);

            return res;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProductAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            using var sqlCommand = new SqlCommand("DeleteProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
            sqlCommand.Parameters.Add("@productID", SqlDbType.Int);
            sqlCommand.Parameters["@productID"].Value = productId;
            await this.connection.OpenAsync().ConfigureAwait(false);
            var res = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

            return res > 0;
        }

        /// <inheritdoc/>
        public async Task<ProductTransferObject> FindProductAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            const string productIdParameter = "@productId";

            using var command = new SqlCommand("FindProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(productIdParameter, SqlDbType.Int);
            command.Parameters[productIdParameter].Value = productId;

            await this.connection.OpenAsync().ConfigureAwait(false);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            if (!reader.HasRows)
            {
                throw new ProductNotFoundException(productId);
            }

            await reader.ReadAsync().ConfigureAwait(false);
            var res = CreateProduct(reader);

            await this.connection.CloseAsync().ConfigureAwait(false);
            return res;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ProductTransferObject> SelectProductsAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            using var command = new SqlCommand("SelectOffsetProducts", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add("@limit", SqlDbType.Int);
            command.Parameters["@limit"].Value = limit;
            command.Parameters.Add("@offset", SqlDbType.Int);
            command.Parameters["@offset"].Value = offset;

            await this.connection.OpenAsync().ConfigureAwait(false);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                yield return CreateProduct(reader);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductTransferObject> SelectProductsByNameAsync(ICollection<string> productNames)
        {
            if (productNames == null)
            {
                throw new ArgumentNullException(nameof(productNames));
            }

            if (productNames.Count < 1)
            {
                throw new ArgumentException("Collection is empty.", nameof(productNames));
            }

            using var command = new SqlCommand("SelectProductsByName", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add("@name", SqlDbType.NVarChar, 100);
            command.Parameters["@name"].Value = string.Join("', '", productNames);
            await this.connection.OpenAsync().ConfigureAwait(false);
            await this.connection.OpenAsync().ConfigureAwait(false);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                yield return CreateProduct(reader);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(ProductTransferObject product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            using var sqlCommand = new SqlCommand("UpdateProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(product, sqlCommand);
            sqlCommand.Parameters.Add("@productID", SqlDbType.Int);
            sqlCommand.Parameters["@productID"].Value = product.Id;
            await this.connection.OpenAsync().ConfigureAwait(false);

            return await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductTransferObject> SelectProductByCategoryAsync(ICollection<int> collectionOfCategoryId)
        {
            if (collectionOfCategoryId == null)
            {
                throw new ArgumentNullException(nameof(collectionOfCategoryId));
            }

            var whereInClause = string.Join(
                "','",
                collectionOfCategoryId.Select(
                id => string.Format(CultureInfo.InvariantCulture, "{0:d}", id)).ToArray());

            using var command = new SqlCommand("FindByCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add("@categoryId", SqlDbType.NVarChar, 100);
            command.Parameters["@categoryId"].Value = whereInClause;

            await this.connection.OpenAsync().ConfigureAwait(false);

            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                yield return CreateProduct(reader);
            }
        }

        private static ProductTransferObject CreateProduct(SqlDataReader reader)
        {
            var id = (int)reader["ProductID"];
            var name = (string)reader["ProductName"];

            const string supplierIdColumnName = "SupplierID";
            int? supplierId;

            if (reader[supplierIdColumnName] != DBNull.Value)
            {
                supplierId = (int)reader[supplierIdColumnName];
            }
            else
            {
                supplierId = null;
            }

            const string categoryIdColumnName = "CategoryID";
            int? categoryId;

            if (reader[categoryIdColumnName] != DBNull.Value)
            {
                categoryId = (int)reader[categoryIdColumnName];
            }
            else
            {
                categoryId = null;
            }

            const string quantityPerUnitColumnName = "QuantityPerUnit";
            string quantityPerUnit;

            if (reader[quantityPerUnitColumnName] != DBNull.Value)
            {
                quantityPerUnit = (string)reader[quantityPerUnitColumnName];
            }
            else
            {
                quantityPerUnit = null;
            }

            const string unitPriceColumnName = "UnitPrice";
            decimal? unitPrice;

            if (reader[unitPriceColumnName] != DBNull.Value)
            {
                unitPrice = (decimal)reader[unitPriceColumnName];
            }
            else
            {
                unitPrice = null;
            }

            const string unitsInStockColumnName = "UnitsInStock";
            short? unitsInStock;

            if (reader[unitsInStockColumnName] != DBNull.Value)
            {
                unitsInStock = (short)reader[unitsInStockColumnName];
            }
            else
            {
                unitsInStock = null;
            }

            const string unitsOnOrderColumnName = "UnitsOnOrder";
            short? unitsOnOrder;

            if (reader[unitsOnOrderColumnName] != DBNull.Value)
            {
                unitsOnOrder = (short)reader[unitsOnOrderColumnName];
            }
            else
            {
                unitsOnOrder = null;
            }

            const string reorderLevelColumnName = "ReorderLevel";
            short? reorderLevel;

            if (reader[reorderLevelColumnName] != DBNull.Value)
            {
                reorderLevel = (short)reader[reorderLevelColumnName];
            }
            else
            {
                reorderLevel = null;
            }

            const string discontinuedColumnName = "Discontinued";
            bool discontinued = (bool)reader[discontinuedColumnName];

            return new ProductTransferObject
            {
                Id = id,
                Name = name,
                SupplierId = supplierId,
                CategoryId = categoryId,
                QuantityPerUnit = quantityPerUnit,
                UnitPrice = unitPrice,
                UnitsInStock = unitsInStock,
                UnitsOnOrder = unitsOnOrder,
                ReorderLevel = reorderLevel,
                Discontinued = discontinued,
            };
        }

        private static void AddSqlParameters(ProductTransferObject product, SqlCommand command)
        {
            const string productNameParameter = "@productName";
            command.Parameters.Add(productNameParameter, SqlDbType.NVarChar, 40);
            command.Parameters[productNameParameter].Value = product.Name;

            const string supplierIdParameter = "@supplierId";
            command.Parameters.Add(supplierIdParameter, SqlDbType.Int);
            command.Parameters[supplierIdParameter].IsNullable = true;

            if (product.SupplierId != null)
            {
                command.Parameters[supplierIdParameter].Value = product.SupplierId;
            }
            else
            {
                command.Parameters[supplierIdParameter].Value = DBNull.Value;
            }

            const string categoryIdParameter = "@categoryId";
            command.Parameters.Add(categoryIdParameter, SqlDbType.Int);
            command.Parameters[categoryIdParameter].IsNullable = true;

            if (product.CategoryId != null)
            {
                command.Parameters[categoryIdParameter].Value = product.CategoryId;
            }
            else
            {
                command.Parameters[categoryIdParameter].Value = DBNull.Value;
            }

            const string quantityPerUnitParameter = "@quantityPerUnit";
            command.Parameters.Add(quantityPerUnitParameter, SqlDbType.NVarChar, 20);
            command.Parameters[quantityPerUnitParameter].IsNullable = true;

            if (product.QuantityPerUnit != null)
            {
                command.Parameters[quantityPerUnitParameter].Value = product.QuantityPerUnit;
            }
            else
            {
                command.Parameters[quantityPerUnitParameter].Value = DBNull.Value;
            }

            const string unitPriceParameter = "@unitPrice";
            command.Parameters.Add(unitPriceParameter, SqlDbType.Money);
            command.Parameters[unitPriceParameter].IsNullable = true;

            if (product.UnitPrice != null)
            {
                command.Parameters[unitPriceParameter].Value = product.UnitPrice;
            }
            else
            {
                command.Parameters[unitPriceParameter].Value = DBNull.Value;
            }

            const string unitsInStockParameter = "@unitsInStock";
            command.Parameters.Add(unitsInStockParameter, SqlDbType.SmallInt);
            command.Parameters[unitsInStockParameter].IsNullable = true;

            if (product.UnitsInStock != null)
            {
                command.Parameters[unitsInStockParameter].Value = product.UnitsInStock;
            }
            else
            {
                command.Parameters[unitsInStockParameter].Value = DBNull.Value;
            }

            const string unitsOnOrderParameter = "@unitsOnOrder";
            command.Parameters.Add(unitsOnOrderParameter, SqlDbType.SmallInt);
            command.Parameters[unitsOnOrderParameter].IsNullable = true;

            if (product.UnitsOnOrder != null)
            {
                command.Parameters[unitsOnOrderParameter].Value = product.UnitsOnOrder;
            }
            else
            {
                command.Parameters[unitsOnOrderParameter].Value = DBNull.Value;
            }

            const string reorderLevelParameter = "@reorderLevel";
            command.Parameters.Add(reorderLevelParameter, SqlDbType.SmallInt);
            command.Parameters[reorderLevelParameter].IsNullable = true;

            if (product.ReorderLevel != null)
            {
                command.Parameters[reorderLevelParameter].Value = product.ReorderLevel;
            }
            else
            {
                command.Parameters[reorderLevelParameter].Value = DBNull.Value;
            }

            const string discontinuedParameter = "@discontinued";
            command.Parameters.Add(discontinuedParameter, SqlDbType.Bit);
            command.Parameters[discontinuedParameter].Value = product.Discontinued;
        }
    }
}
