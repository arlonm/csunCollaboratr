using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CollaboratR.Models
{
    public class LoginModel : BusinessObject
    {
        [Required(ErrorMessage="Enter username or email")]
        [Display(Name="Username or Email")]
        public String LoginName { get; set; }

        [Required(ErrorMessage="Enter your password")]
        [Display(Name="Password")]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        public Boolean RememberMe { get; set; }

    }
}