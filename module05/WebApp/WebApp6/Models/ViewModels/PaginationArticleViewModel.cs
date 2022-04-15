using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp6.Models
{
    public class PaginationArticleViewModel
    {
        public IEnumerable<ArticleViewModel> Collection { get; set; }

        public PagingInfo PagingInfo { get; set; }

    }
}
