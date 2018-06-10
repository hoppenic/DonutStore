using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using donutstore.Models;
using System.ComponentModel.DataAnnotations;

namespace donutstore.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(8,ErrorMessage ="Must be no less than eight characers")]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }







    }
}
