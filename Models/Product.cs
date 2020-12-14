using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        [Index("NameIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [Display(Name = "Price")]
        public decimal? Price { get; set; }
        [Display(Name = "Photo")]
        public string ImagePath { get; set; }
        //public virtual ICollection<SalesQuotationDetail> SalesQuotationDetails { get; set; }
    }
}