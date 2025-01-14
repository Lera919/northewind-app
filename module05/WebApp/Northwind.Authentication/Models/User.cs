﻿using System;

namespace Northwind.Authentication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public Role Role { get; set; }

        public string NorthwindId { get; set; }
    }
}
