using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Entities
{
    public class BlogProductEntity
    {
        [Column("Product_ID")]
        public int ProductId { get; set; }

        [Column("Article_ID")]
        public int ArticleId { get; set; }
    }
}
