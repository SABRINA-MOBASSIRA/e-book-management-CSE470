using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }
        [Required]
        [Index("QuantityIndex")]
        [Display(Name = "Quantity")]
        public decimal? Quantity { get; set; }
  
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [Display(Name = "Unit Price")]
        public decimal? UnitPrice { get; set; }
        
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [Display(Name = "Total Price")]
        public decimal? TotalPrice { get; set; }

        [Index("OrderIDIndex")]
        [ForeignKey("Order")]
        [Display(Name = "Order No")]
        public int? OrderID { get; set; }
        [Index("ProductIDIndex")]
        [ForeignKey("Product")]
        [Display(Name = "Product")]
        public int? ProductID { get; set; }
        public string Status { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}