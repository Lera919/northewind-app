using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Services.Blogging
{
    public class BlogComment
    {
        public int CommentId { get; set; }

        public int ArticleId { get; set; }

        public string CustomerId { get; set; }
        public string AuthorName { get; set; }

        public string ArticleName { get; set; }

        public string Text { get; set; }
    }
}
