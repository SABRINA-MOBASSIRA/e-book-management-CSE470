﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EBM.Models
{
    public class UserGroup
    {
        [Key]
        public int UserGroupID { get; set; }
        [Required]
        [Index("GroupNameIndex")]
        [Column(TypeName = "VARCHAR")]
        [Display(Name = "Group Name")]
        [StringLength(100, ErrorMessage = "Group Name cannot be longer than 100 characters.")]
        public string GroupName { get; set; }
        [Display(Name = "Description")]
        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters.")]
        public string Description { get; set; }
        [Display(Name = "Remarks")]
        [StringLength(200, ErrorMessage = "Remarks cannot be longer than 200 characters.")]
        public string Remarks { get; set; }

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

        public virtual ICollection<UserGroupRoll> UserGroupRolls { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }
}