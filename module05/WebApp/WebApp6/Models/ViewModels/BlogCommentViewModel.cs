using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp6.Models
{
    public class BlogCommentViewModel
    {
        public int CommentId { get; set; }

        public int ArticleId { get; set; }

        public string CustomerId { get; set; }
        public string CommentAuthorName { get; set; }

        public string Text { get; set; }
    }
}
