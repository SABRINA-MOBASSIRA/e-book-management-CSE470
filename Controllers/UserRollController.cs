using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBM.Models;

namespace EBM.Controllers
{
    public class UserRollController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserRoll
        public ActionResult Index()
        {
            return View();
        }

        #region Grid Machenism
        [HttpPost]
        public JsonResult GetUserRolls()
        {
            // Initialization.   
            JsonResult result = new JsonResult();
            try
            {
                // Initialization.   
                string search = Request.Form.GetValues("search[value]")[0];
                string draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
                // Loading.
                var data = ((from l in db.UserRolls
                             select new UserRollGridData
                             {
                                 UserName = l.UserName,
                                 GroupName = l.UserGroup.GroupName,
                                 Action = "<a href='/UserRoll/Details/" + l.UserRollID + "' class='btn btn-primary btn-xs'><i class='fa fa-folder'></i> View </a>" +
                                 "<a href='/UserRoll/Edit/" + l.UserRollID + "' class='btn btn-info btn-xs'><i class='fa fa-pencil'></i> Edit </a>" +
                                 "<a href='/UserRoll/Delete/" + l.UserRollID + "' class='btn btn-danger btn-xs'><i class='fa fa-trash - o'></i> Delete </a>"
                             }).OrderBy(l => l.UserName)).ToList();
                // Total record count.   
                int totalRecords = data.Count;
                // Verification.   
                if (!string.IsNullOrEmpty(search) &&
                    !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    List<UserRollGridData> searchData = new List<UserRollGridData>();
                    foreach (var item in data)
                    {
                        if (((!string.IsNullOrEmpty(item.UserName)) ? item.UserName.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.GroupName)) ? item.GroupName.ToString().ToLower().Contains(search.ToLower()) : false))
                        {
                            searchData.Add(item);
                        }
                    }
                    if (searchData.Count() > 0)
                    {
                        data = new List<UserRollGridData>();
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
        private List<UserRollGridData> SortByColumnWithOrder(string order, string orderDir, List<UserRollGridData> data)
        {
            // Initialization.   
            List<UserRollGridData> lst = new List<UserRollGridData>();
            try
            {
                // Sorting   
                switch (order)
                {
                    case "0":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.UserName).ToList() : data.OrderBy(p => p.UserName).ToList();
                        break;
                    case "1":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.GroupName).ToList() : data.OrderBy(p => p.GroupName).ToList();
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

        // GET: UserRoll/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRoll userRoll = db.UserRolls.Find(id);
            if (userRoll == null)
            {
                return HttpNotFound();
            }
            return View(userRoll);
        }

        // GET: UserRoll/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UserGroupID = new SelectList(db.UserGroups, "UserGroupID", "GroupName");
            return View();
        }

        // POST: UserRoll/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserRollID,UserGroupID,UserID,UserName,IsActive,Status,CreateBy,CreateOn,UpdateBy,UpdateOn,IsDeleted,DeleteBy,DeleteOn")] UserRoll userRoll)
        {
            if (ModelState.IsValid)
            {
                db.UserRolls.Add(userRoll);
                int res = db.SaveChanges();
                //return RedirectToAction("Index");
                if (res > 0)
                    return Json("success", JsonRequestBehavior.AllowGet);
                else
                    return Json("error", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("invalid", JsonRequestBehavior.AllowGet);
            }
        }

        // GET: UserRoll/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRoll userRoll = db.UserRolls.Find(id);
            if (userRoll == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName", userRoll.UserID);
            ViewBag.UserGroupID = new SelectList(db.UserGroups, "UserGroupID", "GroupName", userRoll.UserGroupID);
            return View(userRoll);
        }

        // POST: UserRoll/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserRollID,UserGroupID,UserID,UserName,IsActive,Status,CreateBy,CreateOn,UpdateBy,UpdateOn,IsDeleted,DeleteBy,DeleteOn")] UserRoll userRoll)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userRoll).State = EntityState.Modified;
                int res = db.SaveChanges();
                //return RedirectToAction("Index");
                if (res > 0)
                    return Json("success", JsonRequestBehavior.AllowGet);
                else
                    return Json("error", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("invalid", JsonRequestBehavior.AllowGet);
            }
        }

        // GET: UserRoll/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRoll userRoll = db.UserRolls.Find(id);
            if (userRoll == null)
            {
                return HttpNotFound();
            }
            return View(userRoll);
        }

        // POST: UserRoll/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserRoll userRoll = db.UserRolls.Find(id);
            db.UserRolls.Remove(userRoll);
            int res = db.SaveChanges();
            //return RedirectToAction("Index");
            if (res > 0)
                return Json("success", JsonRequestBehavior.AllowGet);
            else
                return Json("error", JsonRequestBehavior.AllowGet);
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

    public class UserRollGridData
    {
        public string UserName { get; set; }
        public string GroupName { get; set; }
        public string Action { get; set; }

    }
}
