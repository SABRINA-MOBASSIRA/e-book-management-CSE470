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
using System.Web.Script.Serialization;

namespace EBM.Controllers
{
    public class CartController : Controller
    {
        CheckAccessRoll car = new CheckAccessRoll();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cart
        public ActionResult Index(int? id)
        {
            //if (!car.CheckAccessPermission("Order", "IsRead"))
            //{
            //    string URL = Request.UrlReferrer.ToString();
            //    Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
            //    return Redirect(URL);
            //}

            int? orderID = (id != null) ? id : TempData.Peek("OrderID") as int?;
            var prods = (from m in db.Products
                         select new
                         {
                             ProductID = m.ProductID,
                             Name = m.Name
                         }).ToList();
            var jsonSerialiser = new JavaScriptSerializer();
            var prodslst = (from c in prods select c.Name).ToList();
            ViewBag.ProductlistJson = jsonSerialiser.Serialize(prodslst);
            ViewBag.ProductID = new SelectList(prods, "ProductID", "Name");
            ViewBag.OrderID = orderID;
            return View("Index");
        }
        [HttpPost]
        public JsonResult AutocompleteSuggestions(string term)
        {
            var prods = (from m in db.Products
                         select new
                         {
                             Name = m.Name
                         }).ToList();
            var suggestions = (from c in prods where c.Name.ToLower().Contains(term.ToLower()) select c).ToArray();
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDetailsByProduct(string name)
        {
            var prods = (from m in db.Products
                         select new
                         {
                             ProductID = m.ProductID,
                             Name = m.Name
                         }).ToList();
            int productID = (from c in prods where c.Name == name select c.ProductID).FirstOrDefault();
            ProductDetails pod = new ProductDetails();
            var po = (from pro in db.Products where pro.ProductID == productID select pro).FirstOrDefault();
            pod.UnitPrice = po.Price;
            return Json(pod, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTotalPrice(string name, int count)
        {
            var prods = (from m in db.Products
                         select new
                         {
                             ProductID = m.ProductID,
                             Name = m.Name
                         }).ToList();
            int productID = (from c in prods where c.Name == name select c.ProductID).FirstOrDefault();
            ProductDetails pod = new ProductDetails();
            var po = (from pro in db.Products where pro.ProductID == productID select pro).FirstOrDefault();
            pod.TotalPrice = po.Price * count;
            return Json(pod, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetTotalPriceByProductID(int id, int count)
        {
            var prods = (from m in db.Products
                         select new
                         {
                             ProductID = m.ProductID,
                             Name = m.Name
                         }).ToList();
            ProductDetails pod = new ProductDetails();
            var po = (from pro in db.Products where pro.ProductID == id select pro).FirstOrDefault();
            pod.TotalPrice = po.Price * count;
            return Json(pod, JsonRequestBehavior.AllowGet);
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
                                 Discount = 0,
                                 Unit = "Pcs",
                                 Action = "<a href='/Cart/Edit/" + l.CartID + "' class='btn btn-info btn-xs'><i class='fa fa-pencil'></i> Edit </a>" +
                                 "<a href='/Cart/Delete/" + l.CartID + "' class='btn btn-danger btn-xs'><i class='fa fa-trash - o'></i> Delete </a>"
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
        private List<CartGridData> SortByColumnWithOrder(string order, string orderDir, List<CartGridData> data)
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

        // GET: Cart/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Cart/Create
        public ActionResult Create()
        {
            if (!car.CheckAccessPermission("Order", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name");
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderNo");
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CartID,Quantity,UnitPrice,TotalPrice,OrderID,ProductID,Status")] Cart cart,FormCollection form)
        {
            try
            {
                var productName = form.GetValues("hidProductName");
                var prods = (from m in db.Products
                             select new
                             {
                                 ProductID = m.ProductID,
                                 Name = m.Name
                             }).ToList();
                int productID = (from c in prods where c.Name == productName[0].ToString() select c.ProductID).FirstOrDefault();
                cart.ProductID = productID;
            }
            catch { }

            if (ModelState.IsValid)
            {
                if (!CheckProductAlreadyExist(cart.ProductID, cart.OrderID))
                {
                    db.Carts.Add(cart);
                    int res = db.SaveChanges();
                    TempData["OderID"] = cart.OrderID;
                    TempData.Keep("OrderID");
                    //return RedirectToAction("Index");
                    if (res > 0)
                        return Json("success", JsonRequestBehavior.AllowGet);
                    else
                        return Json("error", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["OrderID"] = cart.OrderID;
                    TempData.Keep("OrderID");
                    return Json("ProductAlreadyExist", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("invalid", JsonRequestBehavior.AllowGet);
            }
        }

        private bool CheckProductAlreadyExist(int? prodid, int? orderID)
        {
            bool ret = false;
            var odrDts = (from prdm in db.Carts where (prdm.ProductID == prodid && prdm.OrderID == orderID) select prdm).FirstOrDefault();
            if (odrDts != null)
                ret = true;

            return ret;
        }

        // GET: Cart/Edit/5
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
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            var prods = (from m in db.Products
                         select new
                         {
                             ProductID = m.ProductID,
                             Name = m.Name
                         }).ToList();
            ViewBag.ProductID = new SelectList(prods, "ProductID", "Name", cart.ProductID);
            //ViewBag.OrderID = cart.OrderID;
            return View(cart);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CartID,Quantity,UnitPrice,TotalPrice,OrderID,ProductID,Status")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                if (!CheckSameProductExist(cart.ProductID, cart.OrderID))
                {
                    try
                    {
                        db.Entry(cart).State = EntityState.Modified;
                    }
                    catch
                    {
                        //db.Carts.Add(cart);

                    }
                    int res = db.SaveChanges();
                    TempData["OrderID"] = cart.OrderID;
                    TempData.Keep("OrderID");
                    //return RedirectToAction("Index");
                    if (res > 0)
                        return Json("success", JsonRequestBehavior.AllowGet);
                    else
                        return Json("error", JsonRequestBehavior.AllowGet); 
                }
                else
                {
                    TempData["OderID"] = cart.OrderID;
                    TempData.Keep("OrderID");
                    return Json("ProductAlreadyExist", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("invalid", JsonRequestBehavior.AllowGet);
            }
        }
        private bool CheckSameProductExist(int? productid, int? orderID)
        {
            bool ret = false;
            var orderDts = (from prdm in db.Carts.AsNoTracking() where (prdm.OrderID == orderID) select prdm).ToList();
            if (orderDts.Count() > 0)
            {
                int cnt = 0;
                foreach (var item in orderDts)
                {
                    if (item.ProductID == productid)
                        cnt++;
                }
                if (cnt > 1)
                    ret = true;
            }

            return ret;
        }

        // GET: Cart/Delete/5
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
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cart cart = db.Carts.Find(id);
            db.Carts.Remove(cart);
            TempData["OrderID"] = cart.OrderID;
            TempData.Keep("OrderID");
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

    public class CartGridData
    {
        public int? ID { get; set; }
        public string Product { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? Discount { get; set; }
        public string Unit { get; set; }
        public string Action { get; set; }

    }
    public class ProductDetails
    {
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
