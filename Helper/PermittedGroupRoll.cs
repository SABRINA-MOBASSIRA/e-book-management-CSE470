using Microsoft.AspNet.Identity;
using EBM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBM.Helper
{
    public class PermittedGroupRoll : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        //public List<UserGroupRoll> loadUserGroupRoll(string email, ApplicationUserManager UserManager)
        public List<UserGroupRoll> loadUserGroupRoll(string userID)
        {
            //var user = UserManager.FindByNameAsync(email);

            if (string.IsNullOrEmpty(userID))
                userID = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var userRolls = from d in db.UserRolls.Where(c => c.UserID == userID) select d;
            int? userGroupID = 0;
            foreach (UserRoll ur in userRolls)
            {
                userGroupID = ur.UserGroupID;
            }

            List<UserGroupRoll> userGroupRolls = new List<UserGroupRoll>();
            var userGroupRollByIDs = from d in db.UserGroupRolls.Where(c => c.UserGroupID == userGroupID) select d;
            foreach (UserGroupRoll ugr in userGroupRollByIDs)
            {
                userGroupRolls.Add(ugr);
            }

            return userGroupRolls;
        }
    }
}