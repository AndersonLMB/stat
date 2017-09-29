using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using CsQuery;
namespace Sp
{
    class Town : IPlace
    {
        public List<Village> Villages = new List<Village>();
        public string Name { get; set; }
        public string Number { get; set; }
        public string URL { get; set; }
        public County County { get; set; }
        public int ChildrenShouldHas = 0;
        public int ChildrenCurrentHas = 0;

        public string GetFullName()
        {
            return County.GetFullName() + Name;
        }

        public Town(string name, string url)
        {
            this.Name = name;
            this.URL = url;

        }

        public Town(string name)
        {
            this.Name = name;
        }



        public void Start()
        {
            //Console.WriteLine(GetFullName());
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri(URL));
            client.DownloadStringCompleted += (sender, e) =>
            {
                if (e.Error == null)
                {
                    HandleDownloadCompleted(sender, e);
                }
                else
                {
                    client.DownloadStringAsync(new Uri(URL));
                }
            };

        }
        private void HandleDownloadCompleted(Object sender, DownloadStringCompletedEventArgs e)
        {
            //Console.WriteLine(e.Result);
            CQ doc = e.Result;
            CQ tables = doc[".villagetr"];
            ChildrenShouldHas = tables.Length;
            foreach(var table in tables)
            {
                Village village = new Village();
                village.Name = table.ChildNodes[2].FirstChild.ToString();
                village.CategoryType = table.ChildNodes[1].FirstChild.ToString();
                village.Number= table.ChildNodes[0].FirstChild.ToString();
                village.Town = this;
                Villages.Add(village);
                Console.WriteLine(village.GetFullName());
            }
        }


    }
}
