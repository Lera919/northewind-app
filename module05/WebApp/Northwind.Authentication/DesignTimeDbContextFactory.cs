using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Northwind.Authentication.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Authentication
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<NorthwindUsersContext>
    {
        public NorthwindUsersContext CreateDbContext(string[] args)
        {
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            var builder = new DbContextOptionsBuilder<NorthwindUsersContext>();

            var connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Users;Integrated Security=True";

            builder.UseSqlServer(connectionString);

            return new NorthwindUsersContext(builder.Options);
        }
    }
}
