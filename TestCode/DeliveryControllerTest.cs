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
    public class DeliveryControllerTest
    {
        [TestMethod]
        public void TestDetails()
        {
            var controller = new DeliveryController();
            var result = controller.Details(2) as ViewResult;
            var model = result.Model as Delivery;
            Assert.AreEqual("EBM/DL/0001", model.ChallanNo);
        }
        [TestMethod]
        public void TestIndex()
        {
            var controller = new DeliveryController();
            var result = controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public void TestCreate()
        {
            var db = new ApplicationDbContext();
            Delivery delivery = new Delivery { ChallanNo ="Test", DeliveryDate=DateTime.Now, Quantity=0, TotalPrice = 0, Address="Dhaka", CustomerID = 2, OrderID=2, IsActive = false, Status = "" };
            var controller = new DeliveryController();
            var result = controller.Create(delivery) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestEdit()
        {
            var db = new ApplicationDbContext();
            Delivery delivery = db.Deliveries.AsNoTracking().FirstOrDefault();
            var controller = new DeliveryController();
            var result = controller.Edit(delivery) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestDelete()
        {
            var db = new ApplicationDbContext();
            Delivery delivery = db.Deliveries.Where(o => o.ChallanNo == "EBM/DL/0003").AsNoTracking().FirstOrDefault();
            var controller = new DeliveryController();
            var result = controller.DeleteConfirmed(delivery.DeliveryID) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }

    }
}