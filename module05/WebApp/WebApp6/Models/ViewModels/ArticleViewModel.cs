using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp6.Models
{
    public class ArticleViewModel
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime PublicationDate { get; set; }

        public string AuthorId { get; set; }
        public string AuthorName { get; set; }

        public ICollection<ProductViewModel> Products { get; set; }
    }
}
