using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using EBM.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EBM.Controllers
{
    [TestClass]
    public class CartControllerTest
    {
        [TestMethod]
        public void TestDetails()
        {
            var controller = new CartController();
            var result = controller.Details(2) as ViewResult;
            var model = result.Model as Cart;
            Assert.AreEqual("Java Concurrency in Practice 1st Edition", model.Product.Name);
        }
        [TestMethod]
        public void TestIndex()
        {
            var controller = new CartController();
            var result = controller.Index(1) as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public void TestCreate()
        {
            var db = new ApplicationDbContext();
            Product product = db.Products.Where(p => p.Name == "Angels & Demons").AsNoTracking().FirstOrDefault();
            Cart cart = new Cart { Quantity = 5, UnitPrice = product.Price, TotalPrice = (product.Price * 5), OrderID = 3, ProductID = product.ProductID, Status = "" };
            var controller = new CartController();
            var result = controller.Create(cart, null) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestEdit()
        {
            var db = new ApplicationDbContext();
            Cart cart = db.Carts.AsNoTracking().FirstOrDefault();
            var controller = new CartController();
            var result = controller.Edit(cart) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestDelete()
        {
            var db = new ApplicationDbContext();
            Product product = db.Products.Where(p => p.Name == "Angels & Demons").AsNoTracking().FirstOrDefault();
            Cart cart = db.Carts.Where(c=>c.ProductID == product.ProductID && c.OrderID == 3).AsNoTracking().FirstOrDefault();
            var controller = new CartController();
            var result = controller.DeleteConfirmed(cart.CartID) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }

        [TestMethod]
        public void TestGetDetailsByProduct()
        {
            var db = new ApplicationDbContext();
            var controller = new CartController();
            var result = controller.GetDetailsByProduct("Java Concurrency in Practice 1st Edition") as JsonResult;
            var model = result.Data as ProductDetails;
            Assert.AreEqual((decimal)350.0000, model.UnitPrice);
        }
        [TestMethod]
        public void TestGetTotalPrice()
        {
            var db = new ApplicationDbContext();
            var controller = new CartController();
            var result = controller.GetTotalPrice("Java Concurrency in Practice 1st Edition", 1) as JsonResult;
            var model = result.Data as ProductDetails;
            Assert.AreEqual((decimal)350.0000, model.TotalPrice);
        }
        [TestMethod]
        public void TestGetTotalPriceByProductID()
        {
            var db = new ApplicationDbContext();
            var controller = new CartController();
            var result = controller.GetTotalPriceByProductID(3, 1) as JsonResult;
            var model = result.Data as ProductDetails;
            Assert.AreEqual((decimal)350.0000, model.TotalPrice);
        }
    }
}