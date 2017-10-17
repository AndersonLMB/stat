using System.Collections.Generic;
using System.Web;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using CsQuery;

namespace SP3
{
    public enum PlaceType
    {
        Nation, Province, City, County, Town, Village
    }

    class Place
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public PlaceType PlaceType { get; set; }
        public Place Father { get; set; }
        public List<Place> ChildrenCollection { get; set; }

        public bool PageSuccess { get; set; }
        public bool Traversed { get; set; }

        public Place()
        {
            Traversed = false;

        }


        public async Task ClickInAsync()
        {
            WebClient client = new WebClient();

            string a = await client.DownloadStringTaskAsync(URL);
            Console.WriteLine(a);
        }

        public void Start()
        {
            TryGetPage();

        }

        private void TryGetPage()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted -= OnDownloaded;
            client.DownloadStringAsync(new Uri(URL));
            client.DownloadStringCompleted += OnDownloaded;

        }

        private void OnDownloaded(object sender, DownloadStringCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                TryGetPage();
            }
            else
            {
                OnDownloadSuccess(e.Result);
            }
        }
        private void OnDownloadSuccess(string result)
        {
            ChildrenCollection = TryGetChildren(result);

        }

        public virtual List<Place> TryGetChildren(string result)
        {
            return null;
        }

        private void OnGetPage()
        {

        }
        public string FullName() => Father != null ? Father.FullName() + Name : Name;
        public string FullName(string split) => Father != null ? Father.FullName() + split + Name : Name;
        public string FullName(char split) => Father != null ? Father.FullName() + split + Name : Name;

        public override string ToString()
        {
            return FullName();
        }

        delegate List<Place> TryGetChildrenCollection(string htmlResult);
    }
    class NationPlace : Place
    {
        public NationPlace()
        {
            PlaceType = PlaceType.Nation;
        }

        public override List<Place> TryGetChildren(string htmlResult)
        {
            CQ elements = new CQ(htmlResult)[".provincetr a"];
            List<Place> provinces = new List<Place>();
            foreach (var element in elements)
            {
                ProvincePlace province = new ProvincePlace
                {
                    Name = element.FirstChild.ToString()
                };
                if (element.HasAttribute("href"))
                {
                    province.URL = this.URL + element.Attributes.GetAttribute("href");
                }
                province.Father = this;
                provinces.Add(province);
            }
            return provinces;
        }
    }

    class ProvincePlace : Place
    {
        public ProvincePlace()
        {
            PlaceType = PlaceType.Province;
        }
        public override List<Place> TryGetChildren(string htmlResult)
        {
            CQ elements = new CQ(htmlResult)[".citytr"];
            List<Place> cities = new List<Place>();
            foreach (var element in elements)
            {
                CityPlace city = new CityPlace()
                {
                    Name = element.ChildNodes[1].FirstChild.FirstChild.ToString(),
                    Code = element.FirstChild.FirstChild.FirstChild.ToString(),
                };
                city.Father = this;
                if (element.FirstChild.FirstChild.HasAttribute("href"))
                {
                    var stringArray = URL.Split('/');
                    stringArray[stringArray.Length - 1] = element.FirstChild.FirstChild.Attributes.GetAttribute("href");
                    var newstring = String.Join("/", stringArray);
                    city.URL = newstring;
                }
                else
                {
                }
                cities.Add(city);
            }
            return cities;
        }
    }

    class CityPlace : Place
    {
        public CityPlace()
        {
            PlaceType = PlaceType.City;
        }
        public override List<Place> TryGetChildren(string htmlResult)
        {
            CQ elements = new CQ(htmlResult)[".countytr"];
            List<Place> counties = new List<Place>();
            foreach (var element in elements)
            {
                CountyPlace county = new CountyPlace();
                if (element.FirstChild.FirstChild.HasAttribute("href"))
                {
                    var stringArray = URL.Split('/');
                    stringArray[stringArray.Length - 1] = element.FirstChild.FirstChild.Attributes.GetAttribute("href");
                    var newString = String.Join("/", stringArray);
                    county.URL = newString;
                    county.Name = element.ChildNodes[1].FirstChild.FirstChild.ToString();
                    county.Code = element.FirstChild.FirstChild.FirstChild.ToString();
                }
                else
                {
                    county.Name = element.ChildNodes[1].FirstChild.ToString();
                    county.Code = element.FirstChild.FirstChild.ToString();
                    county.Traversed = true;
                }
                county.Father = this;
                counties.Add(county);
            }
            return counties;
        }
    }

    class CountyPlace : Place
    {
        public CountyPlace()
        {
            PlaceType = PlaceType.County;
        }
        public override List<Place> TryGetChildren(string htmlResult)
        {
            CQ elements = new CQ(htmlResult)[".towntr"];
            List<Place> towns = new List<Place>();
            foreach (var element in elements)
            {
                TownPlace town = new TownPlace();
                if (element.FirstChild.FirstChild.HasAttribute("href"))
                {
                    var stringArray = URL.Split('/');
                    stringArray[stringArray.Length - 1] = element.FirstChild.FirstChild.Attributes.GetAttribute("href");
                    var newString = String.Join("/", stringArray);
                    town.URL = newString;
                    town.Name = element.ChildNodes[1].FirstChild.FirstChild.ToString();
                    town.Code = element.FirstChild.FirstChild.FirstChild.ToString();
                }
                else
                {
                    town.Name = element.ChildNodes[1].FirstChild.ToString();
                    town.Code = element.FirstChild.FirstChild.ToString();
                    town.Traversed = true;
                }
                town.Father = this;
                towns.Add(town);
            }
            return towns;
        }
    }

    class TownPlace : Place
    {
        public TownPlace()
        {
            PlaceType = PlaceType.Town;
        }

        public override List<Place> TryGetChildren(string htmlResult)
        {
            CQ elements = new CQ(htmlResult)[".villagetr"];
            List<Place> villages = new List<Place>();
            foreach (var element in elements)
            {
                villages.Add(new VillagePlace()
                {
                    Name = element.ChildNodes[2].FirstChild.ToString(),
                    Code = element.FirstChild.FirstChild.ToString(),
                    CXType = element.ChildNodes[1].FirstChild.ToString(),
                    Father = this,
                    Traversed = true
                });
            }
            return villages;
        }
    }

    class VillagePlace : Place
    {
        public string CXType { get; set; }
        public VillagePlace()
        {
            PlaceType = PlaceType.Village;

        }
        public override List<Place> TryGetChildren(string result)
        {
            return base.TryGetChildren(result);
        }

    }

}
