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
    public class ProductControllerTest
    {
        [TestMethod]
        public void TestDetails()
        {
            //var repo = new dbRepository();
            //var db = new ApplicationDbContext();
            var controller = new ProductController();
            //controller.db = new Models.ApplicationDbContext();
            var result = controller.Details(3) as ViewResult;
            var model = result.Model as Product;
            Assert.AreEqual("Java Concurrency in Practice 1st Edition", model.Name);
        }
        [TestMethod]
        public void TestIndex()
        {
            var controller = new ProductController();
            var result = controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public void TestCreate()
        {
            Product product = new Product {Name = "Test_Product", Price = 25};
            var controller = new ProductController();
            var result = controller.Create(product,null) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestEdit()
        {
            var db = new ApplicationDbContext();
            Product product = db.Products.Where(p=>p.Name == "Test_Product").AsNoTracking().FirstOrDefault();
            var controller = new ProductController();
            var result = controller.Edit(product, null) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestDelete()
        {
            var db = new ApplicationDbContext();
            Product product = db.Products.Where(p => p.Name == "Test_Product").AsNoTracking().FirstOrDefault();
            var controller = new ProductController();
            var result = controller.DeleteConfirmed(product.ProductID) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
    }
}