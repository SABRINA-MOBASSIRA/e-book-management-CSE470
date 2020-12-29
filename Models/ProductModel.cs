using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class ProductModel
    {
        [Key]
        public int ProductModelID { get; set; }
        [Required]
        [Index("ModelNoIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Display(Name = "Model No")]
        public string ModelNo{ get; set; }
        [Index("IsActiveIndex")]
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

        public virtual ICollection<Product> Products { get; set; }
    }
}