using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Authentication.Context;
using Northwind.Services;
using Northwind.Services.Blogging;
using Northwind.Services.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp6.Models;

namespace WebApp6.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly IMapper mapper;
        public ArticlesController(IBloggingService managementService, 
            IEmployeeManagementService employeeManagementService,
            ICustomerManagmentService customerManagmentService,
            NorthwindUsersContext context, IMapper mapper)
        {
            this.ArticleManagementService = managementService;
            this.EmployeeManagementService = employeeManagementService;
            this.mapper = mapper;
            this.UserContext = context;
            this.CustomerManagmentService = customerManagmentService;
        }
        IBloggingService ArticleManagementService { get; set; }
        NorthwindUsersContext UserContext { get; set; }

        IEmployeeManagementService EmployeeManagementService { get; set; }

        ICustomerManagmentService CustomerManagmentService { get; set; }

        // GET: ArticlesController
        public async Task<ActionResult> GetAll(int page = 1)
        {
            List<ArticleViewModel> result = new List<ArticleViewModel>();
            var offset = PagingInfo.ItemsPerPage * (page - 1);
            await foreach (var article in this.ArticleManagementService.GetArticleAsync(offset, PagingInfo.ItemsPerPage))
            {
                result.Add(this.mapper.Map<ArticleViewModel>(article));

            }
            var pagingInfo = new PagingInfo
            {
                TotalItems = result.Count,
                CurrentPage = offset / PagingInfo.ItemsPerPage,
            };

            var user = this.UserContext.Users.SingleOrDefault(x => x.Email == this.User.Identity.Name);
            ViewBag.NorthwindId = user is null ? null : user.NorthwindId;
            return View(new PaginationArticleViewModel { Collection = result, PagingInfo = pagingInfo });

        }
        // GET: ArticlesController/Details/5
        public async Task< ActionResult> Details(int id)
        {
            (bool operation, BlogArticle article) = await this.ArticleManagementService.TryGetArticleAsync(id);

            if (!operation)
            {
                return this.NotFound();
            }

            return View(this.mapper.Map<ArticleViewModel>(article));
        }

        // GET: ArticlesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ArticlesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind] BlogArticle article)
        {
            if (ModelState.IsValid)
            {
                var user = this.UserContext.Users.SingleOrDefault(x => x.Email == this.User.Identity.Name);
                var id = user is null ? 0 : int.Parse(user.NorthwindId);
                var (result, employee) = await this.EmployeeManagementService.TryGetEmployeeAsync(id);
                if (result)
                {
                    article.Author = employee;   
                }
                else
                {
                    return this.BadRequest();
                }
                await this.ArticleManagementService.CreateBlogArticleAsync(article);
                return RedirectToAction(nameof(GetAll));
            }
            return View(this.mapper.Map<ArticleViewModel>(article));
        }

        // GET: ArticlesController/Edit/5
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult> Edit(int id)
        {
            var (result, article) = await this.ArticleManagementService.TryGetArticleAsync(id);
            if (result)
            {
                return View(this.mapper.Map<ArticleViewModel>(article));
            }

            return this.BadRequest();

        }

        // POST: Test/Edit/5
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int articleId, [Bind] BlogArticle article)
        {
            if (articleId != article.ArticleId)
            {
                this.BadRequest();
            }

            if (ModelState.IsValid)
            {
                await this.ArticleManagementService.UpdateArticleAsync(articleId, article);
                return RedirectToAction(nameof(GetAll));
            }
            return View(this.mapper.Map<ArticleViewModel>(article));
        }



        // GET: ArticlesController/Delete/5
        public async Task< ActionResult> Delete(int id)
        {
            var (res, article) = await this.ArticleManagementService.TryGetArticleAsync(id);
            return res ? View(this.mapper.Map<ArticleViewModel>(article)) : this.NotFound();
        }

        // POST: ArticlesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int articleId)
        {
            var res =  await this.ArticleManagementService.DestroyArticleAsync(articleId);
            if (res)
            {
                return RedirectToAction(nameof(GetAll));
            }
            else
            {
                return this.NotFound();
            }

        }

        [HttpGet("{articleId}/comments")]
        public async Task<ActionResult> GetComments(int articleId, int page = 1)
        {
            var offset = (page - 1) * PagingInfo.ItemsPerPage;
            var comments = this.ArticleManagementService.GetArticalComments(articleId, offset, PagingInfo.ItemsPerPage);

            var result = new List<BlogCommentViewModel>();
            await foreach (var comment in comments)
            {
                result.Add(this.mapper.Map< BlogCommentViewModel >( comment));
            }

            var pagingInfo = new PagingInfo
            {
                TotalItems = result.Count,
                CurrentPage = page,
            };

            var user = this.UserContext.Users.SingleOrDefault(x => x.Email == this.User.Identity.Name);
            ViewBag.NorthwindId = user is null ? null : user.NorthwindId;
            return View(new PaginationBlogCommentViewModel { Collection = result, PagingInfo = pagingInfo, ArticleId = articleId }); ;
        }

        public async Task<ActionResult> DeleteComment(int commentId)
        {
            var (res, article) = await this.ArticleManagementService.TryGetCommentAsync(commentId);
            return res ? View(this.mapper.Map<BlogCommentViewModel>(article)) : this.NotFound();
        }

        // POST: ArticlesController/Delete/5
        [HttpPost, ActionName("DeleteComment")]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCommentConfirmed(int articleId, int commentId)
        {
            var res = await this.ArticleManagementService.RemoveComment(articleId, commentId);
            if (res)
            {
                return RedirectToAction(nameof(GetAll));
            }
            else
            {
                return this.NotFound();
            }

        }

        // GET: ArticlesController/Edit/5
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> EditComment(int id)
        {
            var (result, article) = await this.ArticleManagementService.TryGetCommentAsync(id);
            if (result)
            {
                return View(this.mapper.Map<BlogCommentViewModel>(article));
            }

            return this.BadRequest();

        }

        // POST: Test/Edit/5
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditComment(int articleId, [Bind] BlogComment comment)
        {
            if (articleId != comment.ArticleId)
            {
                this.BadRequest();
            }

            if (ModelState.IsValid)
            {
                await this.ArticleManagementService.UpdateComment(articleId, comment.CommentId, comment.Text);
                return RedirectToAction(nameof(GetAll));
            }
            return View(this.mapper.Map<BlogCommentViewModel>(comment));
        }


        // GET: ArticlesController/CreateComment
        public ActionResult CreateComment(int articleId)
        {
            BlogCommentViewModel model = new()
            {
                ArticleId = articleId,
            };
            return View(model);
        }

        // POST: ArticlesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateComment([Bind] BlogComment comment)
        {
            if (ModelState.IsValid)
            {
                var user = this.UserContext.Users.SingleOrDefault(x => x.Email == this.User.Identity.Name);
                var id = user is null ? "" : user.NorthwindId;
                var (result, customer) = await this.CustomerManagmentService.TryGetCustomerAsync(id);
                if (result)
                {
                    comment.CustomerId = customer.CustomerId;
                    comment.CommentAuthorName = customer.ContactName;
                }
                else
                {
                    return this.BadRequest();
                }
                await this.ArticleManagementService.AddComment(comment);
                return RedirectToAction(nameof(GetAll));
            }
            return View(this.mapper.Map<ArticleViewModel>(comment));
        }
    }
}
