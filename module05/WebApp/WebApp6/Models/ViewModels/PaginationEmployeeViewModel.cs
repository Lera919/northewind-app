using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp6.Models
{
    public class PaginationEmployeeViewModel
    {
        public IEnumerable<EmployeeViewModel> Collection { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
