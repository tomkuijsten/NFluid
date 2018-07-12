using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NFluid.Tests
{
    [TestClass]
    public class FlowTests
    {
        [TestMethod]
        public void BasicFlow()
        {
            string input = "";

            object output =
                input
                    .StartFlow()
                    .Call((i) => 42)
                    .Return();

            Assert.AreEqual(42, output);
        }

        [TestMethod]
        public void CallChainFlow()
        {
            string input = "";

            object output =
                input
                    .StartFlow()
                    .Call((i) => 42)
                    .Call((i) => "42")
                    .Call((i) => DateTime.Now)
                    .Call((i) => i.Year)
                    .Return();

            Assert.AreEqual(DateTime.Now.Year, output);
        }

        [TestMethod]
        public void InjectParameters()
        {
            string input = "";

            object output =
                input
                    .StartFlow()
                    .AddParameters(DateTime.Now)
                    .Call((string i, DateTime dt) => dt.Year)
                    .Return();

            Assert.AreEqual(DateTime.Now.Year, output);
        }

        [TestMethod]
        public void CatchAndIgnoreException()
        {
            string input = "";

            object output =
                input
                    .StartFlow()
                    .Call((i) => { throw new ArgumentException("41"); return 41; })
                    .Catch<Exception>()
                    .Return();

            Assert.AreEqual(DateTime.Now.Year, output);
        }
    }
}
