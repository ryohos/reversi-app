using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReversiPlayFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiPlayFramework.Tests
{
    [TestClass()]
    public class DecisionTests
    {
        [TestMethod()]
        public void DecisionTest()
        {
            var decision = new Decision(true);
            Assert.AreEqual(true, decision.IsPass);
        }

        [TestMethod()]
        public void DecisionTest2()
        {
            var decision = new Decision(1, 2);
            Assert.AreEqual(false, decision.IsPass);
            Assert.AreEqual(1, decision.X);
            Assert.AreEqual(2, decision.Y);
        }
    }
}