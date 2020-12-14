using Microsoft.Owin.Security;
using EBM.Controllers;
using EBM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EBM.Helper
{
    public class CheckAccessRoll : AccountController
    {
        public bool CheckAccessPermission(string privilageName, string action)
        {
            try
            {
                bool check = false;
                List<UserGroupRoll> userGroupRolls = new List<UserGroupRoll>();
                userGroupRolls = System.Web.HttpContext.Current.Session["PermittedGroupRoll"] as List<UserGroupRoll>;

                foreach (UserGroupRoll ugr in userGroupRolls)
                {
                    if (ugr.Privilage.PrivilageName == privilageName)
                    {
                        if (ugr.IsAll == true)
                            return true;
                        if (action == "IsRead")
                        {
                            if (ugr.IsRead == true)
                                check = true;
                        }
                        if (action == "IsEdit")
                        {
                            if (ugr.IsEdit == true)
                                check = true;
                        }
                        if (action == "IsDelete")
                        {
                            if (ugr.IsDelete == true)
                                check = true;
                        }
                    }
                }

                return check;
            }
            catch (Exception)
            {
                var AuthenticationManager = System.Web.HttpContext.Current.Session["AuthenticationManager"] as IAuthenticationManager;
                AuthenticationManager.SignOut();
                return false;
            }
        }
    }
}