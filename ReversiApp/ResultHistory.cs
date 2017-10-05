using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReversiPlayFramework;

namespace ReversiApp
{
    /// <summary>
    /// 結果履歴クラスです。
    /// </summary>
    public class ResultHistory
    {
        /// <summary>
        /// 履歴番号です。
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// 勝者です。
        /// </summary>
        public string Winner { get; set; }

        /// <summary>
        /// 決定時の状態です。
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 結果の状態です。
        /// </summary>
        public string StatusText
        {
            get
            {
                switch (this.Status)
                {
                    case GameResult.STATUS_UNKNOWN:
                        return "未決着";
                    case GameResult.STATUS_NORMAL:
                        return "通常";
                    case GameResult.STATUS_ONLY_PASS:
                        return "通常";
                    case GameResult.STATUS_INVALID_DECISION:
                        return "反則";
                    case GameResult.STATUS_EXCEPTION:
                        return "例外発生";
                }

                return "";
            }
        }

        /// <summary>
        /// プレイヤー1スコアです。
        /// </summary>
        public int Player1Score { get; set; }

        /// <summary>
        /// プレイヤー2スコアです。
        /// </summary>
        public int Player2Score { get; set; }
    }
}
