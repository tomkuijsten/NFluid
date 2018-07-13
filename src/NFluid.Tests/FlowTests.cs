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
                    .Chain((i) => 42)
                    .Return();

            Assert.AreEqual(42, output);
        }

        [TestMethod]
        public void ChainChainFlow()
        {
            string input = "";

            object output =
                input
                    .StartFlow()
                    .Chain((i) => 42)
                    .Chain((i) => "42")
                    .Chain((i) => DateTime.Now)
                    .Chain((i) => i.Year)
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
                    .Register(12.3)
                    .Register(DateTime.Now)
                    .Chain((string i, DateTime dt) => dt.Year)
                    .Return();

            Assert.AreEqual(DateTime.Now.Year, output);
        }

        [TestMethod]
        public void InjectParameters_ByName()
        {
            string input = "";

            object output =
                input
                    .StartFlow()
                    .Register(41, "p1")
                    .Register(1, "p2")
                    .Chain((string i, int p1, int p2) => p1 + p2)
                    .Return();

            Assert.AreEqual(42, output);
        }

        [TestMethod]
        public void CatchAndIgnoreException()
        {
            string input = "";

            object output =
                input
                    .StartFlow()
                    .Chain((i) => { throw new ArgumentException("41"); return 41; })
                    .Catch<Exception>()
                    .Return();

            Assert.AreEqual(DateTime.Now.Year, output);
        }
    }
}
