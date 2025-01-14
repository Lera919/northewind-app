﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppModule6.Entities;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Entities
{
    public class BlogArticleEntity
    {
        [Key]
        [Column("Article_id")]
        public int ArticleId { get; set; }

        [Column("Article_Title")]
        public string Title { get; set; }

        [Column("Article_Text")]
        public string Text { get; set; }

        [Column("Publication_Date")]
        public DateTime PublicationDate { get; set; }

        [Column("Employee_id")]
        public int AuthorId { get; set; }

        [Column("Blog_Products")]
        public ICollection<BlogProductEntity> Products { get; set; }

        [Column("Blog_Commentss")]
        public ICollection<BlogCommentEntity> Comments { get; set; }
    }
}
