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
using Microsoft.AspNet.Identity;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Text;

namespace EBM.Controllers
{
    public class DeliveryController : Controller
    {
        CheckAccessRoll car = new CheckAccessRoll();
        private ApplicationDbContext db = new ApplicationDbContext();
        public List<VehicleType> GetVehicleTypes()
        {
            List<VehicleType> vt = new List<VehicleType>();
            vt.Add(new VehicleType { Name = "Truck", Value = "Truck" });
            vt.Add(new VehicleType { Name = "Cuvered Van", Value = "Cuvered Van" });
            vt.Add(new VehicleType { Name = "Pickup", Value = "Pickup" });
            vt.Add(new VehicleType { Name = "Riksha", Value = "Riksha" });

            return vt;
        }

        [HttpPost]
        public JsonResult GetDetailsByOrder(int id)
        {
            OrderDetails pod = new OrderDetails();
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var po = (from pro in db.Orders where pro.OrderID == id select pro).FirstOrDefault();
            pod.CustomerID = po.CustomerID;
            //decimal add = (decimal)(((po.InstalationCost != null) ? po.InstalationCost : 0) + ((po.OtherCost != null) ? po.OtherCost : 0) + ((po.VAT != null) ? po.VAT : 0) + ((po.ATI != null) ? po.ATI : 0));
            //decimal vat = (decimal)((po.VAT != null) ? po.VAT : 0);
            //decimal less = (decimal)((po.Discount != null) ? po.Discount : 0);
            //pod.PricewithAllVAT = (GetTotalPrice(po.SalesQuotationDetails.ToList()) + add - (decimal)((GetTotalPrice(po.SalesQuotationDetails.ToList()) * less) / 100));
            pod.TotalPrice = GetTotalPrice(po.Carts.ToList());
            //pod.VAT = vat;

            return Json(pod, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult DeliveryChallanRpt(int id)
        //{
        //    string username = System.Web.HttpContext.Current.User.Identity.GetUserName();
        //    List<DeliveryChallanReport> bcrList = new List<DeliveryChallanReport>();
        //    var dcs = (from bc in db.DeliveryChallans where bc.DeliveryChallanID == id select bc).FirstOrDefault();
        //    var bcs = (from bc in db.SalesQuotations where bc.SalesQuotationID == dcs.SalesQuotationID select bc).FirstOrDefault();
        //    var details = (from c in db.SalesQuotationDetails where c.SalesQuotationID == bcs.SalesQuotationID select c).ToList();
        //    string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    var profile = (from c in db.Profiles where c.UserID == userID select c).FirstOrDefault();
        //    foreach (var item in details)
        //    {
        //        DeliveryChallanReport bcr = new DeliveryChallanReport();
        //        bcr.ImagePath = Server.MapPath(item.Product.ImagePath);
        //        bcr.ProductModel = item.Product.ProductModel.ModelNo;
        //        bcr.ChallanDate = Convert.ToDateTime(dcs.ChallanDate);
        //        bcr.ProductName = item.Product.Name;
        //        bcr.ProductDescription = item.Product.Description;
        //        bcr.Quantity = Convert.ToDecimal(item.Quantity);
        //        bcr.QuotationNo = bcs.QuotationNo;
        //        bcr.ChallanNo = dcs.ChallanNo;
        //        bcr.VehicleType = dcs.VehicleType;
        //        bcr.VehicleNo = dcs.VehicleNo;
        //        bcr.VATRegistrationNo = dcs.VATRegistrationNumber;
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
        //        bcr.Areacode = "190303";
        //        //bcr.UserName = username;
        //        //bcr.Designation = 
        //        bcr.ProfileName = (!string.IsNullOrEmpty(profile.FirstName)) ? profile.FirstName + " " + profile.LastName : profile.LastName;
        //        bcr.ProfileTitle = profile.Title;
        //        bcr.ProfileDepartment = profile.Department;
        //        bcr.ProfileSignaturePath = Server.MapPath(profile.ImagePath);
        //        bcr.ProfileMobile = profile.Mobile;
        //        bcr.ProfileEmail = profile.EmailAddress;
        //        bcrList.Add(bcr);
        //    }
        //    Stream stream = null;
        //    string type = "";
        //    string filename = "";

        //    using (ReportDocument rd = new ReportDocument())
        //    {
        //        rd.Load(Server.MapPath("/Report/DeliveryChallanRpt.rpt"));
        //        rd.SetDataSource(bcrList);
        //        Response.Buffer = false;
        //        Response.ClearContent();
        //        Response.ClearHeaders();


        //        stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    }
        //    stream.Seek(0, SeekOrigin.Begin);
        //    type = "application/pdf";
        //    filename = "DeliveryChallanRpt.pdf";

        //    stream.Seek(0, SeekOrigin.Begin);
        //    return File(stream, type, filename);
        //}
        //public ActionResult SalesInvoiceRpt(int id)
        //{
        //    List<DeliveryChallanReport> bcrList = new List<DeliveryChallanReport>();
        //    var sis = (from bc in db.SalesInvoices where bc.DeliveryChallanID == id select bc).FirstOrDefault();
        //    var dcs = (from bc in db.DeliveryChallans where bc.DeliveryChallanID == id select bc).FirstOrDefault();
        //    var bcs = (from bc in db.SalesQuotations where bc.SalesQuotationID == dcs.SalesQuotationID select bc).FirstOrDefault();
        //    var details = (from c in db.SalesQuotationDetails where c.SalesQuotationID == bcs.SalesQuotationID select c).ToList();
        //    string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    var profile = (from c in db.Profiles where c.UserID == userID select c).FirstOrDefault();
        //    foreach (var item in details)
        //    {
        //        DeliveryChallanReport bcr = new DeliveryChallanReport();
        //        bcr.ImagePath = Server.MapPath(item.Product.ImagePath);
        //        bcr.ProductModel = item.Product.ProductModel.ModelNo;
        //        bcr.ChallanDate = Convert.ToDateTime(dcs.ChallanDate);
        //        bcr.InvoiceDate = Convert.ToDateTime(sis.InvoiceDate);
        //        bcr.ProductName = item.Product.Name;
        //        bcr.ProductDescription = item.Product.Description;
        //        bcr.Quantity = Convert.ToDecimal(item.Quantity);
        //        bcr.QuotationNo = bcs.QuotationNo;
        //        bcr.InvoiceNo = sis.InvoiceNo;
        //        bcr.ChallanNo = dcs.ChallanNo;
        //        bcr.VehicleType = dcs.VehicleType;
        //        bcr.VehicleNo = dcs.VehicleNo;
        //        bcr.VATRegistrationNo = dcs.VATRegistrationNumber;
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
        //        bcr.GrossTotalAmount = Convert.ToDecimal(sis.GrossTotalAmount);
        //        bcr.AddAmount = Convert.ToDecimal(sis.AddAmount);
        //        bcr.NetAmount = Convert.ToDecimal(sis.NetAmount);
        //        bcr.AdvanceAmount = Convert.ToDecimal(sis.AdvanceAmount);
        //        bcr.ProfileName = (!string.IsNullOrEmpty(profile.FirstName)) ? profile.FirstName + " " + profile.LastName : profile.LastName;
        //        bcr.ProfileTitle = profile.Title;
        //        bcr.ProfileDepartment = profile.Department;
        //        bcr.ProfileSignaturePath = Server.MapPath(profile.ImagePath);
        //        bcr.ProfileMobile = profile.Mobile;
        //        bcr.ProfileEmail = profile.EmailAddress;
        //        bcrList.Add(bcr);
        //    }

        //    Stream stream = null;
        //    string type = "";
        //    string filename = "";

        //    using (ReportDocument rd = new ReportDocument())
        //    {
        //        rd.Load(Server.MapPath("/Report/SalesInvoiceRpt.rpt"));
        //        rd.SetDataSource(bcrList);
        //        Response.Buffer = false;
        //        Response.ClearContent();
        //        Response.ClearHeaders();


        //        stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    }
        //    stream.Seek(0, SeekOrigin.Begin);
        //    type = "application/pdf";
        //    filename = "SalesInvoiceRpt.pdf";

        //    stream.Seek(0, SeekOrigin.Begin);
        //    return File(stream, type, filename);
        //}

        public ActionResult DoneDeliveryChallan(int? id)
        {
            if (!car.CheckAccessPermission("Done Delivery", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var deliveryChallan = (from c in db.Deliveries where c.DeliveryID == id select c).FirstOrDefault();
            deliveryChallan.IsActive = true;
            db.Entry(deliveryChallan).State = EntityState.Modified;
            int res = db.SaveChanges();
            if (res==1)
            {
                Payment pment = new Payment();
                //pment.AddAmount = (decimal)((deliveryChallan.SalesQuotation.InstalationCost == null) ? 0 : deliveryChallan.SalesQuotation.InstalationCost) + (decimal)((deliveryChallan.SalesQuotation.ATI == null) ? 0 : deliveryChallan.SalesQuotation.ATI);
                pment.CustomerID = deliveryChallan.CustomerID;
                pment.DeliveryID = deliveryChallan.DeliveryID;
                pment.TotalAmount = deliveryChallan.TotalPrice;
                pment.InvoiceDate = DateTime.Now;
                var sql = (from dc in db.Payments select dc).ToList();
                int max = (sql.Count() > 0) ? sql.Max(p => p.DeliveryID) : 0;
                pment.InvoiceNo = "EBM/Inv/000" + (max + 1).ToString();
                //pment.LessAmount = deliveryChallan.SalesQuotation.Discount;
                //pment.NetAmount = (deliveryChallan.PricewithAllVAT - pment.AddAmount);
                //int? toBePaid = (deliveryChallan.SalesQuotation.ToBePaid == null) ? 0 : deliveryChallan.SalesQuotation.ToBePaid;
                //pment.AdvanceAmount = (toBePaid == 0) ? 0 : ((pment.GrossTotalAmount * toBePaid)/100);
                db.Payments.Add(pment);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public ActionResult UnDoneDeliveryChallan(int? id)
        {
            if (!car.CheckAccessPermission("Un Done Delivery", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var deliveryChallan = (from c in db.Deliveries where c.DeliveryID == id select c).FirstOrDefault();
            deliveryChallan.IsActive = false;
            db.Entry(deliveryChallan).State = EntityState.Modified;
            int res = db.SaveChanges();
            if (res == 1)
            {
                var payment = (from c in db.Payments where c.DeliveryID == deliveryChallan.DeliveryID select c).FirstOrDefault();
                if (payment != null)
                {
                    db.Payments.Remove(payment);
                    db.SaveChanges(); 
                }
            }
            return RedirectToAction("Index");
        }

        //public FileContentResult DownloadCSV()
        //{
        //    string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    string userName = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    var allUser = (from c in db.Users select c).ToList();
        //    var useradmin = (from c in db.UserRolls where c.UserID == userID && c.UserGroup.GroupName.ToLower() == "admin" select c).FirstOrDefault();
        //    var data = ((from l in db.DeliveryChallans
        //                 select new DeliveryChallanGridData
        //                 {
        //                     ID = l.DeliveryChallanID,
        //                     ChallanNo = l.ChallanNo,
        //                     ChallanDate = l.ChallanDate.ToString(),
        //                     QuotationNo = l.SalesQuotation.QuotationNo,
        //                     Date_DT = l.ChallanDate,
        //                     Customer = l.Customer.Name,
        //                     Add = (((l.SalesQuotation.InstalationCost != null) ? l.SalesQuotation.InstalationCost : 0) + ((l.SalesQuotation.OtherCost != null) ? l.SalesQuotation.OtherCost : 0) + ((l.SalesQuotation.VAT != null) ? l.SalesQuotation.VAT : 0) + ((l.SalesQuotation.ATI != null) ? l.SalesQuotation.ATI : 0)),
        //                     Less = ((l.SalesQuotation.Discount != null) ? l.SalesQuotation.Discount : 0),
        //                     QuotionDetails = l.SalesQuotation.SalesQuotationDetails.ToList(),
        //                     userid = l.UserID,

        //                 }).OrderBy(l => l.Date_DT)).OrderByDescending(l => l.Date_DT).AsEnumerable().Select(c => new DeliveryChallanCSV
        //                 {
        //                     ChallanNo = c.ChallanNo,
        //                     ChallanDate = c.ChallanDate.ToString(),
        //                     QuotationNo = c.QuotationNo,
        //                     Customer = c.Customer,
        //                     Add = c.Add.ToString(),
        //                     Less = c.Less.ToString(),
        //                     TotalPrice = (GetTotalPrice(c.QuotionDetails.ToList()) + c.Add - (decimal)((GetTotalPrice(c.QuotionDetails.ToList()) * c.Less) / 100)).ToString(),
        //                     userName = allUser.Where(u => u.Id == c.userid).Select(u => u.UserName).FirstOrDefault(),
        //                 }).ToList();

        //    data = (useradmin != null) ? data : (data.Where(c => c.userName == userName)).ToList();

        //    List<object> dChallan = (from q in data
        //                             select new[]
        //                             {
        //                                  q.ChallanNo,
        //                                  q.ChallanDate,
        //                                  q.QuotationNo,
        //                                  q.Customer,
        //                                  q.Add.ToString(),
        //                                  q.Less.ToString(),
        //                                  q.TotalPrice,
        //                                  q.userName
        //                              }).ToList<object>();

        //    //Insert the Column Names.
        //    dChallan.Insert(0, new string[8] { "Challan No", "ChallanDate", "QuotationNo", "Customer", "Add", "Less", "Total Price", "User Name" });

        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < dChallan.Count; i++)
        //    {
        //        string[] dchal = (string[])dChallan[i];
        //        for (int j = 0; j < dchal.Length; j++)
        //        {
        //            //Append data with separator.
        //            sb.Append(dchal[j] + ',');
        //        }
        //        //Append new line character.
        //        sb.Append("\r\n");
        //    }

        //    return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "DeliveryChallan.csv");
        //}

        // GET: Delivery
        public ActionResult Index()
        {
            if (!car.CheckAccessPermission("Delivery", "IsRead"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            return View();
        }

        #region Grid Machenism
        [HttpPost]
        public JsonResult GetDeliveryChallans()
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
                var data = ((from l in db.Deliveries
                             //where l.UserID == userID
                             select new DeliveryChallanGridData
                             {
                                 ID = l.DeliveryID,
                                 ChallanNo = l.ChallanNo,
                                 DeliveryDate = l.DeliveryDate.ToString(),
                                 OrderNo = l.Order.OrderNo,
                                 Date_DT = l.DeliveryDate,
                                 Customer = l.Customer.Name,
                                 //Add = (((l.SalesQuotation.InstalationCost != null) ? l.SalesQuotation.InstalationCost : 0) + ((l.SalesQuotation.OtherCost != null) ? l.SalesQuotation.OtherCost : 0) + ((l.SalesQuotation.VAT != null) ? l.SalesQuotation.VAT : 0) + ((l.SalesQuotation.ATI != null) ? l.SalesQuotation.ATI : 0)),
                                 //Less = ((l.SalesQuotation.Discount != null) ? l.SalesQuotation.Discount : 0),
                                 OrderDetails = l.Order.Carts.ToList(),
                                 //userid = l.UserID,
                                 Detail = "<a href='/Delivery/OrderDetail/" + l.OrderID + "' class='btn btn-primary btn-xs'><i class='fa fa-list'></i> Details </a>",
                                 //"<a href='/SalesQuotation/EditPrice/" + l.SalesQuotationID + "' class='btn btn-primary btn-xs'><i class='fa fa-money'></i> Price </a>",
                                 Action = "<a href='/Delivery/Details/" + l.DeliveryID + "' class='btn btn-primary btn-xs'><i class='fa fa-folder'></i> View </a>" +
                                 "<a href='/Delivery/Edit/" + l.DeliveryID + "' class='btn btn-info btn-xs'><i class='fa fa-pencil'></i> Edit </a>" +
                                 "<a href='/Delivery/Delete/" + l.DeliveryID + "' class='btn btn-danger btn-xs'><i class='fa fa-trash - o'></i> Delete </a>" +
                                 ((l.IsActive == true) ? "<a href='/Delivery/UnDoneDeliveryChallan/" + l.DeliveryID + "' class='btn btn-default btn-xs'><i class='fa fa-close'></i> Payment Reject </a>" : "<a href='/Delivery/DoneDeliveryChallan/" + l.DeliveryID + "' class='btn btn-default btn-xs'><i class='fa fa-check-circle-o'></i> Payment Receive </a>")
                                 //"<a href='/DeliveryChallan/DeliveryChallanRpt/" + l.DeliveryID + "' class='btn btn-default btn-xs'><i class='fa fa-print'></i> Print Challan </a>"+
                                 //((l.IsActive == true) ? "<a href='/DeliveryChallan/SalesInvoiceRpt/" + l.DeliveryID + "' class='btn btn-default btn-xs'><i class='fa fa-print'></i> Print Invoice </a>" : "")
                             }).AsEnumerable().Select(c => new DeliveryChallanGridData
                             {
                                 ID = c.ID,
                                 ChallanNo = c.ChallanNo,
                                 DeliveryDate = c.DeliveryDate.ToString(),
                                 OrderNo = c.OrderNo,
                                 Date_DT = c.Date_DT,
                                 Customer = c.Customer,
                                 //Add = c.Add,
                                 //Less = c.Less,
                                 TotalPrice = (c.OrderDetails != null) ? GetTotalPrice(c.OrderDetails.ToList()) : 0,
                                 Detail = c.Detail,
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
                    List<DeliveryChallanGridData> searchData = new List<DeliveryChallanGridData>();
                    foreach (var item in data)
                    {
                        if (((!string.IsNullOrEmpty(item.ChallanNo)) ? item.ChallanNo.ToString().ToLower().Contains(search.ToLower()) : false) ||
                            ((!string.IsNullOrEmpty(item.Customer)) ? item.Customer.ToString().ToLower().Contains(search.ToLower()) : false))
                        {
                            searchData.Add(item);
                        }
                    }
                    if (searchData.Count() > 0)
                    {
                        data = new List<DeliveryChallanGridData>();
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
        private List<DeliveryChallanGridData> SortByColumnWithOrder(string order, string orderDir, List<DeliveryChallanGridData> data)
        {
            // Initialization.   
            List<DeliveryChallanGridData> lst = new List<DeliveryChallanGridData>();
            try
            {
                // Sorting   
                switch (order)
                {
                    case "0":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.ChallanNo).ToList() : data.OrderBy(p => p.ChallanNo).ToList();
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

        public ActionResult OrderDetail(int? id)
        {
            if (!car.CheckAccessPermission("Delivery", "IsRead"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            ViewBag.OrderID = id;
            return View();
        }

        #region Grid Machenism
        [HttpPost]
        public JsonResult GetCarts(int id)
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
                var data = ((from l in db.Carts
                             where l.OrderID == id
                             select new CartGridData
                             {
                                 ID = l.CartID,
                                 Product = l.Product.Name,
                                 Quantity = l.Quantity,
                                 UnitPrice = l.UnitPrice,
                                 TotalPrice = (l.TotalPrice != null) ? l.TotalPrice : (l.UnitPrice * l.Quantity),
                                 //Unit = l.Product.MesurmentUnit.Name
                             }).OrderBy(l => l.Product)).ToList();
                // Total record count.   
                int totalRecords = data.Count;
                // Verification.   
                if (!string.IsNullOrEmpty(search) &&
                    !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    List<CartGridData> searchData = new List<CartGridData>();
                    foreach (var item in data)
                    {
                        if (((!string.IsNullOrEmpty(item.Product)) ? item.Product.ToString().ToLower().Contains(search.ToLower()) : false))
                        {
                            searchData.Add(item);
                        }
                    }
                    if (searchData.Count() > 0)
                    {
                        data = new List<CartGridData>();
                        data = searchData;
                    }
                }
                // Sorting.   
                data = this.SortByColumnWithOrderQuotationDetails(order, orderDir, data);
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
        private List<CartGridData> SortByColumnWithOrderQuotationDetails(string order, string orderDir, List<CartGridData> data)
        {
            // Initialization.   
            List<CartGridData> lst = new List<CartGridData>();
            try
            {
                // Sorting   
                switch (order)
                {
                    case "0":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Product).ToList() : data.OrderBy(p => p.Product).ToList();
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


        public decimal GetTotalPrice(List<Cart> carts)
        {
            decimal tPrice = 0;
            foreach (var item in carts)
            {
                tPrice = tPrice + Convert.ToDecimal(item.TotalPrice);
            }

            return tPrice;
        }

        // GET: Delivery/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Delivery deliveryChallan = (from c in db.Deliveries where c.DeliveryID == id select c).FirstOrDefault();
            if (deliveryChallan == null)
            {
                return HttpNotFound();
            }
            return View(deliveryChallan);
        }

        // GET: Delivery/Create
        public ActionResult Create()
        {
            if (!car.CheckAccessPermission("Delivery", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ViewBag.CustomerID = new SelectList(db.Customers.ToList(), "CustomerID", "Name");
            var useradmin = (from c in db.UserRolls where c.UserID == userID && c.UserGroup.GroupName.ToLower() == "admin" select c).FirstOrDefault();
            var order = db.Orders.Where(c => c.IsActive == true).ToList();
            order = (useradmin != null) ? order : order;
            ViewBag.OrderID = new SelectList(order, "OrderID", "OrderNo");
            var sql = (from dc in db.Deliveries select dc).ToList();
            int max = (sql.Count() > 0) ? sql.Max(p => p.DeliveryID) : 0;
            ViewBag.ChallanNo = "EBM/DL/000" + (max + 1).ToString();
            //ViewBag.VehicleType = new SelectList(GetVehicleTypes(), "Name", "Value");
            return View();
        }

        // POST: Delivery/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeliveryID,ChallanNo,DeliveryDate,Quantity,TotalPrice,Address,CustomerID,OrderID,IsActive,Status")] Delivery deliveryChallan)
        {
            ModelState.Clear();
            deliveryChallan.TotalPrice = (decimal)((deliveryChallan.TotalPrice == null || deliveryChallan.TotalPrice == 0) ? (decimal)(0.00) : deliveryChallan.TotalPrice);
            //deliveryChallan.VAT = (decimal)((deliveryChallan.VAT == null || deliveryChallan.VAT == 0) ? (decimal)(0.00) : deliveryChallan.VAT);
            //deliveryChallan.OtherVAT = (decimal)(((deliveryChallan.OtherVAT == null || deliveryChallan.OtherVAT == 0) ? (decimal)(0.00) : deliveryChallan.OtherVAT));
            //deliveryChallan.PricewithAllVAT = (decimal)((deliveryChallan.PricewithAllVAT == null || deliveryChallan.PricewithAllVAT == 0) ? (decimal)(0.00) : deliveryChallan.PricewithAllVAT);
            var sql = (from dc in db.Deliveries select dc).AsNoTracking().ToList();
            int max = (sql.Count() > 0) ? sql.Max(p => p.DeliveryID) : 0;
            deliveryChallan.ChallanNo = "EBM/DL/000" + (max + 1).ToString();
            ValidateModel(deliveryChallan);
            if (ModelState.IsValid)
            {
                string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                db.Deliveries.Add(deliveryChallan);
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

        // GET: DeliveryChallan/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!car.CheckAccessPermission("Delivery", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Delivery deliveryChallan = (from c in db.Deliveries where c.DeliveryID == id select c).FirstOrDefault();
            if (deliveryChallan == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers.ToList(), "CustomerID", "Name", deliveryChallan.CustomerID);
            var orders = db.Orders.Where(c => c.IsActive == true).ToList();
            var useradmin = (from c in db.UserRolls where c.UserID == userID && c.UserGroup.GroupName.ToLower() == "admin" select c).FirstOrDefault();
            orders = (useradmin != null) ? orders : orders;
            ViewBag.SalesQuotationID = new SelectList(orders, "OrderID", "OrderNo", deliveryChallan.OrderID);
            //ViewBag.VehicleType = new SelectList(GetVehicleTypes(), "Name", "Value", deliveryChallan.VehicleType);
            return View(deliveryChallan);
        }

        // POST: Delivery/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeliveryID,ChallanNo,DeliveryDate,Quantity,TotalPrice,Address,CustomerID,OrderID,IsActive,Status")] Delivery deliveryChallan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deliveryChallan).State = EntityState.Modified;
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

        // GET: Delivery/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!car.CheckAccessPermission("Delivery", "IsDelete"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Delivery deliveryChallan = (from c in db.Deliveries where c.DeliveryID == id select c).FirstOrDefault();
            if (deliveryChallan == null)
            {
                return HttpNotFound();
            }
            return View(deliveryChallan);
        }

        // POST: Delivery/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Delivery deliveryChallan = db.Deliveries.Find(id);
            db.Deliveries.Remove(deliveryChallan);
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

    public class VehicleType
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class DeliveryChallanGridData
    {
        public int ID { get; set; }
        public string ChallanNo { get; set; }
        public string DeliveryDate { get; set; }
        public string OrderNo { get; set; }
        public DateTime? Date_DT { get; set; }
        public string Customer { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Detail { get; set; }
        public string Action { get; set; }
        public List<Cart> OrderDetails { get; set; }

    }
    public class OrderDetails
    {
        public int? CustomerID { get; set; }
        public decimal? TotalPrice { get; set; }
        //public decimal? OtherVAT { get; set; }
        //public decimal? VAT { get; set; }
        //public decimal? PricewithAllVAT { get; set; }
    }
    public class DeliveryChallanCSV
    {
        public string ChallanNo { get; set; }
        public string Rivision { get; set; }
        public string ChallanDate { get; set; }
        public string QuotationNo { get; set; }
        public string Customer { get; set; }
        public string TotalPrice { get; set; }
        public string Add { get; set; }
        public string Less { get; set; }
        public string userName { get; set; }
    }
}
