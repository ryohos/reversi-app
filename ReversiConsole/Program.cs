using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace ReversiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 3)
            {
                Console.WriteLine("引数が不足しています。");
                return;
            }
            var count = int.Parse(args[0]);
            var limit = long.Parse(args[1]);
            var fileName = args[2];

            var players = new List<Tuple<string, string>>();

            var reversiConfig = (ReversiConfigHandler)ConfigurationManager.GetSection("reversiConfig");
            foreach (string key in reversiConfig.Players.AllKeys)
            {
                var item = (ReversiConfigPlayerItem)reversiConfig.Players[key];

                players.Add(new Tuple<string, string>(item.Name, item.ClassName));
            }

            using (var sw = new StreamWriter(fileName, true, Encoding.GetEncoding("UTF-8")))
            {
                for (int i = 0; i < players.Count; ++i)
                {
                    for (int j = 0; j < players.Count; ++j)
                    {
                        var re = new ReversiExecutor(players[i], players[j]);
                        Console.WriteLine("{0} vs {1} Start", players[i].Item1, players[j].Item1);
                        var t = re.Run(count, limit);
                        Console.WriteLine("{0} vs {1} End", players[i].Item1, players[j].Item1);
                        sw.WriteLine("{0},{1},{2},{3},{4},{5}", t, players[i].Item1, players[j].Item1, re.Winners.Count, 
                            re.Winners.Count(x => x == "Player1"), re.Winners.Count(x => x == "Player2"));
                        sw.Flush();
                    }
                }
            }
        }
    }
}
