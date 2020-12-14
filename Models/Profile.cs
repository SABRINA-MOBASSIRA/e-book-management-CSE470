using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class Profile
    {
        [Key]
        public int ProfileID { get; set; }
        [Index("FirstNameIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(150)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Index("LastNameIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(150)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }
        [Display(Name = "OfficePhone")]
        public string OfficePhone { get; set; }
        [Display(Name = "Fax")]
        public string Fax { get; set; }
        [Display(Name = "Primary Street")]
        public string PrimaryStreet { get; set; }
        [Display(Name = "Primary City")]
        public string PrimaryCity { get; set; }
        [Display(Name = "Primary State")]
        public string PrimaryState { get; set; }
        [Display(Name = "Primary Postal Code")]
        public string PrimaryPostalCode { get; set; }
        [Display(Name = "Primary Country")]
        public string PrimaryCountry { get; set; }
        [Display(Name = "Other Street")]
        public string OtherStreet { get; set; }
        [Display(Name = "Other City")]
        public string OtherCity { get; set; }
        [Display(Name = "Other State")]
        public string OtherState { get; set; }
        [Display(Name = "Other Postal Code")]
        public string OtherPostalCode { get; set; }
        [Display(Name = "Other Country")]
        public string OtherCountry { get; set; }
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Signature")]
        public string ImagePath { get; set; }
        [Index("UserIDIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(300)]
        [Display(Name = "User ID")]
        public string UserID { get; set; }

        [Display(Name = "Is Active")]
        public bool? IsActive { get; set; }
        public string Status { get; set; }
        [Display(Name = "Create By")]
        public int? CreateBy { get; set; }
        [Display(Name = "Create On")]
        public DateTime? CreateOn { get; set; }
        [Display(Name = "Update By")]
        public int? UpdateBy { get; set; }
        [Display(Name = "Update On")]
        public DateTime? UpdateOn { get; set; }
        [Display(Name = "Is Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Delete By")]
        public int? DeleteBy { get; set; }
        [Display(Name = "Delete On")]
        public DateTime? DeleteOn { get; set; }
    }
}