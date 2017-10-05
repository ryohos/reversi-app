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
    public class BoardTests
    {
        [TestMethod()]
        public void BoardTest()
        {
            var board = new Board(8);

            // 初期配置の確認テスト
            Assert.AreEqual(8, board.Size);

            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(3, 4).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(4, 3).Color);
            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(3, 3).Color);
            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(4, 4).Color);

            Assert.AreEqual(Mass.COLOR_NONE, board.GetMass(0, 0).Color);
            Assert.AreEqual(Mass.COLOR_NONE, board.GetMass(7, 7).Color);
            Assert.AreEqual(Mass.COLOR_NONE, board.GetMass(2, 4).Color);
            Assert.AreEqual(Mass.COLOR_NONE, board.GetMass(6, 3).Color);
        }

        [TestMethod()]
        public void SetMassColorNormalTest()
        {
            var board = new Board(8);
            bool result = true;

            // 1. 黒の初手
            result = board.SetMassColor(3, 2, Mass.COLOR_BLACK);

            Assert.AreEqual(true, result);

            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(3, 2).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(3, 3).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(3, 4).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(4, 3).Color);
            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(4, 4).Color);

            Assert.AreEqual(4, board.CountMassPoint(Mass.COLOR_BLACK));
            Assert.AreEqual(1, board.CountMassPoint(Mass.COLOR_WHITE));

            // 2. 白の初手
            result = board.SetMassColor(2, 4, Mass.COLOR_WHITE);

            Assert.AreEqual(true, result);

            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(2, 4).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(3, 2).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(3, 3).Color);
            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(3, 4).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(4, 3).Color);
            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(4, 4).Color);

            Assert.AreEqual(3, board.CountMassPoint(Mass.COLOR_BLACK));
            Assert.AreEqual(3, board.CountMassPoint(Mass.COLOR_WHITE));

            // 3. 黒と白３回後
            result = board.SetMassColor(1, 5, Mass.COLOR_BLACK);
            result = board.SetMassColor(1, 4, Mass.COLOR_WHITE);
            result = board.SetMassColor(3, 5, Mass.COLOR_BLACK);
            result = board.SetMassColor(0, 6, Mass.COLOR_WHITE);
            result = board.SetMassColor(0, 4, Mass.COLOR_BLACK);
            result = board.SetMassColor(4, 2, Mass.COLOR_WHITE);

            Assert.AreEqual(5, board.CountMassPoint(Mass.COLOR_BLACK));
            Assert.AreEqual(7, board.CountMassPoint(Mass.COLOR_WHITE));
        }

        [TestMethod()]
        public void SetMassColorExceptionalTest()
        {
            var board = new Board(8);
            bool result = true;

            // 重ねおきは出来ない
            result = board.SetMassColor(3, 3, Mass.COLOR_BLACK);

            Assert.AreEqual(false, result);

            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(3, 3).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(3, 4).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(4, 3).Color);
            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(4, 4).Color);

            // 挟めないところにはおけない
            result = board.SetMassColor(3, 5, Mass.COLOR_BLACK);

            Assert.AreEqual(false, result);

            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(3, 3).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(3, 4).Color);
            Assert.AreEqual(Mass.COLOR_NONE, board.GetMass(3, 5).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(4, 3).Color);
            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(4, 4).Color);

            // 挟めないところにはおけない
            result = board.SetMassColor(2, 0, Mass.COLOR_BLACK);

            Assert.AreEqual(false, result);

            Assert.AreEqual(Mass.COLOR_NONE, board.GetMass(2, 0).Color);
            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(3, 3).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(3, 4).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(4, 3).Color);
            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(4, 4).Color);

        }

        [TestMethod()]
        public void SetPassTest()
        {
            var board = new Board(8);
            bool result = true;

            // 挟める場合はパスできない
            result = board.SetPass(Mass.COLOR_BLACK);

            Assert.AreEqual(true, result);

            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(3, 3).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(3, 4).Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(4, 3).Color);
            Assert.AreEqual(Mass.COLOR_WHITE, board.GetMass(4, 4).Color);
        }

        [TestMethod()]
        public void GetMassTest()
        {
            var board = new Board(8);

            // オブジェクトコピーされているかどうかを確認するテスト
            var mass = board.GetMass(3, 4);
            mass.Color = Mass.COLOR_WHITE;

            Assert.AreEqual(Mass.COLOR_WHITE, mass.Color);
            Assert.AreEqual(Mass.COLOR_BLACK, board.GetMass(3, 4).Color);
        }

        [TestMethod()]
        public void CountMassPointTest()
        {
            var board = new Board(8);

            Assert.AreEqual(2, board.CountMassPoint(Mass.COLOR_BLACK));
            Assert.AreEqual(2, board.CountMassPoint(Mass.COLOR_WHITE));
        }
    }
}