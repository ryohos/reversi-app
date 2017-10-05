using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ReversiApp
{
    /// <summary>
    /// App.configのreversiConfig用ハンドラーです。
    /// </summary>
    public class ReversiConfigHandler : ConfigurationSection
    {
        /// <summary>
        /// name属性
        /// </summary>
        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        /// <summary>
        /// players要素
        /// </summary>
        [ConfigurationProperty("players", IsRequired = true)]
        public ReversiConfigPlayerItemCollection Players
        {
            get
            {
                return (ReversiConfigPlayerItemCollection)this["players"];
            }
        }
    }

    /// <summary>
    /// ReversiConfigPlayerItemのコレクションクラスです。
    /// </summary>
    public class ReversiConfigPlayerItemCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// すべてのキー名のコレクションです。
        /// </summary>
        public string[] AllKeys
        {
            get
            {
                return this.BaseGetAllKeys().Select(x => x.ToString()).ToArray();
            }
        }

        /// <summary>
        /// 指定されたキーに対応する要素の情報です。
        /// </summary>
        /// <param name="name">キー名です。</param>
        public new ReversiConfigPlayerItem this[string name]
        {
            get
            {
                return (ReversiConfigPlayerItem) this.BaseGet(name);
            }
        }


        /// <summary>
        /// 新しい ConfigurationElement を作成します。
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ReversiConfigPlayerItem();
        }

        /// <summary>
        /// 指定した構成要素の要素キーを取得します。
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            ReversiConfigPlayerItem item = element as ReversiConfigPlayerItem;
            return item.Name;
        }
    }

    /// <summary>
    /// 構成ファイルのPlayerセクションの項目です。
    /// </summary>
    public class ReversiConfigPlayerItem : ConfigurationElement
    {
        /// <summary>
        /// プレイヤー名です。
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        /// <summary>
        /// クラス名です。
        /// </summary>
        [ConfigurationProperty("className", IsRequired = true)]
        public string ClassName
        {
            get
            {
                return (string)this["className"];
            }
            set
            {
                this["className"] = value;
            }
        }

        /// <summary>
        /// オブジェクトを文字列化します。
        /// </summary>
        /// <returns>文字列です。</returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", this.Name, this.ClassName);
        }
    }
}
