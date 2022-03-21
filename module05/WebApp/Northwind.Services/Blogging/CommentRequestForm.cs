using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Services.Blogging
{
    public class CommentRequestForm
    {
        public string CustomerId { get; set; }

        public string Text { get; set; }
    }
}
