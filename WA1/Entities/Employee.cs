using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WA1.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }

        [Required]  
        public int DeptId { get; set; }

        public Department Department { get; set; }
    }

}

