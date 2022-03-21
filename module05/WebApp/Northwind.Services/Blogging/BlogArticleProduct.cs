using NorthwindWebApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Services.Blogging
{
    public class BlogArticleProduct
    {
        public int BloggArticleId { get; set; }

        public Product Products { get; set; }
    }
}
