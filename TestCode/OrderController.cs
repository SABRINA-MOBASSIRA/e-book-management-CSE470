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
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using Microsoft.AspNet.Identity;
using iTextSharp.text;
using iTextSharp.text.pdf;
using CrystalDecisions.Shared;
using System.Text;
using LINQtoCSV;
using System.Text.RegularExpressions;

namespace EBM.Controllers
{
    public class OrderController : Controller
    {
        CheckAccessRoll car = new CheckAccessRoll();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Order
        public ActionResult Index()
        {
            //if (!car.CheckAccessPermission("Order", "IsRead"))
            //{
            //    string URL = Request.UrlReferrer.ToString();
            //    Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
            //    return Redirect(URL);
            //}
            //ViewBag.QotationType = type;
            return View("Index");
        }

        #region Grid Machenism
        [HttpPost]
        public JsonResult GetOrders()
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
                string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var allUser = (from c in db.Users select c).ToList();
                var useradmin = (from c in db.UserRolls where c.UserID == userID && c.UserGroup.GroupName.ToLower() == "admin" select c).FirstOrDefault();
                
                var data = ((from l in db.Orders
                             select new OrderGridData
                             {
                                 ID = l.OrderID,
                                 OrderNo = l.OrderNo,
                                 Date = l.Date.ToString(),
                                 Date_DT = l.Date,
                                 Customer = l.Customer.Name,
                                 OrderDetails = db.Carts.Where(c=>c.OrderID == l.OrderID).ToList(),
                                 Detail = "<a href='/Cart/Index/" + l.OrderID + "' class='btn btn-primary btn-xs'><i class='fa fa-list'></i> Details </a>" +
                                 "<a href='/Order/EditPrice/" + l.OrderID + "' class='btn btn-primary btn-xs'><i class='fa fa-money'></i> Price </a>",
                                 Action = "<a href='/Order/Details/" + l.OrderID + "' class='btn btn-primary btn-xs'><i class='fa fa-folder'></i> View </a>" +
                                 "<a href='/Order/Edit/" + l.OrderID + "' class='btn btn-info btn-xs'><i class='fa fa-pencil'></i> Edit </a>" +
                                 "<a href='/Order/Delete/" + l.OrderID + "' class='btn btn-danger btn-xs'><i class='fa fa-trash - o'></i> Delete </a>" +
                                 ((l.IsActive == true) ? "<a href='/Order/RejectOrder/" + l.OrderID + "' class='btn btn-default btn-xs'><i class='fa fa-close'></i> Reject </a>" : "<a href='/Order/AcceptOrder/" + l.OrderID + "' class='btn btn-default btn-xs'><i class='fa fa-check-circle-o'></i> Confirm </a>")
                                 //+ "<a href='/Order/OrderRpt/" + l.OrderID+"|"+type + "' class='btn btn-default btn-xs'><i class='fa fa-print'></i> Print Order </a>"
                             }).AsEnumerable().Select(c => new OrderGridData
                             {
                                 ID = c.ID,
                                 OrderNo = c.OrderNo,
                                 Date = c.Date.ToString(),
                                 Date_DT = c.Date_DT,
                                 Customer = c.Customer,
                                 TotalPrice = (c.OrderDetails != null)? GetTotalPrice(c.OrderDetails.ToList()) : 0,
                                 Detail = c.Detail,
                                 //userid = c.userid,
                                 //userName = allUser.Where(u => u.Id == c.userid).Select(u => u.UserName).FirstOrDefault(),
                                 Action = c.Action
                             }).OrderBy(l => l.Date_DT)).OrderByDescending(l => l.Date_DT).ToList();

                data = (useradmin != null) ? data : (data.OrderBy(l => l.Date_DT)).OrderByDescending(l => l.Date_DT).ToList();

                // Total record count.   
                int totalRecords = data.Count;
                // Verification.   
                if (!string.IsNullOrEmpty(search) &&
                    !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    List<OrderGridData> searchData = new List<OrderGridData>();
                    foreach (var item in data)
                    {
                        if (((!string.IsNullOrEmpty(item.OrderNo)) ? item.OrderNo.ToString().ToLower().Contains(search.ToLower()) : false) ||
                            ((!string.IsNullOrEmpty(item.Customer)) ? item.Customer.ToString().ToLower().Contains(search.ToLower()) : false))
                        {
                            searchData.Add(item);
                        }
                    }
                    if (searchData.Count() > 0)
                    {
                        data = new List<OrderGridData>();
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
        private List<OrderGridData> SortByColumnWithOrder(string order, string orderDir, List<OrderGridData> data)
        {
            // Initialization.   
            List<OrderGridData> lst = new List<OrderGridData>();
            try
            {
                // Sorting   
                switch (order)
                {
                    case "0":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.OrderNo).ToList() : data.OrderBy(p => p.OrderNo).ToList();
                        break;
                    case "3":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Customer).ToList() : data.OrderBy(p => p.Customer).ToList();
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

        //public FileContentResult DownloadCSV()
        //{
        //    string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    string userName = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    var allUser = (from c in db.Users select c).ToList();
        //    var useradmin = (from c in db.UserRolls where c.UserID == userID && c.UserGroup.GroupName.ToLower() == "admin" select c).FirstOrDefault();
        //    var data = ((from l in db.Orders
        //                 select new SalesQuotationGridData
        //                 {
        //                     ID = l.SalesQuotationID,
        //                     QuotationNo = l.QuotationNo,
        //                     Rivision = (l.Rivision != null) ? l.Rivision.ToString() : "0",
        //                     Date = l.Date.ToString(),
        //                     Date_DT = l.Date,
        //                     Customer = l.Customer.Name,
        //                     Add = (((l.InstalationCost != null) ? l.InstalationCost : 0) + ((l.OtherCost != null) ? l.OtherCost : 0)),
        //                     VATAIT = ((l.VAT != null) ? l.VAT : 0) + ((l.ATI != null) ? l.ATI : 0),
        //                     Less = ((l.Discount != null) ? l.Discount : 0),
        //                     QuotionDetails = l.SalesQuotationDetails.ToList(),
        //                     userid = l.UserID,
        //                 }).OrderBy(l => l.Date_DT)).OrderByDescending(l => l.Date_DT).AsEnumerable().Select(c => new SalesQuotationCSV
        //                 {
        //                     QuotationNo = c.QuotationNo,
        //                     Rivision = c.Rivision,
        //                     Date = c.Date.ToString(),
        //                     Customer = c.Customer,
        //                     Add = c.Add.ToString(),
        //                     Less = c.Less.ToString(),
        //                     TotalPrice = (GetTotalPrice(c.QuotionDetails.ToList()) + ((GetTotalPrice(c.QuotionDetails.ToList()) * c.VATAIT) / 100) + c.Add - ((GetTotalPrice(c.QuotionDetails.ToList()) * c.Less) / 100)).ToString(),
        //                     userName = allUser.Where(u => u.Id == c.userid).Select(u => u.UserName).FirstOrDefault()
        //                 }).ToList();

        //    data = (useradmin != null) ? data : (data.Where(c => c.userName == userName)).ToList();

        //    List<object> qotations = (from q in data
        //                              select new[]
        //                              {
        //                                  q.QuotationNo,
        //                                  q.Rivision,
        //                                  q.Date.ToString(),
        //                                  q.Customer,
        //                                  q.Add.ToString(),
        //                                  q.Less.ToString(),
        //                                  q.TotalPrice,
        //                                  q.userName
        //                              }).ToList<object>();

        //    //Insert the Column Names.
        //    qotations.Insert(0, new string[8] { "Quotation No", "Rivision", "Date", "Customer", "Add", "Less", "Total Price", "User Name" });

        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < qotations.Count; i++)
        //    {
        //        string[] qotation = (string[])qotations[i];
        //        for (int j = 0; j < qotation.Length; j++)
        //        {
        //            //Append data with separator.
        //            sb.Append(qotation[j] + ',');
        //        }
        //        //Append new line character.
        //        sb.Append("\r\n");
        //    }

        //    return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "SalesQutation.csv");
        //}

        public ActionResult AcceptOrder(int? id)
        {
            //if (!car.CheckAccessPermission("Accept Order", "IsEdit"))
            //{
            //    string URL = Request.UrlReferrer.ToString();
            //    Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
            //    return Redirect(URL);
            //}
            var order = db.Orders.Find(id);
            order.IsActive = true;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            //ViewBag.PageID ="Index";
            return RedirectToAction("Index");
        }
        public ActionResult RejectOrder(int? id)
        {
            //if (!car.CheckAccessPermission("Accept Order", "IsEdit"))
            //{
            //    string URL = Request.UrlReferrer.ToString();
            //    Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
            //    return Redirect(URL);
            //}
            var order = db.Orders.Find(id);
            order.IsActive = false;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //public ActionResult SalesQuotationRpt(string id)
        //{
        //    string[] spl = Regex.Split(id, @"\|");
        //    int salqtid = Convert.ToInt32(spl[0]);
        //    List<SalesQuotationReport> bcrList = new List<SalesQuotationReport>();
        //    var bcs = (from bc in db.SalesQuotations where bc.SalesQuotationID == salqtid select bc).FirstOrDefault();
        //    var details = (from c in db.SalesQuotationDetails where c.SalesQuotationID == bcs.SalesQuotationID select c).ToList();
        //    string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    var profile = (from c in db.Profiles where c.UserID == userID select c).FirstOrDefault();
        //    foreach (var item in details)
        //    {
        //        SalesQuotationReport bcr = new SalesQuotationReport();
        //        bcr.ImagePath = Server.MapPath(item.Product.ImagePath);
        //        bcr.ProductModel = item.Product.ProductModel.ModelNo;
        //        bcr.Date = Convert.ToDateTime(bcs.Date);
        //        bcr.ProductName = item.Product.Name;
        //        bcr.ProductDescription = item.Product.Description;
        //        bcr.Quantity = Convert.ToDecimal(item.Quantity);
        //        bcr.QuotationNo = bcs.QuotationNo;
        //        bcr.MesurementUnit = item.Product.MesurmentUnit.Name;
        //        bcr.TotalPrice = Convert.ToDecimal(item.TotalPrice);
        //        bcr.UnitPrice = Convert.ToDecimal(item.UnitPrice);
        //        bcr.InstalationCost = Convert.ToDecimal(bcs.InstalationCost);
        //        bcr.VAT = Convert.ToDecimal(bcs.VAT);
        //        bcr.ATI = Convert.ToDecimal(bcs.ATI);
        //        bcr.Discount = (item.Discount != null) ? Convert.ToDecimal(item.Discount) : 0;//Convert.ToDecimal(bcs.Discount);
        //        bcr.SalesQuotationID = item.SalesQuotationDetailID;
        //        bcr.CustomerName = bcs.Customer.Name;
        //        bcr.CustomerAddress = ((!string.IsNullOrEmpty(bcs.Customer.BillingStreet)) ? bcs.Customer.BillingStreet + ", " : string.Empty) + ((!string.IsNullOrEmpty(bcs.Customer.BillingState)) ? bcs.Customer.BillingState + ", " : string.Empty) +
        //            ((!string.IsNullOrEmpty(bcs.Customer.BillingCity)) ? bcs.Customer.BillingCity + ", " : string.Empty) + ((!string.IsNullOrEmpty(bcs.Customer.BillingPostalCode)) ? bcs.Customer.BillingPostalCode + "." : string.Empty);
        //        bcr.Subject = bcs.Subject;
        //        bcr.ToBePaid = Convert.ToInt32(bcs.ToBePaid);
        //        bcr.WillBePaid = Convert.ToInt32(bcs.WillBePaid);
        //        bcr.DeliveryDay = Convert.ToInt32(bcs.DeliveryDay);
        //        bcr.WarrantyDay = Convert.ToInt32(bcs.WarrantyDay);
        //        try
        //        {
        //            bcr.ProfileName = (!string.IsNullOrEmpty(profile.FirstName)) ? profile.FirstName + " " + profile.LastName : profile.LastName;
        //            bcr.ProfileTitle = profile.Title;
        //            bcr.ProfileDepartment = profile.Department;
        //            bcr.ProfileSignaturePath = Server.MapPath(profile.ImagePath);
        //            bcr.ProfileMobile = profile.Mobile;
        //            bcr.ProfileEmail = profile.EmailAddress;
        //        }
        //        catch { }
        //        bcrList.Add(bcr);
        //    }

        //    Stream stream = null;
        //    string type = "";
        //    string filename = "";

        //    using (ReportDocument rd = new ReportDocument())
        //    {
        //        if (spl[1] == "LS")
        //            rd.Load(Server.MapPath("/Report/SalesQuotationRpt.rpt"), OpenReportMethod.OpenReportByTempCopy);
        //        else
        //            rd.Load(Server.MapPath("/Report/SalesQuotationHSRpt.rpt"), OpenReportMethod.OpenReportByTempCopy);
        //        rd.SetDataSource(bcrList);
        //        Response.Buffer = false;
        //        Response.ClearContent();
        //        Response.ClearHeaders();


        //        stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    }

        //    byte[] mergedPdf = null;
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        using (Document document = new Document())
        //        {
        //            using (PdfCopy copy = new PdfCopy(document, ms))
        //            {
        //                document.Open();
        //                byte[] myBinary = new byte[stream.Length];
        //                stream.Read(myBinary, 0, (int)stream.Length);
        //                PdfReader _reader = new PdfReader(myBinary);
        //                //PdfReader reader = new PdfReader(pdf[i]);
        //                // loop over the pages in that document
        //                int n = _reader.NumberOfPages;
        //                for (int page = 0; page < n;)
        //                {
        //                    copy.AddPage(copy.GetImportedPage(_reader, ++page));
        //                }
        //                foreach (var item in details)
        //                {
        //                    if (!string.IsNullOrEmpty(item.Product.SpecificationFile))
        //                    {
        //                        try
        //                        {
        //                            PdfReader reader = new PdfReader(Server.MapPath(item.Product.SpecificationFile.Replace("~", string.Empty)));
        //                            // loop over the pages in that document
        //                            n = reader.NumberOfPages;
        //                            for (int page = 0; page < n;)
        //                            {
        //                                copy.AddPage(copy.GetImportedPage(reader, ++page));
        //                            }
        //                        }
        //                        catch (Exception ex) { }
        //                    }
        //                }
        //            }
        //        }
        //        mergedPdf = ms.ToArray();
        //    }
        //    Stream stream2 = new MemoryStream(mergedPdf);

        //    stream2.Seek(0, SeekOrigin.Begin);
        //    type = "application/pdf";
        //    filename = "SalesQuotationRpt.pdf";

        //    stream2.Seek(0, SeekOrigin.Begin);
        //    return File(stream2, type, filename);
        //}

        public decimal GetTotalPrice(List<Cart> carts)
        {
            decimal tPrice = 0;
            foreach (var item in carts)
            {
                tPrice = tPrice + Convert.ToDecimal(item.TotalPrice);
            }

            return tPrice;
        }

        //public string GetRiviseValue(string quotationNo, string rivise, int SalesQuotationID)
        //{
        //    string value = "0";
        //    var maxRivise = db.SalesQuotations.Where(c => c.QuotationNo == quotationNo).Max(c => c.Rivision);
        //    if (Convert.ToInt32(rivise) < maxRivise)
        //        value = rivise.ToString();
        //    else
        //        value = "<a href='/SalesQuotation/Rivise/" + SalesQuotationID + "' class='btn btn-warning btn-xs'><i class='fa fa-random'></i> Rivise </a>";

        //    return value;
        //}

        // GET: Order/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Order salesQuotation = (from c in db.Orders where c.OrderID == id select c).FirstOrDefault();
            if (salesQuotation == null)
            {
                return HttpNotFound();
            }
            return View(salesQuotation);
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            if (!car.CheckAccessPermission("Order", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ViewBag.CustomerID = new SelectList(db.Customers.ToList(), "CustomerID", "Name");
            var sql = (from cr in db.Orders select cr).ToList();
            int max = (sql.Count() > 0) ? sql.Count() : 0;
            ViewBag.OrderNo = "EBM/ODR/" + (max + 1).ToString("000000");
            //ViewBag.Subject = "Financial Offer for ";
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,OrderNo,Date,CustomerID")] Order order)
        {
            if (ModelState.IsValid)
            {
                //string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                //order.UserID = userID;
                
                var sql = (from cr in db.Orders select cr).AsNoTracking().ToList();
                int max = (sql.Count() > 0) ? sql.Count() : 0;
                order.OrderNo = "EBM/ODR/" + (max + 1).ToString("000000");
                order.IsActive = false;

                db.Orders.Add(order);
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

        // GET: Order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!car.CheckAccessPermission("Order", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Order order = (from c in db.Orders where c.OrderID == id select c).FirstOrDefault();
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers.ToList(), "CustomerID", "Name", order.CustomerID);
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,OrderNo,Date,IsActive,CustomerID")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
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

        // GET: Order/EditPrice/5
        public ActionResult EditPrice(int? id)
        {
            if (!car.CheckAccessPermission("Order", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Order order = (from c in db.Orders where c.OrderID == id select c).FirstOrDefault();
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // POST: Order/EditPrice/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPrice([Bind(Include = "OrderID,OrderNo,Date,IsActive,CustomerID")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
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

        // GET: Order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!car.CheckAccessPermission("Order", "IsDelete"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Order order = (from c in db.Orders where c.OrderID == id select c).FirstOrDefault();
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var orderDetails = (from c in db.Carts where c.OrderID == id select c).ToList();
            if (orderDetails.Count > 0)
            {
                foreach (var item in orderDetails)
                {
                    db.Carts.Remove(item);
                    db.SaveChanges();
                }
            }
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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

    public class OrderGridData
    {
        public int ID { get; set; }
        public string OrderNo { get; set; }
        public string Date { get; set; }
        public DateTime? Date_DT { get; set; }
        public decimal TotalPrice { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
        public string Detail { get; set; }
        public string Action { get; set; }
        public List<Cart> OrderDetails { get; set; }

    }
    //public class OrderCSV
    //{
    //    public string OrderNo { get; set; }
    //    public string Rivision { get; set; }
    //    public string Date { get; set; }
    //    public string Customer { get; set; }
    //    public string TotalPrice { get; set; }
    //    public string Add { get; set; }
    //    public string Less { get; set; }
    //    public string userName { get; set; }
    //}
}
