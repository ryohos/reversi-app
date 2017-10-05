using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiPlayFramework
{
    /// <summary>
    /// マニュアルのプレイヤーです。
    /// </summary>
    public class ManualPlayer : ReversiPlayFramework.Player
    {
        /// <summary>
        /// 次の決定です。
        /// </summary>
        public Decision Next { get; set; }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="game">ゲームオブジェクト</param>
        /// <param name="name">プレイヤー名</param>
        /// <param name="color">プレイヤー色</param>
        public ManualPlayer(GameEngine game, string name, int color)
            : base(game, name, color)
        {

        }

        /// <summary>
        /// 次の手を打ちます。
        /// </summary>
        /// <returns>決定オブジェクトを返します。</returns>
        /// <remarks>
        /// このアルゴリズムは手動で設定します。
        /// </remarks>
        public override Decision Play()
        {
            this.Notify();

            return this.Next != null ? this.Next : new Decision(true);
        }
    }
}
