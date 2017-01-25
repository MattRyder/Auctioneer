using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Auctioneer.Controllers;

namespace Auctioneer.Tests.Controllers
{
    [TestClass]
    public class BaseControllerTest
    {
        [TestMethod]
        public void GetFlashKey()
        {
            BaseController controller = new BaseController();

            string flashKeySuccess = controller.GetFlashKey(Models.FlashKeyType.Success);
            Assert.AreEqual("alert-Success", flashKeySuccess);

            string flashKeyDanger = controller.GetFlashKey(Models.FlashKeyType.Danger);
            Assert.AreEqual("alert-Danger", flashKeyDanger);

            string flashKeyWarning = controller.GetFlashKey(Models.FlashKeyType.Warning);
            Assert.AreEqual("alert-Warning", flashKeyWarning);

            string flashKeyInfo = controller.GetFlashKey(Models.FlashKeyType.Info);
            Assert.AreEqual("alert-Info", flashKeyInfo);
        }
    }
}