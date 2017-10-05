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
    public class MassTests
    {
        [TestMethod()]
        public void MassTest()
        {
            var mass = new Mass();

            Assert.AreEqual(Mass.COLOR_NONE, mass.Color);
            Assert.AreEqual(true, mass.IsEmpty);
        }

        [TestMethod()]
        public void MassTest1()
        {
            var mass = new Mass(Mass.COLOR_BLACK);

            Assert.AreEqual(Mass.COLOR_BLACK, mass.Color);
            Assert.AreEqual(false, mass.IsEmpty);
        }

        [TestMethod()]
        public void MassTest2()
        {
            var mass = new Mass(new Mass(Mass.COLOR_WHITE));

            Assert.AreEqual(Mass.COLOR_WHITE, mass.Color);
            Assert.AreEqual(false, mass.IsEmpty);
        }
    }
}