using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp6.Models
{
    public class ProductRequestModel
    {
        public List<string> CategoriesName { get; set; }

        public int Page { get; set; }
    }
}
