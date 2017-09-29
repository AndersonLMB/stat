using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace Sp
{
    class Program
    {
        static void Main(string[] args)
        {

            Nation china = new Nation("http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/");
            china.Name = "中华人民共和国";
            china.Start();

            //Province jiangsu = new Province("江苏省", "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/32.html");
            //City nanjing = new City("南京市", "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/32/3201.html");
            //County xuanwu = new County("玄武区", "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/32/01/320102.html");
            //xuanwu.City = nanjing;
            //xuanwu.Start();

            //http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/14/05/140522.html


            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

        }

        public static string ReplaceCC(Match m)
        {
            string newstring = m.ToString();
            newstring = newstring.Insert(1, "\"");

            if (newstring.Split('>').Length == 1)
            {
                newstring += "\" ";
            }
            else
            {
                newstring = newstring.Split('>')[0] + "\"> ";
            }
            return newstring;
        }

    }

    interface IPlace
    {
        void Start();
        //string Name();
    }
}
