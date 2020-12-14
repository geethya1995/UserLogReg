using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TestProj.Models
{
    // Applying meta data validations in 'UserMetaData' class to the main class
    [MetadataType(typeof(UserMetaData))]
    public partial class User
    {
        /* Additonal field which is not in the model 'User'. 
         * The field will be displayed in the web page, 
         * but the value will not get saved in to the database. */
        public String ConfirmPassword { get; set; }
    }

    // Adding validations for the fields
    public class UserMetaData 
    {
        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        public String FName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        public String LName { get; set; }

        [Display(Name = "Email ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "E-mail is mandatory")]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }

        [Display(Name = "Date of Birth")]
        [Required(AllowEmptyStrings = true)]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/mm/yyyy}")]
        public DateTime DOB { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is mandatory")]
        [MinLength(8, ErrorMessage = "At least 8 characters should be in your password"]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password confirmation is mandatory")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords you typed do not match. Please re-type your password again")]
        public String ConfirmPassword { get; set; }

    }
}