using EBM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EBM.ViewModels
{
    public class UserGroupRollIndexData
    {
        public virtual UserGroupRoll UserGroupRoll { get; set; }
        public IEnumerable<UserGroup> UserGroups { get; set; }
        public IEnumerable<Privilage> Privilages { get; set; }
        public UserGroupRollIndexData(UserGroupRoll userGroupRoll, List<UserGroup> userGroups, List<Privilage> privilages)
        {
            UserGroupRoll = userGroupRoll;
            UserGroups = userGroups;
            Privilages = privilages;
        }
    }
}