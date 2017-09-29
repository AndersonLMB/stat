using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using System.Linq;
using System.Collections;
using CsQuery;

namespace Sp
{
    class Nation : IPlace
    {
        public List<Province> Provinces = new List<Province>();
        public string Name { get; set; }
        public string URL { get; set; }
        public string HTML { get; set; }
        public int ChildrenShouldHas = 0;
        public int ChildrenCurrentHas = 0;
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
                if (e.Error == null)
                {
                    HandleDownloadCompleted(sender, e);
                }
                else
                {
                    client.DownloadStringAsync(new Uri(URL));
                }
            };
            //Console.WriteLine(URL);
        }
        private void HandleDownloadCompleted(Object sender, DownloadStringCompletedEventArgs e)
        {
            //Doc.LoadHtml(e.Result);
            //CQ cq = e.Result;
            //CQ tds = cq["td"];
            CQ doc = e.Result;
            CQ tables = doc[".provincetr a"];
            ChildrenShouldHas = tables.Length;
            foreach (var table in tables)
            {
                Province province = new Province(table.FirstChild.NodeValue, URL + table.Attributes.GetAttribute("href"));
                province.Nation = this;
                //Console.WriteLine(province.GetFullName());
                this.Provinces.Add(province);
                province.Start();
                ;
            }
            ;
            //Console.WriteLine(e.Result);
            //HtmlNodeCollection htmlNodeCollection = Doc.DocumentNode.SelectNodes("//td");
            //List<HtmlNode> list = htmlNodeCollection.ToList<HtmlNode>();
            //IEnumerable<HtmlNode> nodes = htmlNodeCollection.AsEnumerable();
            //List<HtmlNode> nodes = htmlNodeCollection.ToList<HtmlNode>();

        }


    }
}
