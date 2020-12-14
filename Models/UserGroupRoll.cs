using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class UserGroupRoll
    {
        [Key]
        public int UserGroupRollID { get; set; }
        [Index("IsReadIndex")]
        [Display(Name = "Read")]
        public bool? IsRead { get; set; }
        [Index("IsEditIndex")]
        [Display(Name = "Edit")]
        public bool? IsEdit { get; set; }
        [Index("IsDeleteIndex")]
        [Display(Name = "Delete")]
        public bool? IsDelete { get; set; }
        [Index("IsAllIndex")]
        [Display(Name = "All")]
        public bool? IsAll { get; set; }

        [Index("UserGroupIDIndex")]
        [ForeignKey("UserGroup")]
        [Display(Name = "User Group")]
        public int? UserGroupID { get; set; }
        [Index("PrivilageIDIndex")]
        [ForeignKey("Privilage")]
        [Display(Name = "Privilage Name")]
        public int? PrivilageID { get; set; }

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
        public virtual Privilage Privilage { get; set; }
    }
}