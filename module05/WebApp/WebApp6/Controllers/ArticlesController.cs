using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Blogging;
using Northwind.Services.Employees;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using WebAppModule6.Entities;

namespace WebApp6.Controllers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    public class ArticlesController : Controller
    {
        public ArticlesController(IBloggingService managementService, IEmployeeManagementService employeeManagementService)
        {
            this.ArticleManagementService = managementService;
            this.EmployeeManagementService = employeeManagementService;
        }

        IBloggingService ArticleManagementService { get; set; }

        IEmployeeManagementService EmployeeManagementService { get; set; }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> Create(BlogArticle article)
        {
            var (employeeExist, _) = await this.EmployeeManagementService.TryGetEmployeeAsync(article.AuthorId);
            if (!employeeExist)
            {
                return this.BadRequest();
            }

            var res = await this.ArticleManagementService.CreateBlogArticleAsync(article);

            article.ArticleId = res;
            return this.CreatedAtAction(nameof(Create), new { id = res }, article);
        }

        [Authorize(Roles = "Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ArticleRequestForm updateForm)
        {
            var (articleExist, _) = await this.ArticleManagementService.TryGetArticleAsync(id);
            if (!articleExist)
            {
                return this.NotFound();
            }

            var updated = await this.ArticleManagementService.UpdateArticleAsync(id, updateForm);
            return updated ? this.Ok() : this.NotFound();
        }

        [Authorize(Roles = "Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (articleExist, _) = await this.ArticleManagementService.TryGetArticleAsync(id);
            if (!articleExist)
            {
                return this.NotFound();
            }

            var deleted = await this.ArticleManagementService.DestroyArticleAsync(id);
            return deleted ? this.Ok() : this.NotFound();
        }

        [HttpGet]
        [HttpGet("{limit}/{offset}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlogArticalFullInformationForm))]
        public async IAsyncEnumerable<BlogArticalFullInformationForm> GetAll(int offset = 0, int limit = 10)
        {
            await foreach (var article in this.ArticleManagementService.GetArticleAsync(offset, limit))
            {
                yield return article;
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlogArticle))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var (articleExist, article) = await this.ArticleManagementService.TryGetArticleAsync(id);
            return articleExist ? this.Ok(article) : this.NotFound();

        }

        [HttpGet("{id}/products")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlogArticle))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async IAsyncEnumerable<ProductEntity> GetByArticleIdAllProducts(int id)
        {
            var products = this.ArticleManagementService.GetArticleProductsAsync(id);

            await foreach (var product in products)
            {
                yield return product;
            }
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("{articleId}/products/{id}")]
        public async Task<IActionResult> AddProductsToArticle(int articleId, int id)
        {
            var result = await this.ArticleManagementService.AddLinkToArticleForProduct(articleId, id);
            if (!result)
            {
                return this.BadRequest();
            }

            return this.Ok();
        }

        [Authorize(Roles = "Employee")]
        [HttpDelete("{articleId}/products/{id}")]
        public async Task<IActionResult> RemoveProductFromArticle(int articleId, int id)
        {
            var result = await this.ArticleManagementService.RemoveProductFromArticle(articleId, id);
            if (!result)
            {
                return this.BadRequest();
            }

            return this.Ok();
        }

        [HttpGet("{id}/comments")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlogComment))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async IAsyncEnumerable<BlogComment> GetByArticleIdAllComments(int id)
        {
            var comments = this.ArticleManagementService.GetArticalComments(id);

            await foreach (var comment in comments)
            {
                yield return comment;
            }
        }

        [HttpPost("{articleId}/comments")]
        public async Task<IActionResult> AddCommentToArticle(int articleId, CommentRequestForm comment)
        {
            var result = await this.ArticleManagementService.AddComment(articleId, comment);
            if (!result)
            {
                return this.BadRequest();
            }

            return this.Ok();
        }

        [HttpPut("{articleId}/comments/{commentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateComment(int articleId, int commentId)
        {
            using StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);

            var text = await reader.ReadToEndAsync();



            if (text is null)
            {
                this.BadRequest(text);
            }

            var result = await this.ArticleManagementService.UpdateComment(articleId, commentId, text);

            return result ? this.Ok() : this.NotFound();
        }

        [HttpDelete("{articleId}/comments/{commentId}")]
        public async Task<IActionResult> RemoveCommentFromArticle(int articleId, int commentId)
        {
            var result = await this.ArticleManagementService.RemoveComment(articleId, commentId);
            if (!result)
            {
                return this.BadRequest();
            }

            return this.Ok();
        }
    }
}
