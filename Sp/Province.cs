using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using System.Linq;
using System.Collections;
using CsQuery;

namespace Sp
{
    class Province : IPlace
    {
        public List<City> Cities = new List<City>();
        public string Name { get; set; }

        public string URL { get; set; }
        public string HTML { get; set; }
        public int ChildrenShouldHas = 0;
        public int ChildrenCurrentHas = 0;

        public Nation Nation { get; set; }
        public HtmlDocument Doc = new HtmlDocument();

        public string GetFullName()
        {
            return Nation.Name + Name;
        }

        public Province(string name, string url)
        {
            Name = name;
            URL = url;

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
                    Nation.ChildrenCurrentHas++;
                    if (Nation.ChildrenCurrentHas == Nation.ChildrenShouldHas)
                    {
                        Console.WriteLine(Nation.Name + " 子节点下载完成 ");
                        ;
                    }
                }
                else
                {
                    client.DownloadStringAsync(new Uri(URL));
                }

            };
        }

        private void HandleDownloadCompleted(Object sender, DownloadStringCompletedEventArgs e)
        {
            //Doc.LoadHtml(e.Result);
            CQ doc = e.Result;
            CQ tables = doc[".citytr"];
            ChildrenShouldHas = tables.Length;
            foreach (var table in tables)
            {

                //Console.WriteLine(table.FirstChild.FirstChild.Attributes.GetAttribute("href")
                //+ " " + table.FirstChild.FirstChild.InnerText
                //+ " " + table.ChildNodes[1].FirstChild.FirstChild.ToString());
                City city = new City(table.ChildNodes[1].FirstChild.FirstChild.ToString(), Nation.URL + table.FirstChild.FirstChild.Attributes.GetAttribute("href"));
                city.Number = table.FirstChild.FirstChild.InnerText;
                //Console.WriteLine(city.Name + " " + city.Number + " " + city.URL);
                city.Province = this;
                //Console.WriteLine(city.GetFullName());
                Cities.Add(city);
                city.Start();
                ;
                //City  
            }
        }
        //public HtmlDocument Doc = new HtmlDocument();

    }
}
