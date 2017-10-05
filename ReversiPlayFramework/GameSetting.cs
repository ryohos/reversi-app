using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiPlayFramework
{
    /// <summary>
    /// ゲーム設定クラスです。
    /// </summary>
    public class GameSetting
    {
        /// <summary>
        /// プレイヤー設定クラスです。
        /// </summary>
        public class PlayerSetting
        {
            /// <summary>
            /// Playerクラスの名前です。この名前でPlayerクラスをインスタンス化します。
            /// </summary>
            public string PlayerClassName { get; set; }

            /// <summary>
            /// プレイヤー名です。プレイヤーを一意に識別する名前です。
            /// </summary>
            public string PlayerName { get; set; }
        }

        /// <summary>
        /// ルール設定クラスです。
        /// </summary>
        public class RuleSetting
        {
            /// <summary>
            /// 通常ルールです。
            /// </summary>
            public const int RULE_NORMAL = 0;

            /// <summary>
            /// ゴールデンフラグルールです。
            /// </summary>
            public const int RULE_GOLDEN_FLAG = 1;

            /// <summary>
            /// 基本ルールです。
            /// </summary>
            public int BaseRule { get; set; }

            /// <summary>
            /// ゴールデンフラグルールでのX座標です。
            /// </summary>
            public int GoldenFlagX { get; set; }

            /// <summary>
            /// ゴールデンフラグルールでのY座標です。
            /// </summary>
            public int GoldenFlagY { get; set; }

            /// <summary>
            /// ゴールデンフラグルールでのポイントです。
            /// </summary>
            public int GoldenFlagPoint { get; set; }
        }

        /// <summary>
        /// プレイヤーアセンブリ名
        /// </summary>
        public string PlayersAssembly { get; set; }

        /// <summary>
        /// プレイヤー1の設定です。
        /// </summary>
        public PlayerSetting Player1 { get; set; }

        /// <summary>
        /// プレイヤー2の設定です。
        /// </summary>
        public PlayerSetting Player2 { get; set; }

        /// <summary>
        /// ルールの設定です。
        /// </summary>
        public RuleSetting Rule { get; set; }

        /// <summary>
        /// ボードサイズです。
        /// </summary>
        public int BoardSize { get; set; }
    }
}
