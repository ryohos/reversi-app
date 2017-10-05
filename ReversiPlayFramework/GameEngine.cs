using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;

namespace ReversiPlayFramework
{
    /// <summary>
    /// ゲームエンジンクラスです。
    /// </summary>
    /// <remarks>
    /// ゲームは下記の流れで実行することができます。
    ///  var game = new GameEngine();
    ///  game.Init(gameSetting);
    ///  game.Start();
    ///  while(game.IsRunning)
    ///  {
    ///     game.Next();
    ///  }
    /// </remarks>
    public class GameEngine
    {
        /// <summary>
        /// 過去の結果リストです。
        /// </summary>
        public List<GameResult> ResultList { get; private set; }

        /// <summary>
        /// 実行しているかどうかをあらわします。
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _Result != null;
            }
        }
    
        /// <summary>
        /// 現在プレイヤーのカラーを返す。
        /// </summary>
        public int CurrentPlayerColor
        {
            get
            {
                return _CurrentPlayer != null ? _CurrentPlayer.Color : Mass.COLOR_NONE;
            }
        }

        #region "Constructor"
        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public GameEngine()
        {
            _Setting = null;
            _Result = null;

            _Board = null;
            _Player1 = null;
            _Player2 = null;
            _CurrentPlayer = null;
            _NextPlayer = null;

            this.ResultList = new List<GameResult>();
        }
        #endregion

        #region "Play Game"
        /// <summary>
        /// ゲームを初期化します。設定できなかった場合はfalseを返します。
        /// </summary>
        /// <param name="setting">ゲーム設定です。</param>
        public void Init(GameSetting setting)
        {
            _Setting = setting;
            _Board = this.CreateBoard(_Setting.BoardSize, _Setting.Rule);
            _Player1 = this.CreatePlayer(_Setting.PlayersAssembly, _Setting.Player1.PlayerClassName, _Setting.Player1.PlayerName, Mass.COLOR_BLACK);
            _Player2 = this.CreatePlayer(_Setting.PlayersAssembly, _Setting.Player2.PlayerClassName, _Setting.Player2.PlayerName, Mass.COLOR_WHITE);
            _Result = null;
            _CurrentPlayer = null;
            _NextPlayer = null;
        }

        /// <summary>
        /// ゲームを開始します。
        /// </summary>
        public void Start()
        {
            _Board = this.CreateBoard(_Setting.BoardSize, _Setting.Rule);
            _CurrentPlayer = _Player1;
            _NextPlayer = _Player2;
            _Result = new GameResult(_Player1, _Player2);
        }

        /// <summary>
        /// ゲームを進行します。
        /// </summary>
        public void Next()
        {
            // ゲームが開始されていない場合は何もしない。
            if (!this.IsRunning)
            {
                return;
            }

            Decision beforeDecision = _Result.DecisionList.Count > 0 ? _Result.DecisionList[_Result.DecisionList.Count - 1] : null;
            Decision currentDecision = null;

            Exception exception = null;
            bool result = false;

            // 進行を進める
            try
            {
                currentDecision = _CurrentPlayer.Play();

                _Result.DecisionList.Add(currentDecision);

                if (currentDecision.IsPass)
                {
                    result = _Board.SetPass(_CurrentPlayer.Color);
                }
                else
                {
                    result = _Board.SetMassColor(currentDecision.X, currentDecision.Y, _CurrentPlayer.Color);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            var player1Point = _Board.CountMassPoint(_Player1.Color);
            var player2Point = _Board.CountMassPoint(_Player2.Color);

            if (exception != null)
            {
                // 例外を発生した場合
                _Result.Status = GameResult.STATUS_EXCEPTION;

                // スコアを更新する
                if (_CurrentPlayer == _Player1)
                {
                    _Result.Player1.Score = 0;
                    _Result.Player2.Score = player2Point;
                    _Result.Player1.IsWinner = false;
                    _Result.Player2.IsWinner = true;
                }
                else
                {
                    _Result.Player1.Score = player1Point;
                    _Result.Player2.Score = 0;
                    _Result.Player1.IsWinner = true;
                    _Result.Player2.IsWinner = false;
                }

                this.ResultList.Add(_Result);
                _Result = null;
            }
            else if (!result)
            {
                // 反則となる手を実行した場合
                _Result.Status = GameResult.STATUS_INVALID_DECISION;

                // スコアを更新する
                if (_CurrentPlayer == _Player1)
                {
                    _Result.Player1.Score = 0;
                    _Result.Player2.Score = player2Point;
                    _Result.Player1.IsWinner = false;
                    _Result.Player2.IsWinner = true;
                }
                else
                {
                    _Result.Player1.Score = player1Point;
                    _Result.Player2.Score = 0;
                    _Result.Player1.IsWinner = true;
                    _Result.Player2.IsWinner = false;
                }

                this.ResultList.Add(_Result);
                _Result = null;
            }
            else if (currentDecision.IsPass && beforeDecision.IsPass)
            {
                // どちらもパスが成功した場合
                _Result.Status = GameResult.STATUS_ONLY_PASS;

                // スコアを更新する
                _Result.Player1.Score = player1Point;
                _Result.Player2.Score = player2Point;
                _Result.Player1.IsWinner = player1Point > player2Point;
                _Result.Player2.IsWinner = player2Point > player1Point;

                this.ResultList.Add(_Result);
                _Result = null;
            }
            else if(_Board.IsFull())
            {
                // ボードが全て埋まった場合
                _Result.Status = GameResult.STATUS_NORMAL;

                // スコアを更新する
                _Result.Player1.Score = player1Point;
                _Result.Player2.Score = player2Point;
                _Result.Player1.IsWinner = player1Point > player2Point;
                _Result.Player2.IsWinner = player2Point > player1Point;

                this.ResultList.Add(_Result);
                _Result = null;
            }
            else
            {
                // スコアを更新する
                _Result.Player1.Score = player1Point;
                _Result.Player2.Score = player2Point;
                _Result.Player1.IsWinner = player1Point > player2Point;
                _Result.Player2.IsWinner = player2Point > player1Point;

                // プレイヤーを変更する
                var player = _NextPlayer;
                _NextPlayer = _CurrentPlayer;
                _CurrentPlayer = player;
            }
        }
        #endregion

        #region "Get GameInfo"
        /// <summary>
        /// ボードのサイズを取得します。
        /// </summary>
        /// <returns></returns>
        public int GetBoardSize()
        {
            if (_Board == null)
            {
                return 0;
            }
            return _Board.Size;
        }

        /// <summary>
        /// マスのコピーを返します。XとYの値が範囲外の場合はnullを返します。
        /// </summary>
        /// <param name="x">X値です。</param>
        /// <param name="y">Y値です。</param>
        /// <returns>マスのコピーを返します。コピーであるため返却されたマスオブジェクトの値を変更してもボードには影響を与えません。</returns>
        public Mass GetMass(int x, int y)
        {
            if(_Board == null)
            {
                return null;
            }
            return _Board.GetMass(x, y);
        }

        /// <summary>
        /// マスのポイントを数えます。
        /// </summary>
        /// <param name="color">対象となるマスの色です。</param>
        /// <returns>マスのポイントを返します。</returns>
        public int CountMassPoint(int color)
        {
            if (_Board == null)
            {
                return 0;
            }
            return _Board.CountMassPoint(color);
        }

        /// <summary>
        /// 結果を初期化します。
        /// </summary>
        public void ClearResult()
        {
            this.ResultList.Clear();
        }

        /// <summary>
        /// 結果を保存します。
        /// </summary>
        /// <param name="fileName">保存ファイル名です。</param>
        public void SaveResults(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<GameResult>));
            using (var sw = new StreamWriter(fileName, false, new UTF8Encoding()))
            {
                serializer.Serialize(sw, this.ResultList);
            }
        }
        #endregion

        #region "Set EventHandler"
        /// <summary>
        /// Player1のイベントハンドラーを設定します。
        /// </summary>
        /// <param name="notificationEventHandler">Notificationイベントハンドラーです。</param>
        public void SetPlayer1EventHandler(EventHandler notificationEventHandler)
        {
            if (_Player1 == null)
            {
                return;
            }
            _Player1.Notification += notificationEventHandler;
        }

        /// <summary>
        /// Player2のイベントハンドラーを設定します。
        /// </summary>
        /// <param name="notificationEventHandler">Notificationイベントハンドラーです。</param>
        public void SetPlayer2EventHandler(EventHandler notificationEventHandler)
        {
            if (_Player2 == null)
            {
                return;
            }
            _Player2.Notification += notificationEventHandler;
        }
        #endregion

        #region "Private"
        /// <summary>
        /// ゲーム設定です。
        /// </summary>
        private GameSetting _Setting;

        /// <summary>
        /// ゲーム結果です。現在のゲーム結果を保持しています。
        /// </summary>
        private GameResult _Result;

        /// <summary>
        /// ボードです。
        /// </summary>
        private Board _Board;

        /// <summary>
        /// プレイヤー1です。
        /// </summary>
        private Player _Player1;

        /// <summary>
        /// プレイヤー2です。
        /// </summary>
        private Player _Player2;

        /// <summary>
        /// 現在プレイヤーです。
        /// </summary>
        private Player _CurrentPlayer;

        /// <summary>
        /// 次プレイヤーです。
        /// </summary>
        private Player _NextPlayer;

        #region "Factory Method"
        /// <summary>
        /// プレイヤーを作成します。
        /// </summary>
        /// <param name="assemblyName">アセンブリ名です。</param>
        /// <param name="playerClassName">インスタンス化するプレイヤークラス名です。</param>
        /// <param name="name">プレイヤー名です。</param>
        /// <param name="color">プレイヤーの色です。</param>
        /// <returns>プレイヤーオブジェクトを返します。</returns>
        private Player CreatePlayer(string assemblyName, string playerClassName, string name, int color)
        {
            Type type = Type.GetType(playerClassName);

            if (type == null)
            {
                Assembly asm = Assembly.LoadFrom(assemblyName);
                type = asm.GetType(playerClassName);
            }

            return (Player)Activator.CreateInstance(type, new object[] { this, name, color });
        }

        /// <summary>
        /// ボードを生成します。
        /// </summary>
        /// <param name="boardSize">ボードサイズです。</param>
        /// <param name="rule">ルールです。</param>
        /// <returns>ボードオブジェクトを返します。</returns>
        private Board CreateBoard(int boardSize, GameSetting.RuleSetting rule)
        {
            if (rule.BaseRule == GameSetting.RuleSetting.RULE_GOLDEN_FLAG)
            {
                return new Board(boardSize, rule.GoldenFlagX, rule.GoldenFlagY, rule.GoldenFlagPoint);
            }
            return new Board(boardSize);
        }
        #endregion
        #endregion
    }
}
