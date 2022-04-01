using WebAppModule6.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Services.Blogging
{
    public interface IBloggingService
    {
        Task<bool> RemoveComment(int articleId, int commentId);
        Task<bool> UpdateComment(int articleId, int commentId, string text);
        Task<bool> AddComment(int articleId, CommentRequestForm comment);
        IAsyncEnumerable<BlogComment> GetArticalComments(int articleId);

        Task<bool> RemoveProductFromArticle(int articleId, int productId);

        Task<bool> AddLinkToArticleForProduct(int articleId, int productId);

        IAsyncEnumerable<ProductEntity> GetArticleProductsAsync(int articleId);

        /// <summary>
        /// Shows a list of BlogArticle using specified offset and limit for pagination.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="BlogArticle"/>.</returns>
        IAsyncEnumerable<BlogArticalFullInformationForm> GetArticleAsync(int offset, int limit);

        /// <summary>
        /// Creates a new BlogArticle.
        /// </summary>
        /// <param name="article">A <see cref="BlogArticle"/> to create.</param>
        /// <returns>An identifier of a created product category.</returns>
        Task<int> CreateBlogArticleAsync(BlogArticle article);

        /// <summary>
        /// Try to show an article with specified identifier.
        /// </summary>
        /// <param name="articleId">An article identifier.</param>
        /// <returns>Returns true if an article is returned; otherwise false.</returns>
        Task<(bool result, BlogArticalFullInformationForm article)> TryGetArticleAsync(int articleId);

        /// <summary>
        /// Destroys an existed article.
        /// </summary>
        /// <param name="id">An article identifier.</param>
        /// <returns>True if an article is destroyed; otherwise false.</returns>
        Task<bool> DestroyArticleAsync(int id);

        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="id">An article identifier.</param>
        /// <param name="article">A <see cref="BlogArticle"/>.</param>
        /// <returns>True if an article is updated; otherwise false.</returns>
        Task<bool> UpdateArticleAsync(int id, ArticleRequestForm newInfo);
    }
}
