using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace Northwind.Services.EntityFrameworkCore.Blogging
{
    public class DesignTimeBloggingContextFactory : IDesignTimeDbContextFactory<BloggingContext>
    {
        public BloggingContext CreateDbContext(string[] args)
        {
            const string connectionStringName = "NORTHWIND_BLOGGING";
            const string connectioStringPrefix = "SQLCONNSTR_";


            var connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=NorthwindBlogging;Integrated Security=True";

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception($"{connectioStringPrefix}{connectionStringName} environment variable is not set.");
            }

            Console.WriteLine($"Using {connectioStringPrefix}{connectionStringName} environment variable as a connection string.");

            var builderOptions = new DbContextOptionsBuilder<BloggingContext>().UseSqlServer(connectionString).Options;
            return new BloggingContext(builderOptions);
        }
    }

}
