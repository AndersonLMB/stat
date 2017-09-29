
using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using System.Linq;
using System.Collections;
using CsQuery;

namespace Sp
{
    class County : IPlace
    {
        public List<Town> towns = new List<Town>();
        public string Name { get; set; }
        public string Number { get; set; }
        public string URL { get; set; }
        public string HTML { get; set; }
        public City City { get; set; }

        public County(string name, string url)
        {
            Name = name;
            URL = url;
        }
        public County(string name)
        {
            Name = name;
        }
        public string GetFullName()
        {
            return City.GetFullName() + Name;
        }

        public void Start()
        {
            //Console.WriteLine(GetFullName());
            WebClient client = new WebClient();
            if (URL != null)
            {
                client.DownloadStringAsync(new Uri(URL));
            }
            client.DownloadStringCompleted += (sender, e) =>
            {
                if (e.Error == null)
                {
                    HandleDownloadCompleted(sender, e);
                    City.ChildrenCurrentHas++;
                    if (City.ChildrenCurrentHas == City.ChildrenShouldHas)
                    {
                        Console.WriteLine(City.Name + " 子节点下载完成 ");
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
            CQ tables = doc[".towntr"];
            foreach (var table in tables)
            {
                string[] stringArray = URL.Split('/');
                Town town;
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
                    town = new Town(table.ChildNodes[1].FirstChild.FirstChild.ToString(), newString)
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
                    town = new Town(table.ChildNodes[1].FirstChild.ToString())
                    {
                        Number = table.FirstChild.FirstChild.ToString()
                    };
                    ;
                }
                town.County = this;
                towns.Add(town);
                town.Start();
                //Console.WriteLine(town.County.City.Province.Nation);
                ;
            }

            ;
        }
    }
}
