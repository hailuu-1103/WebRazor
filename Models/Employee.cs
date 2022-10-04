﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebRazor.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Accounts = new HashSet<Account>();
            Orders = new HashSet<Order>();
        }

        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        public int? DepartmentId { get; set; }
        public string? Title { get; set; }
        public string? TitleOfCourtesy { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Address { get; set; }

        public virtual Department? Department { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
