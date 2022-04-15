using Northwind.Services.Employees;
using System;

namespace Northwind.Services.Blogging
{
    public class BlogArticle
    {
        public int ArticleId { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime PublicationDate { get; set; }

        public Employee Author { get; set; }
    }
}
