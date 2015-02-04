using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CollaboratR.Models
{
    public class RegistrationModel : BusinessObject
    {
        [Required(ErrorMessage="Please Enter a username")]
        [Display(Name="Username (What other users will see)")]
        [MinLength(4, ErrorMessage="Username must be 4 or more characters")]
        [MaxLength(32,ErrorMessage="Username must be less than 32 characters")]
        public String Username { get; set; }

        [Required(ErrorMessage="Email required")]
        [Display(Name="Email Address")]
        [MinLength(4,ErrorMessage="Please enter a valid email")]
        [MaxLength(128, ErrorMessage="Email address is too long")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public String Email { get; set; }

        [Required(ErrorMessage="Enter a password")]
        [MinLength(6,ErrorMessage="Password must be at least 6 characters")]
        [MaxLength(32, ErrorMessage="Password too long")]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Required(ErrorMessage = "Passwords must match")]
        [Compare("Password",ErrorMessage="Passwords must match")]
        [MinLength(6, ErrorMessage = "Passwords must match")]
        [MaxLength(32, ErrorMessage = "Passwords must match")]
        [DataType(DataType.Password)]
        public String ConfirmPassword { get; set; }

        public String Salt { get; set; }
        public String IpAddress { get; set; }

        [Required(ErrorMessage = "Please accept the terms of use")]
        public Boolean TermsAccepted { get; set; }

    }
}