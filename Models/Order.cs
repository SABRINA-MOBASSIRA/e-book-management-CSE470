using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        [Required]
        [Index("QuotationNoIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Order No")]
        public string OrderNo { get; set; }
        [Required]
        [Index("DateIndex")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        //[DataType(DataType.Currency)]
        //[Column(TypeName = "money")]
        //[Display(Name = "Instalation Cost")]
        //public decimal? InstalationCost { get; set; }
        //[DataType(DataType.Currency)]
        //[Column(TypeName = "money")]
        //[Display(Name = "Other Cost")]
        //public decimal? OtherCost { get; set; }
        //[DataType(DataType.Currency)]
        //[Column(TypeName = "money")]
        //[Display(Name = "VAT")]
        //public decimal? VAT { get; set; }
        //[DataType(DataType.Currency)]
        //[Column(TypeName = "money")]
        //[Display(Name = "ATI")]
        //public decimal? ATI { get; set; }
        //[DataType(DataType.Currency)]
        //[Column(TypeName = "money")]
        //[Display(Name = "Discount")]
        //public decimal? Discount { get; set; }
        //[Index("ToBePaidIndex")]
        //[Display(Name = "To Be Paid")]
        //public int? ToBePaid { get; set; }
        //[Index("WillBePaidIndex")]
        //[Display(Name = "Will Be Paid")]
        //public int? WillBePaid { get; set; }
        //[Display(Name = "Delivery Day")]
        //public int? DeliveryDay { get; set; }
        //[Display(Name = "Warranty Day")]
        //public int? WarrantyDay { get; set; }
        //[Display(Name = "Subject")]
        //public string Subject { get; set; }
        //[Display(Name = "Remarks")]
        //public string Remarks { get; set; }
        //[Index("UserIDIndex")]
        //[Column(TypeName = "VARCHAR")]
        //[StringLength(300)]
        //[Display(Name = "User ID")]
        //public string UserID { get; set; }

        [Index("IsActiveIndex")]
        [Display(Name = "Is Active")]
        public bool? IsActive { get; set; }

        [Index("CustomerIDIndex")]
        [ForeignKey("Customer")]
        [Display(Name = "Customer")]
        public int? CustomerID { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Delivery> Deliveries { get; set; }
    }
}