using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiPlayFramework
{
    /// <summary>
    /// プレイヤークラスです。
    /// </summary>
    public class Player
    {
        /// <summary>
        /// 通知イベントです。
        /// </summary>
        public event EventHandler Notification;

        /// <summary>
        /// プレイヤー色です。
        /// </summary>
        public int Color { get; private set; }

        /// <summary>
        /// プレイヤー名です。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 現在のゲームオブジェクトです。
        /// </summary>
        public GameEngine CurrentGame { get; private set; }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public Player(GameEngine game, string name, int color)
        {
            this.CurrentGame = game;
            this.Name = name;
            this.Color = color;
        }

        /// <summary>
        /// 次の手を打ちます。継承先で実装してください。
        /// </summary>
        /// <returns>決定オブジェクトを返します。</returns>
        public virtual Decision Play()
        {
            return new Decision(true);
        }

        /// <summary>
        /// 通知します。必要に応じて継承先で使用してください。
        /// </summary>
        protected void Notify()
        {
            if (this.Notification != null)
            {
                this.Notification(this, EventArgs.Empty);
            }
        }
    }
}
