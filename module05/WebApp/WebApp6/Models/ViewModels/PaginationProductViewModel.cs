using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp6.Models
{
    public class PaginationProductViewModel
    {
        public IEnumerable<ProductViewModel> Collection { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public string CurrentCategory { get; set; }


    }
}
