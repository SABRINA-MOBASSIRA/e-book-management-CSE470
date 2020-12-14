using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBM.Models;
using EBM.Helper;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Drawing;

namespace EBM.Controllers
{
    public class ProductController : Controller
    {
        CheckAccessRoll car = new CheckAccessRoll();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Product
        public ActionResult Index()
        {
            if (!car.CheckAccessPermission("Product", "IsRead"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            return View();
        }

        #region Grid Machenism
        [HttpPost]
        public JsonResult GetProducts()
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
                var data = ((from l in db.Products
                             select new ProductGridData
                             {
                                 Name = l.Name,
                                 Price = l.Price,
                                 Photo = "<img src = '" + l.ImagePath.Replace("~", "") + "' style = 'width: 40px; height: 40px;' />",
                                 Action = "<a href='/Product/Details/" + l.ProductID + "' class='btn btn-primary btn-xs'><i class='fa fa-folder'></i> View </a>" +
                                 "<a href='/Product/Edit/" + l.ProductID + "' class='btn btn-info btn-xs'><i class='fa fa-pencil'></i> Edit </a>" +
                                 "<a href='/Product/Delete/" + l.ProductID + "' class='btn btn-danger btn-xs'><i class='fa fa-trash - o'></i> Delete </a>"
                             }).OrderBy(l => l.Name)).ToList();
                // Total record count.   
                int totalRecords = data.Count;
                // Verification.   
                if (!string.IsNullOrEmpty(search) &&
                    !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    List<ProductGridData> searchData = new List<ProductGridData>();
                    foreach (var item in data)
                    {
                        if (((!string.IsNullOrEmpty(item.Name)) ? item.Name.ToString().ToLower().Contains(search.ToLower()) : false))
                        {
                            searchData.Add(item);
                        }
                    }
                    if (searchData.Count() > 0)
                    {
                        data = new List<ProductGridData>();
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
        private List<ProductGridData> SortByColumnWithOrder(string order, string orderDir, List<ProductGridData> data)
        {
            // Initialization.   
            List<ProductGridData> lst = new List<ProductGridData>();
            try
            {
                // Sorting   
                switch (order)
                {
                    case "0":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Name).ToList() : data.OrderBy(p => p.Name).ToList();
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

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            List<UploadedFile> upfiles = new List<UploadedFile>();
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
                        string UploadPath = "~/Files/" + gud + "/";
                        string serverPath = Server.MapPath(UploadPath);
                        if (!Directory.Exists(serverPath))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(serverPath);
                        }
                        string imagename = Path.GetFileName(fname);
                        fname = Path.Combine(serverPath, imagename);
                        file.SaveAs(fname);
                        fname = Path.Combine(UploadPath, imagename);
                        UploadedFile upf = new UploadedFile();
                        if (file.ContentType.Contains("image"))
                            upf.type = "image";
                        else if (file.ContentType.Contains("pdf"))
                            upf.type = "pdf";
                        upf.path = fname;
                        upfiles.Add(upf);
                    }
                    // Returns message that successfully uploaded  
                    return Json(upfiles);
                }
                catch (Exception ex)
                {
                    UploadedFile upf = new UploadedFile();
                    upf.type = "nofile";
                    upf.path = "Nofilesuploded";
                    upfiles.Add(upf);
                    return Json(upfiles);
                }
            }
            else
            {
                UploadedFile upf = new UploadedFile();
                upf.type = "nofile";
                upf.path = "Nofilesuploded";
                upfiles.Add(upf);
                return Json(upfiles);
            }
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            if (!car.CheckAccessPermission("Product", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,Name,Price,ImagePath")] Product product, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                string filePath = form.GetValues("hidImagePath")[0];
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (filePath == "Nofilesuploded" || filePath == "Nofilesselected")
                        product.ImagePath = string.Empty;
                    else
                        product.ImagePath = filePath;
                }
                db.Products.Add(product);
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

        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!car.CheckAccessPermission("Product", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,Name,Price,ImagePath")] Product product, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                string filePath = form.GetValues("hidImagePath")[0];
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (filePath == "Nofilesuploded" || filePath == "Nofilesselected")
                    { }
                    else
                    {
                        string file = Server.MapPath(product.ImagePath);
                        if (System.IO.File.Exists(file))
                            System.IO.File.Delete(file);
                        product.ImagePath = filePath;
                    }
                }
                db.Entry(product).State = EntityState.Modified;
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

        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!car.CheckAccessPermission("Product", "IsDelete"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
    public class ProductGridData
    {
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }

    }
    public class UploadedFile
    {
        public string type { get; set; }
        public string path { get; set; }
    }
}
