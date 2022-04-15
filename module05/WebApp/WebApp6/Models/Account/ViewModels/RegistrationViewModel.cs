using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Authentication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp6.Models
{
    public class RegistrationViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Incorrect password")]
        public string ConfirmPassword { get; set; }

        // This property will hold a state, selected by user
        [Required]
        [Display(Name = "SelectedRoleId")]
        public string Role { get; set; }



        // This property will hold all available states for selection
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
