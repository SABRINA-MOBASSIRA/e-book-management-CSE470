using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBM.Models;
using EBM.ViewModels;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace EBM.Controllers
{
    public class UserGroupRollController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserGroupRoll
        public ActionResult Index()
        {
            var userGroupRolls = db.UserGroupRolls.Include(u => u.Privilage).Include(u => u.UserGroup);
            return View(userGroupRolls.ToList());
        }

        // GET: UserGroupRoll/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserGroupRoll userGroupRoll = db.UserGroupRolls.Find(id);
            if (userGroupRoll == null)
            {
                return HttpNotFound();
            }
            return View(userGroupRoll);
        }

        // GET: UserGroupRoll/Create
        public ActionResult Create()
        {
            ViewBag.PrivilageID = new SelectList(db.Privilages, "PrivilageID", "PrivilageName");
            ViewBag.UserGroupID = new SelectList(db.UserGroups, "UserGroupID", "GroupName");
            return View();
        }

        // POST: UserGroupRoll/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserGroupRollID,IsRead,IsEdit,IsDelete,IsAll,UserGroupID,PrivilageID,IsActive,Status,CreateBy,CreateOn,UpdateBy,UpdateOn,IsDeleted,DeleteBy,DeleteOn")] UserGroupRoll userGroupRoll)
        {
            if (ModelState.IsValid)
            {
                db.UserGroupRolls.Add(userGroupRoll);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PrivilageID = new SelectList(db.Privilages, "PrivilageID", "PrivilageName", userGroupRoll.PrivilageID);
            ViewBag.UserGroupID = new SelectList(db.UserGroups, "UserGroupID", "GroupName", userGroupRoll.UserGroupID);
            return View(userGroupRoll);
        }

        #region Grid Machenism
        [HttpPost]
        public JsonResult GetUserGroupRolls(int id)
        {
            // Initialization.   
            JsonResult result = new JsonResult();
            try
            {
                List<UserGroupRollGridData> data = new List<UserGroupRollGridData>();
                // Initialization.   
                string search = Request.Form.GetValues("search[value]")[0];
                string draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                var _ddls = getAllGroupRoll(id);
                if (_ddls == null)
                {
                    // Loading.
                    data = ((from l in db.Privilages
                             select new UserGroupRollGridData
                             {
                                 PrivilageName = l.PrivilageName,
                                 Read = "<th><input type='checkbox' id='checkRead' class='flat' value='" + l.PrivilageID + "'></th>",
                                 Edit = "<th><input type='checkbox' id='checkEdit' class='flat' value='" + l.PrivilageID + "'></th>",
                                 Delete = "<th><input type='checkbox' id='checkDelete' class='flat' value='" + l.PrivilageID + "'></th>",
                                 All = "<th><input type='checkbox' id='checkAll' class='flat' value='" + l.PrivilageID + "'></th>",
                             }).OrderBy(l => l.PrivilageName)).ToList();
                }
                else
                {
                    // Loading.
                    data = ((from l in _ddls
                             select new UserGroupRollGridData
                             {
                                 PrivilageName = l.PrivilageName,
                                 Read = "<th><input type='checkbox' id='checkRead' class='flat' " + ((l.IsRead != null && l.IsRead == true) ? "checked='checked'" : "") + " value='" + l.PrivilageID + "'></th>",
                                 Edit = "<th><input type='checkbox' id='checkEdit' class='flat' " + ((l.IsEdit != null && l.IsEdit == true) ? "checked='checked'" : "") + " value='" + l.PrivilageID + "'></th>",
                                 Delete = "<th><input type='checkbox' id='checkDelete' class='flat' " + ((l.IsDelete != null && l.IsDelete == true) ? "checked='checked'" : "") + " value='" + l.PrivilageID + "'></th>",
                                 All = "<th><input type='checkbox' id='checkAll' class='flat' " + ((l.IsAll != null && l.IsAll == true) ? "checked='checked'" : "") + " value='" + l.PrivilageID + "'></th>",
                             }).OrderBy(l => l.PrivilageName)).ToList();
                }

                // Total record count.   
                int totalRecords = data.Count;
                // Verification.   
                if (!string.IsNullOrEmpty(search) &&
                    !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    List<UserGroupRollGridData> searchData = new List<UserGroupRollGridData>();
                    foreach (var item in data)
                    {
                        if (((!string.IsNullOrEmpty(item.PrivilageName)) ? item.PrivilageName.ToString().ToLower().Contains(search.ToLower()) : false))
                        {
                            searchData.Add(item);
                        }
                    }
                    if (searchData.Count() > 0)
                    {
                        data = new List<UserGroupRollGridData>();
                        data = searchData;
                    }
                }
                // Sorting.   
                data = this.SortByColumnWithOrder(order, orderDir, data);
                // Filter record count.   
                int recFilter = data.Count;
                // Apply pagination.   
                data = data.Skip(startRec).Take(pageSize).ToList();
                // Loading drop down lists.   
                result = this.Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = totalRecords,
                    recordsFiltered = recFilter,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Info   
                Console.Write(ex);
            }

            // Return info.   
            return result;
        }

        /// <summary>  
        /// Sort by column with order method.   
        /// </summary>  
        /// <param name="order">Order parameter</param>  
        /// <param name="orderDir">Order direction parameter</param>  
        /// <param name="data">Data parameter</param>  
        /// <returns>Returns - Data</returns>  
        private List<UserGroupRollGridData> SortByColumnWithOrder(string order, string orderDir, List<UserGroupRollGridData> data)
        {
            // Initialization.   
            List<UserGroupRollGridData> lst = new List<UserGroupRollGridData>();
            try
            {
                // Sorting   
                switch (order)
                {
                    case "0":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.PrivilageName).ToList() : data.OrderBy(p => p.PrivilageName).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                // info.   
                Console.Write(ex);
            }
            // info.   
            return lst;
        }
        #endregion

        [HttpPost]
        public ActionResult LoadAllRolls(int UserGroupId)
        {
            var _ddls = getAllGroupRoll(UserGroupId);

            if (_ddls == null)
                return Json(null);

            //List<ddlSite> sites = (List<ddlSite>)_ddls.ToList();
            return Json(_ddls);
        }
        public List<AssignGroupRoll> getAllGroupRoll(int UserGroupId)
        {
            List<AssignGroupRoll> assignGroupRolls = new List<AssignGroupRoll>();
            var groupRolls = from d in db.UserGroupRolls where d.UserGroupID == UserGroupId select d;
            var privilages = db.Privilages.ToList();
            foreach (var priv in privilages)
            {
                var ugr = (from g in groupRolls where g.PrivilageID == priv.PrivilageID select g).FirstOrDefault();
                AssignGroupRoll agr = new AssignGroupRoll();
                if (ugr != null)
                {
                    agr.UserGroupRollID = Convert.ToInt32(ugr.UserGroupRollID);
                    agr.PrivilageID = Convert.ToInt32(ugr.PrivilageID);
                    agr.PrivilageName = priv.PrivilageName;
                    agr.IsRead = (ugr.IsRead != null && ugr.IsRead == true) ? true : false;
                    agr.IsEdit = (ugr.IsEdit != null && ugr.IsEdit == true) ? true : false;
                    agr.IsDelete = (ugr.IsDelete != null && ugr.IsDelete == true) ? true : false;
                    agr.IsAll = (ugr.IsAll != null && ugr.IsAll == true) ? true : false;
                }
                else
                {
                    agr.PrivilageID = Convert.ToInt32(priv.PrivilageID);
                    agr.PrivilageName = priv.PrivilageName;
                    agr.IsRead = false;
                    agr.IsEdit = false;
                    agr.IsDelete = false;
                    agr.IsAll = false;
                }
                assignGroupRolls.Add(agr);
            }
            return assignGroupRolls;
        }

        public ActionResult CreateGroupRoll()
        {
            UserGroupRoll ugr = new UserGroupRoll();
            var userGroups = from d in db.UserGroups
                             select d;
            var privilages = from d in db.Privilages
                             select d;
            UserGroupRollIndexData ugrid = new UserGroupRollIndexData(ugr, userGroups.ToList(), privilages.ToList());

            ViewBag.PrivilageID = new SelectList(db.Privilages, "PrivilageID", "PrivilageName");
            ViewBag.UserGroupID = new SelectList(db.UserGroups, "UserGroupID", "GroupName");
            return View(ugrid);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGroupRoll(UserGroupRoll ugr, FormCollection form)
        {
            UserGroupRoll userGroupRoll = new UserGroupRoll();
            BindingList<UserGroupRoll> userGroupRolls = new BindingList<UserGroupRoll>();
            //First Delete all Group Roll
            DeleteAllbyGroupID(Convert.ToInt32(ugr.UserGroupID));
            ///////////////////////////////////
            var selectedprivilages = form.GetValues("hidChekedPrivilage");
            string[] privilages = Regex.Split(selectedprivilages[0].ToString(), "/");
            foreach (var itemSection in privilages)
            {
                if (itemSection.Contains("read="))
                {
                    string val = itemSection.Replace("read=", string.Empty);
                    string[] readVal = Regex.Split(val.ToString(), ",");
                    foreach (var item in readVal)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var any = userGroupRolls.Where(c => c.PrivilageID == Convert.ToInt32(item)).FirstOrDefault();
                            if (any == null)
                            {
                                userGroupRoll = new UserGroupRoll();
                                userGroupRoll.UserGroupRollID = 0;
                                userGroupRoll.UserGroupID = Convert.ToInt32(ugr.UserGroupID);
                                userGroupRoll.PrivilageID = Convert.ToInt32(item);
                                userGroupRoll.IsRead = true;
                                userGroupRolls.Add(userGroupRoll);
                            }
                            else
                            {
                                foreach (var roll in userGroupRolls)
                                {
                                    if (roll.PrivilageID == Convert.ToInt32(item))
                                    {
                                        roll.IsRead = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (itemSection.Contains("edit="))
                {
                    string val = itemSection.Replace("edit=", string.Empty);
                    string[] editVal = Regex.Split(val.ToString(), ",");
                    foreach (var item in editVal)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var any = userGroupRolls.Where(c => c.PrivilageID == Convert.ToInt32(item)).FirstOrDefault();
                            if (any == null)
                            {
                                userGroupRoll = new UserGroupRoll();
                                userGroupRoll.UserGroupRollID = 0;
                                userGroupRoll.UserGroupID = Convert.ToInt32(ugr.UserGroupID);
                                userGroupRoll.PrivilageID = Convert.ToInt32(item);
                                userGroupRoll.IsEdit = true;
                                userGroupRolls.Add(userGroupRoll);
                            }
                            else
                            {
                                foreach (var roll in userGroupRolls)
                                {
                                    if (roll.PrivilageID == Convert.ToInt32(item))
                                    {
                                        roll.IsEdit = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (itemSection.Contains("delete="))
                {
                    string val = itemSection.Replace("delete=", string.Empty);
                    string[] deleteVal = Regex.Split(val.ToString(), ",");
                    foreach (var item in deleteVal)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var any = userGroupRolls.Where(c => c.PrivilageID == Convert.ToInt32(item)).FirstOrDefault();
                            if (any == null)
                            {
                                userGroupRoll = new UserGroupRoll();
                                userGroupRoll.UserGroupRollID = 0;
                                userGroupRoll.UserGroupID = Convert.ToInt32(ugr.UserGroupID);
                                userGroupRoll.PrivilageID = Convert.ToInt32(item);
                                userGroupRoll.IsDelete = true;
                                userGroupRolls.Add(userGroupRoll);
                            }
                            else
                            {
                                foreach (var roll in userGroupRolls)
                                {
                                    if (roll.PrivilageID == Convert.ToInt32(item))
                                    {
                                        roll.IsDelete = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (itemSection.Contains("all="))
                {
                    string val = itemSection.Replace("all=", string.Empty);
                    string[] allVal = Regex.Split(val.ToString(), ",");
                    foreach (var item in allVal)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var any = userGroupRolls.Where(c => c.PrivilageID == Convert.ToInt32(item)).FirstOrDefault();
                            if (any == null)
                            {
                                userGroupRoll = new UserGroupRoll();
                                userGroupRoll.UserGroupRollID = 0;
                                userGroupRoll.UserGroupID = Convert.ToInt32(ugr.UserGroupID);
                                userGroupRoll.PrivilageID = Convert.ToInt32(item);
                                userGroupRoll.IsAll = true;
                                userGroupRolls.Add(userGroupRoll);
                            }
                            else
                            {
                                foreach (var roll in userGroupRolls)
                                {
                                    if (roll.PrivilageID == Convert.ToInt32(item))
                                    {
                                        roll.IsAll = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ////////////////////////////////
            bool ret = false;
            foreach (UserGroupRoll userGrpRoll in userGroupRolls)
            {
                ret = CreateUserGroupRoll(userGrpRoll);
            }
            if (ret)
                return Json("success", JsonRequestBehavior.AllowGet);
            else
                return Json("error", JsonRequestBehavior.AllowGet);
            //return RedirectToAction("CreateGroupRoll");
        }
        public void DeleteAllbyGroupID(int id)
        {
            List<UserGroupRoll> userGroupRolls = db.UserGroupRolls.Where(c => c.UserGroupID == id).ToList();
            foreach (UserGroupRoll ugr in userGroupRolls)
            {
                UserGroupRoll userGroupRoll = db.UserGroupRolls.Find(ugr.UserGroupRollID);
                db.UserGroupRolls.Remove(userGroupRoll);
                db.SaveChanges();
            }
        }
        public bool CreateUserGroupRoll([Bind(Include = "UserGroupRollID,IsRead,IsEdit,IsDelete,IsAll,UserGroupID,PrivilageID,IsActive,Status,CreateBy,CreateOn,UpdateBy,UpdateOn,IsDeleted,DeleteBy,DeleteOn")] UserGroupRoll userGroupRoll)
        {
            if (ModelState.IsValid)
            {
                db.UserGroupRolls.Add(userGroupRoll);
                int res = db.SaveChanges();
                if (res > 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }



        // GET: UserGroupRoll/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserGroupRoll userGroupRoll = db.UserGroupRolls.Find(id);
            if (userGroupRoll == null)
            {
                return HttpNotFound();
            }
            ViewBag.PrivilageID = new SelectList(db.Privilages, "PrivilageID", "PrivilageName", userGroupRoll.PrivilageID);
            ViewBag.UserGroupID = new SelectList(db.UserGroups, "UserGroupID", "GroupName", userGroupRoll.UserGroupID);
            return View(userGroupRoll);
        }

        // POST: UserGroupRoll/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserGroupRollID,IsRead,IsEdit,IsDelete,IsAll,UserGroupID,PrivilageID,IsActive,Status,CreateBy,CreateOn,UpdateBy,UpdateOn,IsDeleted,DeleteBy,DeleteOn")] UserGroupRoll userGroupRoll)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userGroupRoll).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PrivilageID = new SelectList(db.Privilages, "PrivilageID", "PrivilageName", userGroupRoll.PrivilageID);
            ViewBag.UserGroupID = new SelectList(db.UserGroups, "UserGroupID", "GroupName", userGroupRoll.UserGroupID);
            return View(userGroupRoll);
        }

        // GET: UserGroupRoll/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserGroupRoll userGroupRoll = db.UserGroupRolls.Find(id);
            if (userGroupRoll == null)
            {
                return HttpNotFound();
            }
            return View(userGroupRoll);
        }

        // POST: UserGroupRoll/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserGroupRoll userGroupRoll = db.UserGroupRolls.Find(id);
            db.UserGroupRolls.Remove(userGroupRoll);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    public class AssignGroupRoll
    {
        public int UserGroupRollID { get; set; }
        public int PrivilageID { get; set; }
        public string PrivilageName { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsAll { get; set; }
    }

    public class UserGroupRollGridData
    {
        public string PrivilageName { get; set; }
        public string Read { get; set; }
        public string Edit { get; set; }
        public string Delete { get; set; }
        public string All { get; set; }

    }
}
