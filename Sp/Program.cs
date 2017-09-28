using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Xml;
using System.Web.UI;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Sp
{
    class Program
    {
        static void Main(string[] args)
        {
            #region
            //WebClient client = new WebClient();
            //client.Encoding = Encoding.Default;
            //"Content-Type"


            //string html = client.DownloadString("http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/");
            //HtmlDocument doc = new HtmlDocument();
            //doc.LoadHtml(html);
            //HtmlNodeCollection htmlNodeCollection = doc.DocumentNode.SelectNodes("//tr");

            //List<HtmlNode> nodes = htmlNodeCollection.ToList<HtmlNode>();

            //IEnumerable<HtmlNode> nodes = htmlNodeCollection.AsEnumerable();
            //htmlNodeCollection.
            //Console.WriteLine(htmlNodeCollection.AsEnumerable<HtmlNode>());
            //doc.DocumentNode.SelectNodes("")

            //var nodes=doc.DocumentNode.SelectNodes
            ;

            //string pattern = @"<*=([^'""][^\s>""']{0,}[^'""])[\s>]";
            //var matches = Regex.Matches(html, pattern);
            //foreach (Match match in Regex.Matches(html, pattern))
            //{
            //    //Console.WriteLine(match.Value);
            //    string newstring = match.Value.Replace("=", "=\"");
            //    //newstring += "\"";

            //    //match.Value = newstring;

            //    Console.WriteLine(newstring);
            //    Console.WriteLine(match.Value);
            //};
            //Regex rgx = new Regex(pattern);
            //MatchEvaluator evaluator = new MatchEvaluator(Program.ReplaceCC);
            //string result = rgx.Replace(html, evaluator);
            //HtmlDocument doc = new HtmlDocument();
            //doc.LoadHtml(result);
            //XmlDocument xml = new XmlDocument();
            //xml.LoadXml(result);
            //Console.WriteLine(html);
            #endregion

            Nation china = new Nation("http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/");
            china.Start();

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


    class Nation : IPlace
    {
        public List<Province> Provinces = new List<Province>();
        public string Name { get; set; }
        public string URL { get; set; }
        public string HTML { get; set; }
        public HtmlDocument Doc = new HtmlDocument();

        public Nation(string url)
        {
            this.URL = url;
        }

        public void Start()
        {
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri(URL));
            client.DownloadStringCompleted += (sender, e) =>
            {
                HandleDownloadCompleted(sender, e);
            };
            //Doc.LoadHtml(HTML);
        }
        private void HandleDownloadCompleted(Object sender, DownloadStringCompletedEventArgs e)
        {
            Doc.LoadHtml(e.Result);
            Console.WriteLine(e.Result);
            ;
        }


    }
    class Province
    {

    }

    class City
    {

    }
    class County
    {

    }
    class Village
    {

    }
}
