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
    public class OrderControllerTest
    {
        [TestMethod]
        public void TestDetails()
        {
            var controller = new OrderController();
            var result = controller.Details(2) as ViewResult;
            var model = result.Model as Order;
            Assert.AreEqual("EBM/ODR/000001", model.OrderNo);
        }
        [TestMethod]
        public void TestIndex()
        {
            var controller = new OrderController();
            var result = controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public void TestCreate()
        {
            var db = new ApplicationDbContext();
            Order order = new Order { OrderNo ="Test", Date= DateTime.Now, CustomerID = 3 };
            var controller = new OrderController();
            var result = controller.Create(order) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestEdit()
        {
            var db = new ApplicationDbContext();
            Order order = db.Orders.AsNoTracking().FirstOrDefault();
            var controller = new OrderController();
            var result = controller.Edit(order) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestDelete()
        {
            var db = new ApplicationDbContext();
            Order order = db.Orders.Where(o=>o.OrderNo == "EBM/ODR/000003").AsNoTracking().FirstOrDefault();
            var controller = new OrderController();
            var result = controller.DeleteConfirmed(order.OrderID) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }

        [TestMethod]
        public void TestAcceptOrder()
        {
            var db = new ApplicationDbContext();
            var controller = new OrderController();
            Order order = db.Orders.Where(o => o.OrderNo == "EBM/ODR/000003").AsNoTracking().FirstOrDefault();
            var result = controller.AcceptOrder(order.OrderID) as ViewResult;
            //var model = result.Data as ProductDetails;
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public void TestRejectOrder()
        {
            var db = new ApplicationDbContext();
            var controller = new OrderController();
            Order order = db.Orders.Where(o => o.OrderNo == "EBM/ODR/000003").AsNoTracking().FirstOrDefault();
            var result = controller.RejectOrder(order.OrderID) as ViewResult;
            //var model = result.Data as ProductDetails;
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public void TestGetTotalPrice()
        {
            var db = new ApplicationDbContext();
            var controller = new OrderController();
            List<Cart> cart = db.Carts.Where(o => o.Order.OrderNo == "EBM/ODR/000001").AsNoTracking().ToList();
            var result = controller.GetTotalPrice(cart);
            //var model = result.Data as ProductDetails;
            Assert.AreEqual((decimal)1410.0000, result);
        }
    }
}