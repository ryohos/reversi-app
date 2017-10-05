using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiPlayFramework
{
    /// <summary>
    /// マスクラスです。
    /// </summary>
    public class Mass
    {
        /// <summary>
        /// マスの色（白色）です。
        /// </summary>
        public const int COLOR_WHITE = 1;

        /// <summary>
        /// マスの色（黒色）です。
        /// </summary>
        public const int COLOR_BLACK = -1;

        /// <summary>
        /// マスの色（なし）です。
        /// </summary>
        public const int COLOR_NONE = 0;

        /// <summary>
        /// マスの色です。
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// マスのポイントです。
        /// </summary>
        public int Point { get; set; }

        /// <summary>
        /// マスの色が設定されている場合はtrueを返します。
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.Color == COLOR_NONE;
            }
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public Mass()
        {
            this.Color = COLOR_NONE;
            this.Point = 1;
        }

        /// <summary>
        /// 色指定のコンストラクタです。
        /// </summary>
        /// <param name="color">色です。</param>
        /// <param name="point">ポイントです。</param>
        public Mass(int color, int point = 1)
        {
            this.Color = color;
            this.Point = point;
        }

        /// <summary>
        /// コピーコンストラクタです。
        /// </summary>
        /// <param name="mass"></param>
        public Mass(Mass mass)
        {
            this.Color = mass.Color;
            this.Point = mass.Point;
        }
    }
}
