using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Blogging;
using Northwind.Services.EntityFrameworkCore.Blogging.Entities;
using WebAppModule6.Context;
using WebAppModule6.Entities;

namespace Northwind.Services.EntityFrameworkCore.Blogging
{
    /// <summary>
    /// Service to work with blog.
    /// </summary>
    public class BloggingService : IBloggingService
    {
        private readonly BloggingContext context;

        private readonly NorthwindContext northwindContext;
        private readonly IMapper mapper;

        public BloggingService(BloggingContext context, NorthwindContext northwindContext, IMapper mapper)
        {
            this.context = context;
            this.northwindContext = northwindContext;
            this.mapper = mapper;
        }

        public async Task<int> CreateBlogArticleAsync(BlogArticle article)
        {
            article.PublicationDate = DateTime.Now;
            var result = await context.Articles.AddAsync(this.mapper.Map<Northwind.Services.EntityFrameworkCore.Blogging.Entities.BlogArticleEntity>(article));
            await context.SaveChangesAsync();
            return result.Entity.ArticleId;
        }

        public async Task<bool> DestroyArticleAsync(int id)
        {
            var (result, article) = await this.TryGetArticleAsync(id).ConfigureAwait(false);
            if (result)
            {
                this.context.Articles.Remove(this.mapper.Map<Northwind.Services.EntityFrameworkCore.Blogging.Entities.BlogArticleEntity>(article));
                this.context.SaveChanges();
            }

            return result;
        }

        public async Task<(bool result, BlogArticalFullInformationForm article)> TryGetArticleAsync(int articleId)
        {
            var article = await this.context.Articles.FindAsync(articleId).ConfigureAwait(false);
            if (article is null)
            {
                return (false, null);
            }

            var author = await this.northwindContext.Employees.FindAsync(article.AuthorId).ConfigureAwait(false);
            if (author is null)
            {
                return (false, null);
            }
            var fulInformation = new BlogArticalFullInformationForm
            {
                ArticleId = article.ArticleId,
                PublicationDate = article.PublicationDate,
                Text = article.Text,
                Title = article.Title,
                AuthorId = author.EmployeeId,
                AuthorName = author.FirstName,
            };
            return (article != null, fulInformation);
        }

        public async IAsyncEnumerable<BlogComment> GetArticalComments(int articleId)
        {
            var article = this.context.Articles.Include(a => a.Comments).Where(a => a.ArticleId == articleId).Single();
            var commentsInArticle = article.Comments;
            var customers = await this.northwindContext.Customers.ToListAsync();

            var result = (from comment in commentsInArticle
                          from customer in customers
                          where customer.CustomerId == comment.CustomerId
                          select comment);

            foreach(var comment in result)
            {
                var customer = customers.Where(cust => cust.CustomerId == comment.CustomerId).Single();
                yield return new BlogComment
                {
                    Text = comment.Text,
                    AuthorName = customer.ContactName,
                    CommentId = comment.CommentId,
                    ArticleName = article.Title,
                    ArticleId = article.ArticleId,
                    CustomerId = customer.CustomerId,
                };
            }


        }

        public async Task<bool> AddComment (int articleId, CommentRequestForm comment)
        {
            if (comment is null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            var article = this.context.Articles.Include(a => a.Comments).Where(a => a.ArticleId == articleId).Single();

            if (article is null)
            {
                return false;
            }


            var blogCommnet = new BlogCommentEntity
            {
                Text = comment.Text,
                CustomerId = comment.CustomerId,
            };
            article.Comments.Add(blogCommnet);
            this.context.Articles.Update(article);

            this.context.Comments.Add(blogCommnet);
            await this.context.SaveChangesAsync();

            return true;
        }
        public async IAsyncEnumerable<ProductEntity> GetArticleProductsAsync(int articleId)
        {
            var article = this.context.Articles.Include(a => a.Products).Where(a => a.ArticleId == articleId).Single();
            var productsInArticle = article.Products;
            var northwindProducts = await this.northwindContext.Products.ToListAsync();
            var result = (from product in northwindProducts
                          from articleProduct in productsInArticle
                          where product.ProductId == articleProduct.ProductId
                          select product);
            foreach (var product in result)
            {
                yield return product;
            }

        }

        public async Task<bool> AddLinkToArticleForProduct(int articleId, int productId)
        {
            var product = await this.northwindContext.Products.FindAsync(productId);
            var article = this.context.Articles.Include(a => a.Products).Where(a => a.ArticleId == articleId).Single();

            if (article is null || product is null)
            {
                return false;
            }

            
            var blogProduct = new BlogProductEntity
            {
                ProductId = product.ProductId,
                ArticleId = article.ArticleId,
            };
            article.Products.Add(blogProduct);
            this.context.Articles.Update(article);

            this.context.Products.Add(blogProduct);
            await this.context.SaveChangesAsync();
            
            return true;

        }

        public async Task<bool> RemoveProductFromArticle(int articleId, int productId)
        {
            var product = await this.northwindContext.Products.FindAsync(productId);
            var article = this.context.Articles.Include(a => a.Products).Where(a => a.ArticleId == articleId).Single();

            if (article is null || product is null)
            {
                return false;
            }
            var blogProduct = new BlogProductEntity
            {
                ProductId = product.ProductId,
                ArticleId = article.ArticleId,
            };


            this.context.Products.Remove(blogProduct);
            await this.context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UpdateArticleAsync(int id, ArticleRequestForm newInfo)
        {
            if (newInfo is null)
            {
                throw new ArgumentNullException(nameof(newInfo));
            }

            var toUpdate = await this.context.Articles.FindAsync(id).ConfigureAwait(false);
            if (toUpdate is null)
            {
                return false;
            }
            else
            {
                toUpdate.Text = newInfo.Text;
                toUpdate.Title = newInfo.Title;
                toUpdate.PublicationDate = DateTime.Now;
                this.context.Articles.Update(toUpdate);
                await this.context.SaveChangesAsync();
                return true;
            }
        }

        public async IAsyncEnumerable<BlogArticalFullInformationForm> GetArticleAsync(int offset, int limit)
        {
            await foreach (var article in this.context.Articles.OrderBy(article => article.ArticleId).Skip(offset).Take(limit).AsAsyncEnumerable())
            {
                var author = await this.northwindContext.Employees.FindAsync(article.AuthorId);
                if (author != null)
                {

                    yield return new BlogArticalFullInformationForm
                    {
                        ArticleId = article.ArticleId,
                        PublicationDate = article.PublicationDate,
                        Text = article.Text,
                        Title = article.Title,
                        AuthorId = author.EmployeeId,
                        AuthorName = author.FirstName,
                    };
                }
            }
        }

        public async Task<bool> RemoveComment(int articleId, int commentId)
        {

            var blogComment = await this.context.Comments.FindAsync(commentId);
            if (blogComment is null)
            {
                return false;
            }



            this.context.Comments.Remove(blogComment);
            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateComment(int articleId, int commentId, string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var blogComment = await this.context.Comments.FindAsync(commentId);
            if (blogComment is null)
            {
                return false;
            }

            
            blogComment.Text = text;

            this.context.Comments.Update(blogComment);
            await this.context.SaveChangesAsync();

            return true;
        }
    }
}
