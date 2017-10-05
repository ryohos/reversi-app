using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiPlayFramework
{
    /// <summary>
    /// ボードクラスです。リバーシのボードを表現します。
    /// </summary>
    public class Board
    {
        /// <summary>
        /// 最大サイズです。
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="size">ボードのサイズです。</param>
        /// <param name="x">特別マスのX値です。</param>
        /// <param name="y">特別マスY値です。</param>
        /// <param name="point">特別マスのポイントです。</param>
        public Board(int size, int x = -1, int y = -1, int point = -1)
        {
            this.Size = size;
            _Matrix = this.CreateMatrix(this.Size);

            // ポイントマスを設定する
            if(x >= 0 && x < this.Size && y >= 0 && y < this.Size)
            {
                _Matrix[x][y].Point = point;
            }

            // 初期配置にする
            this.InitMatrix();
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="board">コピー元のボードです。</param>
        public Board(Board board)
        {
            this.Size = board.Size;
            _Matrix = this.CreateMatrix(this.Size, board);
        }

        /// <summary>
        /// マスの色を設定します。
        /// </summary>
        /// <param name="x">設定するX値です。</param>
        /// <param name="y">設定するY値です。</param>
        /// <param name="color">設定する色です。プレイヤー色を設定してください。</param>
        /// <returns>設定に成功した場合は、trueを返します。設定に失敗した場合は、falseを返します。</returns>
        public bool SetMassColor(int x, int y, int color)
        {
            // 範囲外の場合は設定できない
            if (x < 0 || x >= this.Size)
            {
                return false;
            }

            // 範囲外の場合は設定できない
            if (y < 0 || y >= this.Size)
            {
                return false;
            }

            // 既に石が配置されている場合は置けない
            if (!_Matrix[x][y].IsEmpty)
            {
                return false;
            }

            int changedMassColor = 0;

            // 左上方向(-1,-1)
            changedMassColor += this.ChangeMassColor(x, y, color, -1, -1);

            // 中上方向(+0,-1)
            changedMassColor += this.ChangeMassColor(x, y, color, 0, -1);

            // 右上方向(+1,-1)
            changedMassColor += this.ChangeMassColor(x, y, color, 1, -1);

            // 左横方向(-1,+0)
            changedMassColor += this.ChangeMassColor(x, y, color, -1, 0);

            // 右横方向(+1,+0)
            changedMassColor += this.ChangeMassColor(x, y, color, 1, 0);

            // 左下方向(-1,+1)
            changedMassColor += this.ChangeMassColor(x, y, color, -1, 1);

            // 中下方向(+0,+1)
            changedMassColor += this.ChangeMassColor(x, y, color, 0, 1);

            // 右下方向(+1,+1)
            changedMassColor += this.ChangeMassColor(x, y, color, 1, 1);

            // 挟めない場合はおけない
            if (changedMassColor == 0)
            {
                return false;
            }

            // 石を配置する
            _Matrix[x][y].Color = color;

            return true;
        }

        /// <summary>
        /// パスを設定します。
        /// </summary>
        /// <param name="color">設定する色です。プレイヤー色を設定してください。</param>
        /// <returns>設定に成功した場合は、trueを返します。設定に失敗した場合は、falseを返します。</returns>
        /// <remarks>ボードの状態は変化しません。</remarks>
        public bool SetPass(int color)
        {
            var board = new Board(this);

            for (int i = 0; i < board.Size; ++i)
            {
                for (int j = 0; j < board.Size; ++j)
                {
                    if (board.SetMassColor(color, i, j))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// マスのコピーを返します。XとYの値が範囲外の場合はnullを返します。
        /// </summary>
        /// <param name="x">X値です。</param>
        /// <param name="y">Y値です。</param>
        /// <returns>マスのコピーを返します。コピーであるため返却されたマスオブジェクトの値を変更してもボードには影響を与えません。</returns>
        public Mass GetMass(int x, int y)
        {
            if (x < 0 || x >= this.Size)
            {
                return null;
            }
            if (y < 0 || y >= this.Size)
            {
                return null;
            }
            return new Mass(_Matrix[x][y]);
        }

        /// <summary>
        /// マスのポイントを数えます。
        /// </summary>
        /// <param name="color">対象となるマスの色です。</param>
        /// <returns>マスのポイントを返します。</returns>
        public int CountMassPoint(int color)
        {
            int point = 0;
            for(int i = 0; i < _Matrix.Count; ++i)
            {
                for(int j = 0; j < _Matrix[i].Count; ++j)
                {
                    if(_Matrix[i][j].Color == color)
                    {
                        point += _Matrix[i][j].Point;
                    }
                }
            }
            return point;
        }

        /// <summary>
        /// マスが全て埋まっているかどうかを判定します。
        /// </summary>
        /// <returns>マスが全て埋まっている場合はtrueを返します。それ以外の場合はfalseを返します。</returns>
        public bool IsFull()
        {
            for(int i = 0; i < _Matrix.Count; ++i)
            {
                for(int j = 0; j < _Matrix[i].Count; ++j)
                {
                    if(_Matrix[i][j].IsEmpty)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #region "Private"
        /// <summary>
        /// マスのマトリクスです。
        /// </summary>
        private List<List<Mass>> _Matrix;

        /// <summary>
        /// マス色を変更します。
        /// </summary>
        /// <param name="x">設定するX値です。</param>
        /// <param name="y">設定するY値です。</param>
        /// <param name="color">設定する色です。</param>
        /// <param name="divX">走査変化量のX値です。</param>
        /// <param name="divY">走査変化量のY値です。</param>
        /// <returns>色を変更したマスの数を返します。</returns>
        private int ChangeMassColor(int x, int y, int color, int divX, int divY)
        {
            var targetMassList = new List<Mass>();

            int i = x;
            int j = y;

            // 色変更の対象となるマスを選択する処理
            while(true)
            {
                i += divX;
                j += divY;

                if (i >= this.Size || i < 0)
                {
                    targetMassList.Clear();
                    break;
                }

                if (j >= this.Size || j < 0)
                {
                    targetMassList.Clear();
                    break;
                }

                if(_Matrix[i][j].IsEmpty)
                {
                    targetMassList.Clear();
                    break;
                }

                if(_Matrix[i][j].Color == color)
                {
                    break;
                }

                targetMassList.Add(_Matrix[i][j]);
            }

            // マス色を変える処理
            foreach(var mass in targetMassList)
            {
                mass.Color = color;
            }

            return targetMassList.Count;
        }

        /// <summary>
        /// マトリクスを初期化します。
        /// </summary>
        /// <returns>初期化が成功した場合、trueを返します</returns>
        private void InitMatrix()
        {
            int center = this.Size / 2;

            _Matrix[center - 1][center - 1].Color = Mass.COLOR_WHITE;
            _Matrix[center - 0][center - 0].Color = Mass.COLOR_WHITE;
            _Matrix[center - 1][center - 0].Color = Mass.COLOR_BLACK;
            _Matrix[center - 0][center - 1].Color = Mass.COLOR_BLACK;
        }

        /// <summary>
        /// マトリクスを生成します。
        /// </summary>
        /// <param name="size">サイズです。</param>
        /// <param name="board">ボードです。nullでない場合はこのボードのマトリクスの状態が再現されます。</param>
        /// <returns>生成されたマトリクスを返します。</returns>
        private List<List<Mass>> CreateMatrix(int size, Board board = null)
        {
            var matrix = new List<List<Mass>>();

            for (int i = 0; i < size; ++i)
            {
                var row = new List<Mass>();
                for (int j = 0; j < size; ++j)
                {
                    var mass = new Mass();
                    if(board != null)
                    {
                        mass = board.GetMass(i, j);
                    }
                    row.Add(mass);
                }
                matrix.Add(row);
            }

            return matrix;
        }
        #endregion
    }
}
