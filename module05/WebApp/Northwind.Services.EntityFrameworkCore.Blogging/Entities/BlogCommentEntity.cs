using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Entities
{
    public class BlogCommentEntity
    {
        [Key]
        [Column("Comment_id")]
        public int CommentId { get; set; }


        [Column("Customer_id")]
        public string CustomerId { get; set; }

        [Column("Comment_text")]
        public string Text { get; set; }

        public BlogArticleEntity Article { get; set; }
    }
}
