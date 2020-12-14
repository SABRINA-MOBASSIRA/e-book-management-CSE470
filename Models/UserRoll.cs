using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class UserRoll
    {
        [Key]
        public int UserRollID { get; set; }

        [Index("NameIndex")]
        [ForeignKey("UserGroup")]
        [Display(Name = "User Group")]
        public int? UserGroupID { get; set; }
        //[ForeignKey("User")]
        [Index("UserIDIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(300)]
        [Display(Name = "User ID")]
        public string UserID { get; set; }
        [Index("UserNameIndex")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(300)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
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
        public virtual UserGroup UserGroup { get; set; }
    }
}