using Microsoft.EntityFrameworkCore;
using Northwind.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Authentication.Context
{
    public class NorthwindUsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public NorthwindUsersContext(DbContextOptions<NorthwindUsersContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(new Role[]
            {
                new Role { Id = 1, Name = "Customer" },
                new Role { Id = 2, Name = "Employee" }
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
