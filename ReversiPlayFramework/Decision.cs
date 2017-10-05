using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiPlayFramework
{
    /// <summary>
    /// 決定クラスです。
    /// </summary>
    public class Decision
    {
        /// <summary>
        /// 対象のX値です。
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 対象のY値です。
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// パスの場合は、trueを設定してください。
        /// 本プロパティがtrueに設定されている場合は、X, Yの値は無効になります。
        /// </summary>
        public bool IsPass { get; set; }

        /// <summary>
        /// デフォルトコンストラクタです。
        /// </summary>
        public Decision()
        {
            this.X = 0;
            this.Y = 0;
            this.IsPass = false;
        }

        /// <summary>
        /// コンストラクタ（パス用）です。
        /// </summary>
        public Decision(bool isPass)
        {
            this.X = 0;
            this.Y = 0;
            this.IsPass = isPass;
        }

        /// <summary>
        /// コンストラクタ（設定用）です。
        /// </summary>
        public Decision(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.IsPass = false;
        }
    }
}
