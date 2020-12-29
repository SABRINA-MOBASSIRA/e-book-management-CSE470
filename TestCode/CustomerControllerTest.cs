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
    public class CustomerControllerTest
    {
        [TestMethod]
        public void TestDetails()
        {
            var controller = new CustomerController();
            var result = controller.Details(2) as ViewResult;
            var model = result.Model as Customer;
            Assert.AreEqual("Moon", model.Name);
        }
        [TestMethod]
        public void TestIndex()
        {
            var controller = new CustomerController();
            var result = controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public void TestCreate()
        {
            Customer customer = new Customer { Name="Test_Customer", PhoneNumber="01721008234", Address="Danasree", EmailAddress="atofael@gmail.com" };
            var controller = new CustomerController();
            var result = controller.Create(customer) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestEdit()
        {
            var db = new ApplicationDbContext();
            Customer customer = db.Customers.Where(p => p.Name == "Test_Customer").AsNoTracking().FirstOrDefault();
            var controller = new CustomerController();
            var result = controller.Edit(customer) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
        [TestMethod]
        public void TestDelete()
        {
            var db = new ApplicationDbContext();
            Customer customer = db.Customers.Where(p => p.Name == "Test_Customer").AsNoTracking().FirstOrDefault();
            var controller = new CustomerController();
            var result = controller.DeleteConfirmed(customer.CustomerID) as JsonResult;
            Assert.AreEqual("success", result.Data.ToString());
        }
    }
}