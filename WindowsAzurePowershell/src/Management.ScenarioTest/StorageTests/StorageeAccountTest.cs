
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.WindowsAzure.Management.ScenarioTest.StorageTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.ScenarioTest.Common;

    [TestClass]
    public class StorageAccountTest : WindowsAzurePowerShellTest
    {
        public StorageAccountTest()
            : base("Storage\\Common.ps1",
            "Storage\\StorageAccount.ps1")
        {
           
        }

        [TestMethod]
        [TestCategory(Category.All)]
        [TestCategory(Category.Storage)]
        public void TestNewGetSetAzureStorageAccountAndKeys()
        {
            RunPowerShellTest("Test-NewGetSetAzureStorageAccountAndKeys");
        }
    }

}
