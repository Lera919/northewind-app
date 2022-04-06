// <copyright file="ProductCategorySqlServerDataAccessObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.DataAccess.Products
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind product categories.
    /// </summary>
    public sealed class ProductCategorySqlServerDataAccessObject : IProductCategoryDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategorySqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public ProductCategorySqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        async Task<int> IProductCategoryDataAccessObject.InsertProductCategoryAsync(ProductCategoryTransferObject productCategory)
        {
            using var sqlCommand = new SqlCommand("InsertCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(productCategory, sqlCommand);
            await this.connection.OpenAsync().ConfigureAwait(false);
            var res = (int)await sqlCommand.ExecuteScalarAsync().ConfigureAwait(false);

            return res;
        }

        /// <inheritdoc/>
        async Task<bool> IProductCategoryDataAccessObject.DeleteProductCategoryAsync(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            using var sqlCommand = new SqlCommand("DeleteCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
            sqlCommand.Parameters.Add("@categoryID", SqlDbType.Int);
            sqlCommand.Parameters["@categoryID"].Value = productCategoryId;
            await this.connection.OpenAsync().ConfigureAwait(false);
            var res = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

            return res > 0;
        }

        /// <inheritdoc/>
        async Task<bool> IProductCategoryDataAccessObject.UpdateProductCategoryAsync(ProductCategoryTransferObject productCategory)
        {
            if (productCategory == null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            using var sqlCommand = new SqlCommand("UpdateProductCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(productCategory, sqlCommand);
            sqlCommand.Parameters.Add("@categoryID", SqlDbType.Int);
            sqlCommand.Parameters["@categoryID"].Value = productCategory.Id;

            await this.connection.OpenAsync().ConfigureAwait(false);

            return await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
        }

        /// <inheritdoc/>
        async Task<ProductCategoryTransferObject> IProductCategoryDataAccessObject.FindProductCategoryAsync(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            const string productIdParameter = "@categoryId";

            using var command = new SqlCommand("FindProductCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add(productIdParameter, SqlDbType.Int);
            command.Parameters[productIdParameter].Value = productCategoryId;

            await this.connection.OpenAsync().ConfigureAwait(false);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            if (!reader.HasRows)
            {
                throw new ProductNotFoundException(productCategoryId);
            }

            await reader.ReadAsync().ConfigureAwait(false);
            var res = CreateProductCategory(reader);

            await this.connection.CloseAsync().ConfigureAwait(false);
            return res;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategoryTransferObject> SelectProductCategoriesAsync()
        {
            using var command = new SqlCommand("SelectProductCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            await this.connection.OpenAsync().ConfigureAwait(false);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    yield return CreateProductCategory(reader);
                }
            }
        }

        /// <inheritdoc/>
        async IAsyncEnumerable<ProductCategoryTransferObject> IProductCategoryDataAccessObject.SelectProductCategoriesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            using var command = new SqlCommand("SelectProductCategoryOffset", this.connection)
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
                    yield return CreateProductCategory(reader);
                }
            }
        }

        /// <inheritdoc/>
        async IAsyncEnumerable<ProductCategoryTransferObject> IProductCategoryDataAccessObject.SelectProductCategoriesByNameAsync(ICollection<string> productCategoryNames)
        {
            if (productCategoryNames == null)
            {
                throw new ArgumentNullException(nameof(productCategoryNames));
            }

            if (productCategoryNames.Count < 1)
            {
                throw new ArgumentException("Collection is empty.", nameof(productCategoryNames));
            }

            using var command = new SqlCommand("SelectProductCategoryByName", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.Add("@name", SqlDbType.NVarChar, 100);
            command.Parameters["@name"].Value = string.Join("', '", productCategoryNames);
            await this.connection.OpenAsync().ConfigureAwait(false);
            await this.connection.OpenAsync().ConfigureAwait(false);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                yield return CreateProductCategory(reader);
            }
        }

        private static ProductCategoryTransferObject CreateProductCategory(SqlDataReader reader)
        {
            var id = (int)reader["CategoryID"];
            var name = (string)reader["CategoryName"];

            const string descriptionColumnName = "Description";
            string description = null;

            if (reader[descriptionColumnName] != DBNull.Value)
            {
                description = (string)reader["Description"];
            }

            const string pictureColumnName = "Picture";
            byte[] picture = null;

            if (reader[pictureColumnName] != DBNull.Value)
            {
                picture = (byte[])reader["Picture"];
            }

            return new ProductCategoryTransferObject
            {
                Id = id,
                Name = name,
                Description = description,
                Picture = picture,
            };
        }

        private static void AddSqlParameters(ProductCategoryTransferObject productCategory, SqlCommand command)
        {
            const string categoryNameParameter = "@categoryName";
            command.Parameters.Add(categoryNameParameter, SqlDbType.NVarChar, 15);
            command.Parameters[categoryNameParameter].Value = productCategory.Name;

            const string descriptionParameter = "@description";
            command.Parameters.Add(descriptionParameter, SqlDbType.NText);
            command.Parameters[descriptionParameter].IsNullable = true;

            if (productCategory.Description != null)
            {
                command.Parameters[descriptionParameter].Value = productCategory.Description;
            }
            else
            {
                command.Parameters[descriptionParameter].Value = DBNull.Value;
            }

            const string pictureParameter = "@picture";
            command.Parameters.Add(pictureParameter, SqlDbType.Image);
            command.Parameters[pictureParameter].IsNullable = true;

            if (productCategory.Picture != null)
            {
                command.Parameters[pictureParameter].Value = productCategory.Picture;
            }
            else
            {
                command.Parameters[pictureParameter].Value = DBNull.Value;
            }
        }

    }
}
