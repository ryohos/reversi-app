using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReversiPlayFramework;

namespace ReversiPlayers
{
    /// <summary>
    /// サンプルのプレイヤーです。
    /// </summary>
    public class SamplePlayer : ReversiPlayFramework.Player
    {
        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="game">ゲームオブジェクト</param>
        /// <param name="name">プレイヤー名</param>
        /// <param name="color">プレイヤー色</param>
        public SamplePlayer(GameEngine game, string name, int color)
            : base(game, name, color)
        {

        }

        /// <summary>
        /// 次の手を打ちます。
        /// </summary>
        /// <returns>決定オブジェクトを返します。</returns>
        /// <remarks>
        /// このアルゴリズムはサンプルです。サンプルであるため関数などはコメントしていません。
        /// </remarks>
        public override Decision Play()
        {
            Decision decision = new Decision(true);

            var currentBoard = new List<List<Mass>>();
            int boardSize = this.CurrentGame.GetBoardSize();
            for(int i = 0; i < boardSize; ++i)
            {
                var row = new List<Mass>();
                for(int j = 0; j < boardSize; ++j)
                {
                    row.Add(this.CurrentGame.GetMass(i, j));
                }
                currentBoard.Add(row);
            }

            for (int i = 0; i < boardSize; ++i)
            {
                for (int j = 0; j < boardSize; ++j)
                {
                    if (this.CanPlace(currentBoard, i, j))
                    {
                        return new Decision(i, j);
                    }
                }
            }

            return new Decision(true);
        }

        /// <summary>
        /// CanPlace
        /// </summary>
        /// <param name="board"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool CanPlace(List<List<Mass>> board, int x, int y)
        {
            if (!board[x][y].IsEmpty)
            {
                return false;
            }

            int changedMassColor = 0;

            changedMassColor += this.ChangeMassColor(board, x, y, -1, -1);
            changedMassColor += this.ChangeMassColor(board, x, y, 0, -1);
            changedMassColor += this.ChangeMassColor(board, x, y, 1, -1);
            changedMassColor += this.ChangeMassColor(board, x, y, -1, 0);
            changedMassColor += this.ChangeMassColor(board, x, y, 1, 0);
            changedMassColor += this.ChangeMassColor(board, x, y, -1, 1);
            changedMassColor += this.ChangeMassColor(board, x, y, 0, 1);
            changedMassColor += this.ChangeMassColor(board, x, y, 1, 1);

            if (changedMassColor == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ChangeMassColor
        /// </summary>
        /// <param name="board"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="divX"></param>
        /// <param name="divY"></param>
        /// <returns></returns>
        private int ChangeMassColor(List<List<Mass>> board, int x, int y, int divX, int divY)
        {
            var targetMassList = new List<Mass>();

            int i = x;
            int j = y;

            while (true)
            {
                i += divX;
                j += divY;

                if (i >= board.Count() || i < 0)
                {
                    targetMassList.Clear();
                    break;
                }

                if (j >= board[i].Count() || j < 0)
                {
                    targetMassList.Clear();
                    break;
                }

                if (board[i][j].IsEmpty)
                {
                    targetMassList.Clear();
                    break;
                }

                if (board[i][j].Color == this.Color)
                {
                    break;
                }

                targetMassList.Add(board[i][j]);
            }

            foreach (var mass in targetMassList)
            {
                mass.Color = this.Color;
            }

            return targetMassList.Count;
        }
    }
}
