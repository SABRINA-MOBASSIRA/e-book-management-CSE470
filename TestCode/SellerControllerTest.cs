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
    public class SellerControllerTest
    {
        [TestMethod]
        public void TestDetails()
        {
            var controller = new SellerController();
            var result = controller.Details(3) as ViewResult;
            var model = result.Model as Seller;
            Assert.AreEqual("Sabrina Mobassira", model.Name);
        }
        [TestMethod]
        public void TestIndex()
        {
            var controller = new SellerController();
            var result = controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public void TestCreate()
        {
            Seller customer = new Seller { Name = "Sabrina", PhoneNumber = "01715321061", EmailAddress = "sabrinamobassira@gmail.com" };
            var seller = new SellerController();
            var result = seller.Create(customer) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestEdit()
        {
            var db = new ApplicationDbContext();
            Seller seller = db.Sellers.Where(p => p.Name == "Sabrina").AsNoTracking().FirstOrDefault();
            var controller = new SellerController();
            var result = controller.Edit(seller) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestDelete()
        {
            var db = new ApplicationDbContext();
            Seller seller = db.Sellers.Where(p => p.Name == "Sabrina").AsNoTracking().FirstOrDefault();
            var controller = new SellerController();
            var result = controller.DeleteConfirmed(seller.SellerID) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
    }
}