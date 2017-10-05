using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiPlayFramework
{
    /// <summary>
    /// ゲーム結果クラスです。
    /// </summary>
    public class GameResult
    {
        /// <summary>
        /// プレイヤー結果クラスです。
        /// </summary>
        public class PlayerResult
        {
            /// <summary>
            /// プレイヤー色です。
            /// </summary>
            public int Color { get; set; }

            /// <summary>
            /// プレイヤー名です。
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// スコアです。
            /// </summary>
            public int Score { get; set; }

            /// <summary>
            /// 勝者です。
            /// </summary>
            public bool IsWinner { get; set; }
        }

        /// <summary>
        /// 未決定の状態です。
        /// </summary>
        public const int STATUS_UNKNOWN = -1;

        /// <summary>
        /// 正常終了の状態です。
        /// </summary>
        public const int STATUS_NORMAL = 0;

        /// <summary>
        /// 双方ともパスしかできなくなった状態です。
        /// </summary>
        public const int STATUS_ONLY_PASS = 1;

        /// <summary>
        /// ルール違反の状態です。
        /// </summary>
        public const int STATUS_INVALID_DECISION = 2;

        /// <summary>
        /// 例外が発生した状態です。
        /// </summary>
        public const int STATUS_EXCEPTION = 3;

        /// <summary>
        /// 結果の状態です。
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// プレイヤー1の結果です。
        /// </summary>
        public PlayerResult Player1 { get; set; }

        /// <summary>
        /// プレイヤー2の結果です。
        /// </summary>
        public PlayerResult Player2 { get; set; }

        /// <summary>
        /// 決定リストです。
        /// </summary>
        public List<Decision> DecisionList { get; set; }

        /// <summary>
        /// デフォルトコンストラクタです。
        /// </summary>
        public GameResult()
        {
            this.Status = STATUS_UNKNOWN;
            this.Player1 = new PlayerResult();
            this.Player2 = new PlayerResult();
            this.DecisionList = new List<Decision>();
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public GameResult(Player player1, Player player2)
        {
            this.Status = STATUS_UNKNOWN;

            this.Player1 = new PlayerResult
            {
                Color = player1.Color,
                Name = player1.Name,
                Score = 0,
                IsWinner = false
            };

            this.Player2 = new PlayerResult
            {
                Color = player2.Color,
                Name = player2.Name,
                Score = 0,
                IsWinner = false
            };

            this.DecisionList = new List<Decision>();
        }
    }
}
