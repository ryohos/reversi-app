using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Configuration;
using System.Threading;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using ReversiPlayFramework;

namespace ReversiApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int DEFAULT_GOLDEN_X = 0;
        public const int DEFAULT_GOLDEN_Y = 0;
        public const int DEFAULT_GOLDEN_POINT = 5;
        public const int DEFAULT_GOLDEN_POINT_MAX = 100;
        public const int DEFAULT_AUTO_PLAY_TIME = 10;
        public const int DEFAULT_BUTTLE_COUNT = 1;

        /// <summary>
        /// ゲームエンジンです。
        /// </summary>
        public GameEngine Engine { get; private set; }

        /// <summary>
        /// 現在のセル位置です。
        /// </summary>
        public Tuple<int, int> CurrentCell { get; private set; }

        /// <summary>
        /// プレイヤー候補一覧です。
        /// </summary>
        public Dictionary<string, string> PlayerDictionary;

        /// <summary>
        /// 結果一覧です。
        /// </summary>
        public ObservableCollection<ResultHistory> ResultHistories { get; private set; }

        /// <summary>
        /// 出力用結果一覧です。
        /// </summary>
        public List<GameResult> OutputResults { get; private set; }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public MainWindow()
        {
            this.Engine = new GameEngine();
            this.CurrentCell = new Tuple<int, int>(-1, -1);
            this.ResultHistories = new ObservableCollection<ResultHistory>();
            this.OutputResults = new List<GameResult>();

            this.InitializeComponent();
            this.InitializeResultDataGrid();
            this.InitializeBoardGrid();
            this.InitializePlayerComboBox();
            this.InitializeResultDataGrid();
        }

        #region "Utility"
        /// <summary>
        /// 塗りつぶしブラシを取得します。
        /// </summary>
        /// <param name="color">色</param>
        /// <returns>塗りつぶしブラシ</returns>
        private static SolidColorBrush GetFillBrush(int color)
        {
            switch(color)
            {
                case Mass.COLOR_BLACK:
                    return Brushes.Black;
                case Mass.COLOR_WHITE:
                    return Brushes.White;
            }
            return Brushes.Transparent;
        }

        /// <summary>
        /// 枠線ブラシを取得します。
        /// </summary>
        /// <param name="color">色</param>
        /// <returns>枠線ブラシ</returns>
        private static SolidColorBrush GetStrokeBrush(int color)
        {
            switch (color)
            {
                case Mass.COLOR_BLACK:
                    return Brushes.Black;
                case Mass.COLOR_WHITE:
                    return Brushes.Black;
            }
            return Brushes.Transparent;
        }
        #endregion

        #region "Initialize"
        /// <summary>
        /// 結果用データグリッドを初期化します。
        /// </summary>
        public void InitializeResultDataGrid()
        {
            this.resultDataGrid.ItemsSource = this.ResultHistories;
        }

        /// <summary>
        /// ボード用グリッドを初期化します。
        /// </summary>
        public void InitializeBoardGrid()
        {
            int columnCount = this.boardGrid.ColumnDefinitions.Count;
            int rowCount = this.boardGrid.RowDefinitions.Count;

            // 外枠の描画
            Rectangle outerBorder = new Rectangle();
            outerBorder.Stroke = Brushes.LightGray;
            outerBorder.StrokeThickness = 1;
            Grid.SetRowSpan(outerBorder, rowCount);
            Grid.SetColumnSpan(outerBorder, columnCount);
            this.boardGrid.Children.Add(outerBorder);

            // 各セルの枠を描画
            for (int i = 0; i < rowCount; ++i)
            {
                for (int j = 0; j < columnCount; ++j)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = Brushes.Transparent;
                    rect.Stroke = Brushes.LightGray;
                    rect.StrokeThickness = 1;
                    rect.MouseDown += new MouseButtonEventHandler(this.cell_MouseDown);
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    this.boardGrid.Children.Add(rect);
                }
            }

            // 各セル内に石を描画
            for (int i = 0; i < rowCount; ++i)
            {
                for (int j = 0; j < columnCount; ++j)
                {
                    // 円弧を書く
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = Brushes.Transparent;
                    ellipse.Stroke = Brushes.Transparent;
                    ellipse.StrokeThickness = 1;
                    ellipse.MouseDown += new MouseButtonEventHandler(this.cell_MouseDown);
                    Grid.SetRow(ellipse, i);
                    Grid.SetColumn(ellipse, j);
                    this.boardGrid.Children.Add(ellipse);
                }
            }
        }

        /// <summary>
        /// プレイヤーのコンボボックスを初期化します。
        /// </summary>
        public void InitializePlayerComboBox()
        {
            this.PlayerDictionary = new Dictionary<string, string>();

            var reversiConfig = (ReversiConfigHandler) ConfigurationManager.GetSection("reversiConfig");
            foreach (string key in reversiConfig.Players.AllKeys)
            {
                var item = (ReversiConfigPlayerItem) reversiConfig.Players[key];

                this.PlayerDictionary.Add(item.Name, item.ClassName);
            }

            var selectList = this.PlayerDictionary.Select(x => x.Key).ToList();
            selectList.Insert(0, "");

            this.player1ComboBox.ItemsSource = selectList.ToArray();
            this.player2ComboBox.ItemsSource = selectList.ToArray();
        }

        /// <summary>
        /// ゲームエンジンを初期化します。
        /// </summary>
        private void InitializeGameEngine()
        {
            GameSetting setting = new GameSetting();

            // アセンブリファイル名指定
            setting.PlayersAssembly = "ReversiPlayers.dll";

            // ボードサイズは８×８固定
            setting.BoardSize = 8;

            // プレイヤーの設定
            Random random = new Random();
            setting.Player1 = this.CreatePlayerSetting(random, this.player1ComboBox.Text);
            setting.Player2 = this.CreatePlayerSetting(random, this.player2ComboBox.Text);
            this.player1ComboBox.SelectedItem = setting.Player1.PlayerName;
            this.player2ComboBox.SelectedItem = setting.Player2.PlayerName;

            // ルールの設定
            setting.Rule = this.CreateRuleSetting();

            this.Engine.Init(setting);
            this.Engine.SetPlayer1EventHandler(player_Notification);
            this.Engine.SetPlayer2EventHandler(player_Notification);
        }

        /// <summary>
        /// プレイヤー設定を生成します。
        /// </summary>
        /// <param name="random">乱数です。</param>
        /// <param name="playerName">プレイヤー名です。</param>
        /// <returns>プレイヤー設定を返します。</returns>
        private GameSetting.PlayerSetting CreatePlayerSetting(Random random, string playerName)
        {
            var playerSetting = new GameSetting.PlayerSetting();

            // 空の場合はランダムに割り当てます
            if(string.IsNullOrEmpty(playerName))
            {
                int count = random.Next(this.PlayerDictionary.Count);

                playerName = this.PlayerDictionary.Skip(count).FirstOrDefault().Key;
            }

            playerSetting.PlayerName = playerName;
            playerSetting.PlayerClassName = this.PlayerDictionary[playerName];

            return playerSetting;
        }

        /// <summary>
        /// ルール設定を生成します。
        /// </summary>
        /// <returns></returns>
        private GameSetting.RuleSetting CreateRuleSetting()
        {
            var rule = new GameSetting.RuleSetting();

            var isChecked = this.goldenMassCheckBox.IsChecked.HasValue ? this.goldenMassCheckBox.IsChecked.Value : false;

            if(isChecked)
            {
                rule.BaseRule = GameSetting.RuleSetting.RULE_GOLDEN_FLAG;
                rule.GoldenFlagX = int.Parse(this.goldenMassXTextBox.Text);
                rule.GoldenFlagY = int.Parse(this.goldenMassYTextBox.Text);
                rule.GoldenFlagPoint = int.Parse(this.goldenMassPointTextBox.Text);
            }
            else
            {
                rule.BaseRule = GameSetting.RuleSetting.RULE_NORMAL;
            }

            return rule;
        }
        #endregion

        #region "EventHandler"
        /// <summary>
        /// 開始ボタンを押下した場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.Engine.IsRunning)
            {
                return;
            }

            this.Start();

            if(!this.nextButton.IsEnabled)
            {
                int sleep = int.Parse(this.autoPlayTimeTextBox.Text);
                while (this.Engine.IsRunning)
                {
                    Thread.Sleep(sleep);
                    this.Next();
                }
            }
        }

        /// <summary>
        /// 次ボタンを押下した場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.Engine.IsRunning)
            {
                return;
            }

            this.Next();
        }

        /// <summary>
        /// 出力ボタンを押下した場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void outputButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "ファイルを保存";
            dialog.Filter = "テキストファイル|*.xml";

            if (dialog.ShowDialog() != true) return;

            this.Engine.SaveResults(dialog.FileName);
        }

        /// <summary>
        /// 結果のクリアボタンを押下した場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearResultButton_Click(object sender, RoutedEventArgs e)
        {
            this.ResultHistories.Clear();
            this.Engine.ClearResult();
            this.ClearBoard();
        }

        /// <summary>
        /// Player1コンボボックスをクリックした場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void player1ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.EnableAutoPlayControl();
        }

        /// <summary>
        /// Player2コンボボックスをクリックした場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void player2ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.EnableAutoPlayControl();
        }

        /// <summary>
        /// ゴールデンマスチェックボックスをクリックした場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goldenMassCheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.EnableGoldenMassControl();
            var isChecked = this.goldenMassCheckBox.IsChecked.HasValue ? this.goldenMassCheckBox.IsChecked.Value : false;

            int x = DEFAULT_GOLDEN_X;
            int y = DEFAULT_GOLDEN_Y;
            int point = DEFAULT_GOLDEN_POINT;

            // ランダムに設定する
            int columnCount = this.boardGrid.ColumnDefinitions.Count;
            int rowCount = this.boardGrid.RowDefinitions.Count;

            var rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            x = rand.Next() % columnCount;
            y = rand.Next() % rowCount;
            point = rand.Next() % DEFAULT_GOLDEN_POINT_MAX;

            this.goldenMassXTextBox.Text = isChecked ? x.ToString() : "";
            this.goldenMassYTextBox.Text = isChecked ? y.ToString() : "";
            this.goldenMassPointTextBox.Text = isChecked ? point.ToString() : "";
        }

        /// <summary>
        /// 自動再生チェックボックスをクリックした場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoPlayCheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.EnableAutoPlayControl();

            var isChecked = this.autoPlayCheckBox.IsChecked.HasValue ? this.autoPlayCheckBox.IsChecked.Value : false;

            this.autoPlayTimeTextBox.Text = isChecked ? DEFAULT_AUTO_PLAY_TIME.ToString() : "";
        }

        /// <summary>
        /// ゴールデンマスX値のテキストが変化した場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goldenMassXTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int rowCount = this.boardGrid.RowDefinitions.Count;
            this.SetIntegerText(this.goldenMassXTextBox, 0, rowCount - 1, DEFAULT_GOLDEN_X);
        }

        /// <summary>
        /// ゴールデンマスY値のテキストが変化した場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goldenMassYTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int columnCount = this.boardGrid.ColumnDefinitions.Count;
            this.SetIntegerText(this.goldenMassYTextBox, 0, columnCount - 1, DEFAULT_GOLDEN_Y);
        }

        /// <summary>
        /// ゴールデンマスポイントのテキストが変化した場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goldenMassPointTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetIntegerText(this.goldenMassPointTextBox, 0, int.MaxValue, DEFAULT_GOLDEN_POINT);
        }

        /// <summary>
        /// 自動再生設定のテキストが変化した場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoPlayTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetIntegerText(this.autoPlayTimeTextBox, 0, int.MaxValue, DEFAULT_AUTO_PLAY_TIME);
        }

        /// <summary>
        /// 自動再生設定のテキストが変化した場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttleCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetIntegerText(this.buttleCountTextBox, 1, int.MaxValue, DEFAULT_BUTTLE_COUNT);
        }

        /// <summary>
        /// セルでマウスダウンした場合のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var shape = sender as Shape;
            if(shape == null)
            {
                return;
            }
            Tuple<int, int> nextCell = this.GetCellPoint(shape);
            if(this.CurrentCell.Item1 == nextCell.Item1 && this.CurrentCell.Item2 == nextCell.Item2)
            {
                nextCell = new Tuple<int, int>(-1, -1);
            }
            this.CurrentCell = nextCell;

            if(this.goldenMassCheckBox.IsEnabled)
            {
                bool isChecked = this.goldenMassCheckBox.IsChecked.HasValue ? this.goldenMassCheckBox.IsChecked.Value : false;
                if(isChecked)
                {
                    this.goldenMassXTextBox.Text = this.CurrentCell.Item1.ToString();
                    this.goldenMassYTextBox.Text = this.CurrentCell.Item2.ToString();
                }
            }

            this.DrawBoard();
        }

        /// <summary>
        /// Playerオブジェクトからの通知のイベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void player_Notification(object sender, System.EventArgs e)
        {            
            ManualPlayer manualPlayer = sender as ManualPlayer;
            if(manualPlayer == null)
            {
                return;
            }

            var decision = new Decision(true);
            if(this.CurrentCell.Item1 != -1 && this.CurrentCell.Item2 != -1)
            {
                decision = new Decision(this.CurrentCell.Item1 , this.CurrentCell.Item2);
            }

            manualPlayer.Next = decision;
        }
        #endregion

        #region "Play Game"
        /// <summary>
        /// ゲームを開始します。
        /// </summary>
        private void Start()
        {
            this.InitializeGameEngine();
            this.Engine.Start();

            this.DrawBoard();
            this.DrawNotification();
            this.EnableButton();
            this.EnablePlayerSelectBox();
            this.EnableGoldenMassControl();
            this.EnableAutoPlayControl();
            this.DoEvents();     
        }

        /// <summary>
        /// 次の処理を行います。
        /// </summary>
        private void Next()
        {
            this.Engine.Next();

            this.DrawBoard();
            this.DrawNotification();
            this.EnableButton();
            this.EnablePlayerSelectBox();
            this.EnableGoldenMassControl();
            this.EnableAutoPlayControl();

            this.DoEvents();

            if (!this.Engine.IsRunning)
            {
                // Histryリストを更新する
                int count = this.Engine.ResultList.Count;
                var result = this.Engine.ResultList[count - 1];
                string winnerName = result.Player1.IsWinner ? result.Player1.Name : result.Player2.Name;

                this.ResultHistories.Add(new ResultHistory{
                    No = this.ResultHistories.Count + 1,
                    Winner = winnerName,
                    Status = result.Status,
                    Player1Score = result.Player1.Score,
                    Player2Score = result.Player2.Score
                });
                this.OutputResults.Add(result);

                if(this.ResultHistories.Count() < int.Parse(buttleCountTextBox.Text))
                {
                    this.goldenMassCheckBox_Click(this, null);
                    this.Start();
                    return;
                }

                var player1WinCount = this.ResultHistories.Count(x => x.Winner == result.Player1.Name);
                var player2WinCount = this.ResultHistories.Count(x => x.Winner == result.Player2.Name);
                if (result.Player1.IsWinner == true)
                {
                    MessageBox.Show(string.Format("{0}の勝ちです。{1}勝{2}敗", result.Player1.Name, player1WinCount, player2WinCount));
                }
                else if (result.Player2.IsWinner == true)
                {
                    MessageBox.Show(string.Format("{0}の勝ちです。{1}勝{2}敗", result.Player2.Name, player2WinCount, player1WinCount));
                }
                else
                {
                    MessageBox.Show(string.Format("引き分けです。{0}勝{1}敗", player1WinCount, player2WinCount));
                }
            }
        }
        #endregion

        #region "Draw Notification"
        /// <summary>
        /// 通知を描画する。
        /// </summary>
        private void DrawNotification()
        {
            this.notificationLabel.Visibility = this.Engine.IsRunning ? Visibility.Visible : Visibility.Hidden;

            switch(this.Engine.CurrentPlayerColor)
            {
                case Mass.COLOR_BLACK:
                    this.notificationLabel.Content = "黒番です。";
                    break;
                case Mass.COLOR_WHITE:
                    this.notificationLabel.Content = "白番です。";
                    break;
            }

        }
        #endregion

        #region "Draw Board"
        /// <summary>
        /// ボードをクリアします。
        /// </summary>
        private void ClearBoard()
        {
            int rowCount = this.boardGrid.RowDefinitions.Count;
            int columnCount = this.boardGrid.ColumnDefinitions.Count;
            for (int i = 0; i < rowCount; ++i)
            {
                for (int j = 0; j < columnCount; ++j)
                {
                    this.DrawCell(i, j, new Mass());
                }
            }
        }


        /// <summary>
        /// ボードを描画します。
        /// </summary>
        private void DrawBoard()
        {
            int rowCount = this.boardGrid.RowDefinitions.Count;
            int columnCount = this.boardGrid.ColumnDefinitions.Count;
            for (int i = 0; i < rowCount; ++i)
            {
                for (int j = 0; j < columnCount; ++j)
                {
                    var mass = this.Engine.GetMass(i, j);
                    this.DrawCell(i, j, mass);
                }
            }
        }

        /// <summary>
        /// 円を描画します。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mass"></param>
        private void DrawCell(int x, int y, Mass mass)
        {
            int columnCount = this.boardGrid.ColumnDefinitions.Count;
            if (x < 0 || x >= columnCount)
            {
                return;
            }

            int rowCount = this.boardGrid.RowDefinitions.Count;
            if (y < 0 || y >= rowCount)
            {
                return;
            }

            {   // 円の描画
                int offset = GetEllipseOffset();
                int index = offset + y * columnCount + x;
                int color = mass != null ? mass.Color : Mass.COLOR_NONE;
                Ellipse ellipse = (Ellipse)this.boardGrid.Children[index];
                ellipse.Fill = GetFillBrush(color);
                ellipse.Stroke = GetStrokeBrush(color);
            }

            {   // 四角形の描画
                int offset = this.GetRectOffset();
                int index = offset + y * columnCount + x;
                bool special = mass != null ? mass.Point > 1 : false;
                Rectangle rect = (Rectangle)this.boardGrid.Children[index];
                if(x == this.CurrentCell.Item1 && y == this.CurrentCell.Item2)
                {
                    rect.Fill = special ? Brushes.LightGreen : Brushes.LightSkyBlue;
                }
                else
                {
                    rect.Fill = special ? Brushes.Yellow : Brushes.Transparent;
                }
            }
        }
        #endregion

        #region "Enable Control"
        /// <summary>
        /// ボタンの有効化/無効化を切り替えます。
        /// </summary>
        private void EnableButton()
        {
            var isRunning = this.Engine != null ? this.Engine.IsRunning : false;
            var isAutoPlay = this.autoPlayCheckBox.IsChecked.HasValue ? this.autoPlayCheckBox.IsChecked.Value : false;
            this.startButton.IsEnabled = !isRunning;
            this.nextButton.IsEnabled = isRunning & !isAutoPlay;
        }

        /// <summary>
        /// プレイヤーコンボボックスの有効化/無効化を切り替えます。
        /// </summary>
        private void EnablePlayerSelectBox()
        {
            var isRunning = this.Engine != null ? this.Engine.IsRunning : false;
            this.player1ComboBox.IsEnabled = !isRunning;
            this.player2ComboBox.IsEnabled = !isRunning;
        }

        /// <summary>
        /// ゴールデンマステキストボックスの有効化/無効化を切り替えます。
        /// </summary>
        private void EnableGoldenMassControl()
        {

            this.goldenMassCheckBox.IsEnabled = !this.Engine.IsRunning;
            var isChecked = this.goldenMassCheckBox.IsChecked.HasValue ? this.goldenMassCheckBox.IsChecked.Value : false;
            var isEnabled = isChecked & this.goldenMassCheckBox.IsEnabled;

            this.goldenMassLabel.IsEnabled = isEnabled;
            this.goldenMassLeftLabel.IsEnabled = isEnabled;
            this.goldenMassXTextBox.IsEnabled = isEnabled;
            this.goldenMassCommaLabel.IsEnabled = isEnabled;
            this.goldenMassYTextBox.IsEnabled = isEnabled;
            this.goldenMassRightLabel.IsEnabled = isEnabled;
            this.goldenMassPointLabel.IsEnabled = isEnabled;
            this.goldenMassPointTextBox.IsEnabled = isEnabled;
        }

        /// <summary>
        /// 自動再生テキストボックスの有効化/無効化を切り替えます。
        /// </summary>
        private void EnableAutoPlayControl()
        {
            this.autoPlayCheckBox.IsEnabled = !this.Engine.IsRunning;
            string manualPlayer = "ManualPlayer";
            if ((this.player1ComboBox.SelectedItem != null && this.player1ComboBox.SelectedItem.ToString() == manualPlayer) || 
                (this.player2ComboBox.SelectedItem != null && this.player2ComboBox.SelectedItem.ToString() == manualPlayer))
            {
                this.autoPlayCheckBox.IsEnabled = false;
                this.autoPlayCheckBox.IsChecked = false;
                this.autoPlayTimeTextBox.Text = "";
            }

            var isChecked = this.autoPlayCheckBox.IsChecked.HasValue ? this.autoPlayCheckBox.IsChecked.Value : false;
            var isEnabled = isChecked & this.autoPlayCheckBox.IsEnabled;

            this.autoPlayTimeLabel.IsEnabled = isEnabled;
            this.autoPlayTimeTextBox.IsEnabled = isEnabled;
            this.autoPlayTimeSignLabel.IsEnabled = isEnabled;
        }
        #endregion

        #region "Set Text"
        /// <summary>
        /// 整数テキストを設定する
        /// </summary>
        /// <param name="textBox">テキストボックス</param>
        /// <param name="minValue">最小値</param>
        /// <param name="maxValue">最大値</param>
        /// <param name="defaultValue">デフォルト値</param>
        private void SetIntegerText(TextBox textBox, int minValue, int maxValue, int defaultValue)
        {
            if(string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }

            int result = 0;
            if (!int.TryParse(textBox.Text, out result))
            {
                textBox.Text = defaultValue.ToString();
            }
            if (result < minValue || result > maxValue)
            {
                textBox.Text = defaultValue.ToString();
            }
        }
        #endregion

        #region "Refresh"
        /// <summary>
        /// 現在メッセージ待ち行列の中にある全てのUIメッセージを処理します。
        /// </summary>
        private void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(obj =>
            {
                ((DispatcherFrame)obj).Continue = false;
                return null;
            });
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
        }
        #endregion

        #region "Information"
        /// <summary>
        /// Rectangleオフセット値を返します。
        /// </summary>
        /// <returns>Rectangleオフセット値です。</returns>
        /// <remarks>
        /// 本メソッドはInitializeBoardGridのアルゴリズムに影響を受けます。
        /// </remarks>
        private int GetRectOffset()
            {
                // 枠線部分のIndex(OuterBoarder分)
                return 1;
            }

        /// <summary>
        /// Ellipseオフセット値を返します。
        /// </summary>
        /// <returns>Ellipseオフセット値です。</returns>
        /// <remarks>
        /// 本メソッドはInitializeBoardGridのアルゴリズムに影響を受けます。
        /// </remarks>
        private int GetEllipseOffset()
        {
            int columnCount = this.boardGrid.ColumnDefinitions.Count;
            int rowCount = this.boardGrid.RowDefinitions.Count;

            // 枠線部分のIndex(OuterBoarder＋Rectangle分)
            return 1 + columnCount * rowCount;
        }

        /// <summary>
        /// ボードオブジェクト上の指定したオブジェクトのセル位置を返します。
        /// </summary>
        /// <param name="shape">図形</param>
        /// <returns>セル位置</returns>
        private Tuple<int, int> GetCellPoint(Shape shape)
        {
            if(shape.Parent != this.boardGrid)
            {
                return new Tuple<int, int>(-1, -1);
            }

            int offset = shape is Ellipse ? this.GetEllipseOffset() : this.GetRectOffset();
            int index = offset;
            for (index = offset; index < this.boardGrid.Children.Count; ++index)
            {
                if (this.boardGrid.Children[index] == shape)
                {
                    break;
                }
            }
            index -= offset;

            int columnCount = this.boardGrid.ColumnDefinitions.Count;

            return new Tuple<int, int>(index % columnCount, index / columnCount);
        }
        #endregion
    }
}
