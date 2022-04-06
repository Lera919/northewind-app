using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp6.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Please, input name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please, input name")]
        public string FirstName { get; set; }
        public byte[] Photo { get; set; }

    }
}

