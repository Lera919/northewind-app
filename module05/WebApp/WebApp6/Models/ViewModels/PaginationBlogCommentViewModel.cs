using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp6.Models
{
    public class PaginationBlogCommentViewModel
    {
        public IEnumerable<BlogCommentViewModel> Collection { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public int ArticleId { get; set; }

    }
}
