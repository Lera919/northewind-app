using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFrameworkCore.Blogging.Entities;
using WebAppModule6.Entities;

namespace Northwind.Services.EntityFrameworkCore.Blogging
{
    public class BloggingContext : DbContext
    {
        public BloggingContext()
        {
            //atabase.EnsureCreated();
        }
        
       
        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        {
        }

        public DbSet<BlogArticleEntity> Articles { get; set; } = null!;

        public DbSet<BlogProductEntity> Products { get; set; } = null!;

        public DbSet<BlogCommentEntity> Comments { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogArticleEntity>().HasKey(u => u.ArticleId);
            modelBuilder.Entity<BlogArticleEntity>().HasMany(u => u.Products);
            modelBuilder.Entity<BlogArticleEntity>().HasMany(u => u.Comments);
            modelBuilder.Entity<BlogCommentEntity>().HasKey(u => u.CommentId);
            modelBuilder.Entity<BlogCommentEntity>().HasOne(u => u.Article).WithMany(u=> u.Comments).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BlogCommentEntity>().Property(u => u.Text).HasColumnType("ntext");
            modelBuilder.Entity<BlogProductEntity>().HasKey(p => new { p.ProductId, p.ArticleId });
            modelBuilder.Ignore<CustomerCustomerDemo>();
            modelBuilder.Ignore<CustomerDemographic>();
            modelBuilder.Ignore<CustomerDemographic>();
            modelBuilder.Ignore<EmployeeTerritory>();
            modelBuilder.Ignore<OrderDetail>();
        }
    }
}
