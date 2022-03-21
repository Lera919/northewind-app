using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Services.Blogging
{
    public class BlogArticalFullInformationForm
    {
        public int ArticleId { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime PublicationDate { get; set; }

        public int AuthorId { get; set; }

        public string AuthorName { get; set; }
    }
}
