using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using System.Linq;
using System.Collections;
using CsQuery;


namespace Sp
{
    class City : IPlace
    {
        public List<County> Counties = new List<County>();
        public string Name { get; set; }
        public string Number { get; set; }
        public string URL { get; set; }
        public string HTML { get; set; }
        public HtmlDocument Doc = new HtmlDocument();
        public Province Province { get; set; }
        public int ChildrenShouldHas = 0;
        public int ChildrenCurrentHas = 0;

        public string GetFullName()
        {
            return this.Province.GetFullName() + Name;
        }

        public City(string name, string url)
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
                    Province.ChildrenCurrentHas++;
                    if (Province.ChildrenCurrentHas == Province.ChildrenShouldHas)
                    {
                        Console.WriteLine(Province.Name + " 子节点下载完成 ");
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
            CQ doc = e.Result;
            CQ tables = doc[".countytr"];
            foreach (var table in tables)
            {
                //Console.WriteLine(URL);
                //Console.WriteLine(URL.LastIndexOf('/') + " " + table.FirstChild.FirstChild.Attributes.GetAttribute("href"));
                //Console.WriteLine(URL.Insert(URL.LastIndexOf('/') + 1, table.FirstChild.FirstChild.Attributes.GetAttribute("href")));
                string[] stringArray = URL.Split('/');
                County county;
                if (table.FirstChild.FirstChild.HasAttribute("href"))
                {
                    stringArray[stringArray.Length - 1] = table.FirstChild.FirstChild.Attributes.GetAttribute("href");
                    string newString = "";
                    for (var i = 0; i < stringArray.Length; i++)
                    {
                        if (i == 0)
                        {
                            newString += stringArray[i];
                        }
                        else
                        {
                            newString += '/' + stringArray[i];
                        }
                    }
                    county = new County(table.ChildNodes[1].FirstChild.FirstChild.ToString(), newString)
                    {
                        Number = table.FirstChild.FirstChild.FirstChild.ToString()
                    };
                }
                else
                {
                    string newString = "";
                    for (var i = 0; i < stringArray.Length; i++)
                    {
                        if (i == 0)
                        {
                            newString += stringArray[i];
                        }
                        else
                        {
                            newString += '/' + stringArray[i];
                        }
                    }
                    county = new County(table.ChildNodes[1].FirstChild.ToString())
                    {
                        Number = table.FirstChild.FirstChild.ToString()
                    };
                    ;
                }
                county.City = this;
                //Console.WriteLine(county.GetFullName());
                Counties.Add(county);
                county.Start();
                ;
                //County county=new County()
                //string newString = "";
                //for (var i = 0; i < stringArray.Length; i++)
                //{
                //    if (i == 0)
                //    {
                //        newString += stringArray[i];
                //    }
                //    else
                //    {
                //        newString += '/' + stringArray[i];
                //    }
                //}
                //County county = new County(table.ChildNodes[1].FirstChild.FirstChild.ToString(), newString)
                //{
                //    Number = table.FirstChild.FirstChild.FirstChild.ToString()
                //};
            }
        }

    }


}
