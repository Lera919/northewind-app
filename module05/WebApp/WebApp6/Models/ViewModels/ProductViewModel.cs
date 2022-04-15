using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp6.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }
        [Required]
        public string CategoryName { get; set; }

        // This property will hold all available states for selection
        public IEnumerable<SelectListItem> Categories { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }
        [Required]
        public short UnitsInStock { get; set; }
    
        public bool Discontinued { get; set; }
    }
}
