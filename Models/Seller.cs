using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class Seller
    {
        [Key]
        public int SellerID { get; set; }
        [Required]
        [Index("NameIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Index("PhoneIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Index("EmailIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(150)]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }
    }
}