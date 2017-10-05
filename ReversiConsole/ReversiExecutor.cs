using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReversiPlayFramework;

namespace ReversiConsole
{
    public class ReversiExecutor
    {
        /// <summary>
        /// 勝者リストです。
        /// </summary>
        public List<string> Winners { get; private set; }

        /// <summary>
        /// プレイヤー１です。
        /// </summary>
        public Tuple<string, string> Player1 { get; private set; }

        /// <summary>
        /// プレイヤー２です。
        /// </summary>
        public Tuple<string, string> Player2 { get; private set; }

        /// <summary>
        /// 勝者ポイントです。Player1の勝利数からPlayer2の勝利数を引いたものです。
        /// </summary>
        public int WinnerPoint
        {
            get
            {
                return this.Winners.Count(x => x == Player1.Item1) - this.Winners.Count(x => x == Player2.Item1);
            }
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public ReversiExecutor(Tuple<string, string> player1, Tuple<string, string> player2)
        {
            _Engine = new GameEngine();
            this.Winners = new List<string>();
            this.Player1 = player1;
            this.Player2 = player2;
        }

        /// <summary>
        /// リバーシを実行します。
        /// </summary>
        public long Run(int count, long limit)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            for (int i = 0; i < count; ++i)
            {
                var setting = CreateGameSetting(Player1, Player2);
                _Engine.Init(setting);
                _Engine.Start();
                while(_Engine.IsRunning)
                {
                    _Engine.Next();
                }

                var winner = "";
                if (_Engine.ResultList[0].Player1.IsWinner) winner = "Player1";
                if (_Engine.ResultList[0].Player2.IsWinner) winner = "Player2";
                Winners.Add(winner);

                sw.Stop();
                if (sw.ElapsedMilliseconds > limit)
                {
                    return -1;
                }
                else
                {
                    sw.Start();
                }
            }

            return sw.ElapsedMilliseconds;
        }

        #region "Private"
        /// <summary>
        /// ボードサイズです。
        /// </summary>
        private const int BOARD_SIZE = 8;

        /// <summary>
        /// ゴールデンマスの最大ポイントです。
        /// </summary>
        private const int MAX_GOLDEN_FLAG_POINT = 100;

        /// <summary>
        /// ゲームエンジンです。
        /// </summary>
        private GameEngine _Engine;

        /// <summary>
        /// ゲーム設定を生成します。
        /// </summary>
        /// <returns></returns>
        private static GameSetting CreateGameSetting(Tuple<string, string> player1, Tuple<string, string> player2)
        {
            var setting = new GameSetting();

            setting.PlayersAssembly = "ReversiPlayers.dll";
            setting.BoardSize = BOARD_SIZE;
            setting.Player1 = CreatePlayerSetting(player1);
            setting.Player2 = CreatePlayerSetting(player2);
            setting.Rule = CreateRuleSetting();

            return setting;
        }

        /// <summary>
        /// プレイヤー設定を生成します。
        /// </summary>
        /// <param name="playerName">プレイヤー名です。</param>
        /// <returns>プレイヤー設定を返します。</returns>
        private static GameSetting.PlayerSetting CreatePlayerSetting(Tuple<string, string> player)
        {
            var playerSetting = new GameSetting.PlayerSetting();

            playerSetting.PlayerName = player.Item1;
            playerSetting.PlayerClassName = player.Item2;

            return playerSetting;
        }

        /// <summary>
        /// ルール設定を生成します。
        /// </summary>
        /// <returns></returns>
        private static GameSetting.RuleSetting CreateRuleSetting()
        {
            var rule = new GameSetting.RuleSetting();

            rule.BaseRule = GameSetting.RuleSetting.RULE_GOLDEN_FLAG;

            var rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            rule.GoldenFlagX = rand.Next() % BOARD_SIZE;
            rule.GoldenFlagY = rand.Next() % BOARD_SIZE;
            rule.GoldenFlagPoint = rand.Next() % MAX_GOLDEN_FLAG_POINT;

            return rule;
        }
        #endregion
    }
}
