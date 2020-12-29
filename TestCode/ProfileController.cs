using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBM.Helper;
using EBM.Models;
using Microsoft.AspNet.Identity;

namespace EBM.Controllers
{
    public class ProfileController : Controller
    {
        CheckAccessRoll car = new CheckAccessRoll();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Profile
        public ActionResult Index()
        {
            if (!car.CheckAccessPermission("Profile", "IsRead"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            return View();
        }

        #region Grid Machenism
        [HttpPost]
        public JsonResult GetProfiles()
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
                string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                // Loading.
                var data = ((from l in db.Profiles
                             where l.UserID == userID
                             select new ProfileGridData
                             {
                                 Name = (!string.IsNullOrEmpty(l.FirstName)) ? l.FirstName + " " + l.LastName : l.LastName,
                                 Title = l.Title,
                                 Department = l.Department,
                                 OfficePhone = l.OfficePhone,
                                 Email = l.EmailAddress,
                                 Action = "<a href='/Profile/Details/" + l.ProfileID + "' class='btn btn-primary btn-xs'><i class='fa fa-folder'></i> View </a>" +
                                 "<a href='/Profile/Edit/" + l.ProfileID + "' class='btn btn-info btn-xs'><i class='fa fa-pencil'></i> Edit </a>" +
                                 "<a href='/Profile/Delete/" + l.ProfileID + "' class='btn btn-danger btn-xs'><i class='fa fa-trash - o'></i> Delete </a>"
                             }).OrderBy(l => l.Name)).ToList();
                // Total record count.   
                int totalRecords = data.Count;
                // Verification.   
                if (!string.IsNullOrEmpty(search) &&
                    !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    List<ProfileGridData> searchData = new List<ProfileGridData>();
                    foreach (var item in data)
                    {
                        if (((!string.IsNullOrEmpty(item.Name)) ? item.Name.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.OfficePhone)) ? item.OfficePhone.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.Email)) ? item.Email.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.Title)) ? item.Title.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.Department)) ? item.Department.ToString().ToLower().Contains(search.ToLower()) : false))
                        {
                            searchData.Add(item);
                        }
                    }
                    if (searchData.Count() > 0)
                    {
                        data = new List<ProfileGridData>();
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
        private List<ProfileGridData> SortByColumnWithOrder(string order, string orderDir, List<ProfileGridData> data)
        {
            // Initialization.   
            List<ProfileGridData> lst = new List<ProfileGridData>();
            try
            {
                // Sorting   
                switch (order)
                {
                    case "0":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Name).ToList() : data.OrderBy(p => p.Name).ToList();
                        break;
                    case "1":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Title).ToList() : data.OrderBy(p => p.Title).ToList();
                        break;
                    case "2":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Department).ToList() : data.OrderBy(p => p.Department).ToList();
                        break;
                    case "3":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.OfficePhone).ToList() : data.OrderBy(p => p.OfficePhone).ToList();
                        break;
                    case "4":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Email).ToList() : data.OrderBy(p => p.Email).ToList();
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

        // GET: Profile/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object 
                    string fname = string.Empty;
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];


                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.
                        Guid gud = System.Guid.NewGuid();
                        string UploadPath = "~/Signature/" + gud + "/";
                        string serverPath = Server.MapPath(UploadPath);
                        if (!Directory.Exists(serverPath))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(serverPath);
                        }
                        string imagename = Path.GetFileName(fname);
                        fname = Path.Combine(serverPath, imagename);
                        file.SaveAs(fname);
                        fname = Path.Combine(UploadPath, imagename);
                    }
                    // Returns message that successfully uploaded  
                    return Json(fname);
                }
                catch (Exception ex)
                {
                    return Json("Nofilesuploded");
                }
            }
            else
            {
                return Json("Nofilesselected");
            }
        }

        // GET: Profile/Create
        public ActionResult Create()
        {
            if (!car.CheckAccessPermission("Profile", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            return View();
        }

        // POST: Profile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProfileID,FirstName,LastName,Title,Department,Mobile,OfficePhone,Fax,PrimaryStreet,PrimaryCity,PrimaryState,PrimaryPostalCode,PrimaryCountry,OtherStreet,OtherCity,OtherState,OtherPostalCode,OtherCountry,EmailAddress,ImagePath,UserID,IsActive,Status,CreateBy,CreateOn,UpdateBy,UpdateOn,IsDeleted,DeleteBy,DeleteOn")] Profile profile, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                string filePath = form.GetValues("hidImagePath")[0];
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (filePath == "Nofilesuploded" || filePath == "Nofilesselected")
                        profile.ImagePath = string.Empty;
                    else
                        profile.ImagePath = filePath;
                }
                string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                profile.UserID = userID;
                db.Profiles.Add(profile);
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

        // GET: Profile/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!car.CheckAccessPermission("Profile", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProfileID,FirstName,LastName,Title,Department,Mobile,OfficePhone,Fax,PrimaryStreet,PrimaryCity,PrimaryState,PrimaryPostalCode,PrimaryCountry,OtherStreet,OtherCity,OtherState,OtherPostalCode,OtherCountry,EmailAddress,ImagePath,UserID,IsActive,Status,CreateBy,CreateOn,UpdateBy,UpdateOn,IsDeleted,DeleteBy,DeleteOn")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profile).State = EntityState.Modified;
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

        // GET: Profile/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!car.CheckAccessPermission("Profile", "IsDelete"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Profile profile = db.Profiles.Find(id);
            db.Profiles.Remove(profile);
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
    public class ProfileGridData
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string OfficePhone { get; set; }
        public string Email { get; set; }
        public string Action { get; set; }

    }
}
