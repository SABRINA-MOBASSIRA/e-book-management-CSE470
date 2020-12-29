using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EBM.Controllers
{
    [TestClass]
    public class ProductControllerTest
    {
        [TestMethod]
        public void TestProductDetailsView()
        {
            var controller = new ProductController();
            var result = controller.Details(1) as ViewResult;
            Assert.AreEqual("Details", result.ViewName);
        }
    }
}