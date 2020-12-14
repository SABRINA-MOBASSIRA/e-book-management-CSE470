using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        [Required]
        [Index("NameIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Index("OfficePhoneIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Index("BillingStreetIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(300)]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Index("EmailIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(150)]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        //public virtual ICollection<SalesQuotation> SalesQuotations { get; set; }
        //public virtual ICollection<SalesInvoice> SalesInvoices { get; set; }
        //public virtual ICollection<DeliveryChallan> DeliveryChallans { get; set; }
    }
}