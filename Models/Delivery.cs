using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class Delivery
    {
        [Key]
        public int DeliveryID { get; set; }
        [Required]
        [Index("ChallanNoIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [Display(Name = "Challan No")]
        public string ChallanNo { get; set; }
        [Required]
        [Index("DeliveryDateIndex")]
        [Display(Name = "Delivery Date")]
        public DateTime DeliveryDate { get; set; }
        [Index("QuantityIndex")]
        [Display(Name = "Quantity")]
        public decimal? Quantity { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [Display(Name = "Total Price")]
        public decimal? TotalPrice { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Index("CustomerIDIndex")]
        [ForeignKey("Customer")]
        [Display(Name = "Customer")]
        public int? CustomerID { get; set; }

        [Required]
        [Index("OrderIDIndex")]
        [ForeignKey("Order")]
        [Display(Name = "Order No")]
        public int OrderID { get; set; }

        [Index("IsActiveIndex")]
        [Display(Name = "Is Active")]
        public bool? IsActive { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Order Order { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}