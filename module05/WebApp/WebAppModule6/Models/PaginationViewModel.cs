using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class PaginationViewModel<T>
    {
        public IEnumerable<T> Collection { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
