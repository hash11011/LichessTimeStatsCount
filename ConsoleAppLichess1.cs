using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleAppLichess1
{
    class Program
    {
        static void Main(string[] args)
        {
            string fname = @"J:\tbd\lichess_db_standard_rated_2017-04.pgn";
            FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read);

            StreamReader sr = new StreamReader(fs, Encoding.UTF8);

            var res = new Dictionary<string, int>();
            //var res = new SortedDictionary<string, int>(new SortComp());

            string sline = string.Empty;
            string sevent = "";
            string stime = "";
            while ((sline = sr.ReadLine()) != null)
            {
                if (sline.IndexOf("[Event") == 0)
                {
                    sevent = sline;
                    if (sline.IndexOf("tournament") != -1)
                    {
                        sevent = CleanTourny(sevent);
                    }

                }
                if (sline.IndexOf("[TimeControl") == 0)
                {
                    if (sevent.IndexOf("Correspondence") != -1)
                        continue;
                    stime = sline;
                    var key = sevent + " " + stime;
                    if (res.ContainsKey(key))
                        res[key]++;
                    else
                        res[key] = 1;
                }
            }
            var sortres = new SortedDictionary<string, int>(res, new SortComp());

            var sout = PrintOut(sortres);

            Console.WriteLine(sout);

            Console.ReadKey();

        }

        private static string PrintOut(SortedDictionary<string, int> res)
        {
            var maxlen = res.Keys.Max(x => x.Length);
            var str = new StringBuilder();
            str.Append(" Time Control ".PadRight(maxlen));
            str.Append(" Count");
            str.Append("\r\n");
            foreach (var item in res)
            {
                str.Append(item.Key.PadRight(maxlen));
                str.Append(" ");
                str.Append(item.Value);
                str.Append("\r\n");
            }
            return str.ToString();

        }


        static Dictionary<string, string> cleannames = new Dictionary<string, string>();
        private static string CleanTourny(string sevent)
        {
            if (cleannames.ContainsKey(sevent))
                return cleannames[sevent];

            Regex reg = new Regex(" https[^\"]+");
            var res = reg.Replace(sevent, "");
            cleannames[sevent] = res;
            return res;
        }




    }

    class SortComp : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            //[TimeControl "600+0"]
            Regex reg = new Regex("TimeControl \"([0-9]+)\\+([0-9]+)\"");
            var mx = reg.Match(x);
            var mx1 = mx.Groups[1].Captures[0].Value;
            var mx2 = mx.Groups[2].Captures[0].Value;
            var mx1i = int.Parse(mx1);
            var mx2i = int.Parse(mx2);

            var my = reg.Match(y);
            var my1 = my.Groups[1].Captures[0].Value;
            var my2 = my.Groups[2].Captures[0].Value;
            var my1i = int.Parse(my1);
            var my2i = int.Parse(my2);

            if (mx1i.CompareTo(my1i) != 0)
                return mx1i.CompareTo(my1i);
            else if (mx2i.CompareTo(my2i) != 0)
                return mx2i.CompareTo(my2i);
            else 

            return x.CompareTo(y);

        }


    }
}
