using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }
        [Required]
        [Index("InvoiceNoIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }
        [Index("InvoiceDateIndex")]
        [Display(Name = "Invoice Date")]
        public DateTime? InvoiceDate { get; set; }
        [Index("TotalAmountIndex")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [Display(Name = "otal Amount")]
        public decimal? TotalAmount { get; set; }
        public int? CustomerID { get; set; }
        [Required]
        [Index("DeliveryIDIndex")]
        [ForeignKey("Delivery")]
        [Display(Name = "Delivery Challan")]
        public int DeliveryID { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        public string Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Delivery Delivery { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}