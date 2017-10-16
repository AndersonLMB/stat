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
        public List<Place> Children { get; set; }


        public bool Traversed { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
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

        //private async Task AnalyzePageAsync()
        //{


        //}

        private void TryGetPage()
        {
            WebClient client = new WebClient();
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
            //throw new NotImplementedException();
        }
        private void OnDownloadSuccess(string result)
        {
            Children = TryGetChildren(result);

        }

        public virtual List<Place> TryGetChildren(string result)
        {
            return null;
        }

        //private List<Place> TryGetChildren(string result)
        //{
        //    //List<Place> childrenCollection = new List<Place>();

        //    //switch (PlaceType)
        //    //{
        //    //    default: break;
        //    //        //case PlaceType.Nation:
        //    //        //    {
        //    //        //        childrenCollection = TryGetNationChildren(result);
        //    //        //        break;
        //    //        //    };
        //    //        //case PlaceType.Province:
        //    //        //    {
        //    //        //        childrenCollection = TryGetProvinceChildren(result);
        //    //        //        break;
        //    //        //    }
        //    //        //case PlaceType.City:
        //    //        //    {
        //    //        //        childrenCollection = TryGetCityChildren(result);
        //    //        //        break;
        //    //        //    };
        //    //}
        //    //return childrenCollection;
        //    ////return null;
        //}

        private void OnGetPage()
        {

        }

        //private void On

        public string FullName() => Father != null ? Father.FullName() + Name : Name;
        public string FullName(string split) => Father != null ? Father.FullName() + split + Name : Name;
        public string FullName(char split) => Father != null ? Father.FullName() + split + Name : Name;

        //public virtual List<Place> TryGetChildren(string htmlResult) { }
        delegate List<Place> TryGetChildrenCollection(string htmlResult);

        public virtual List<Place> TryGetNationChildren(string htmlResult)
        {
            //CQ cq = new CQ(htmlResult);
            CQ elements = new CQ(htmlResult)[".provincetr a"];
            List<Place> provinces = new List<Place>();
            foreach (var element in elements)
            {
                Place province = new Place
                {
                    PlaceType = PlaceType.Province,
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
            //return null;
        }
        public virtual List<Place> TryGetProvinceChildren(string htmlResult)
        {
            CQ elements = new CQ(htmlResult)[".citytr"];
            List<Place> cities = new List<Place>();
            foreach (var element in elements)
            {
                Place city = new Place()
                {
                    PlaceType = PlaceType.City,
                    Name = element.ChildNodes[1].FirstChild.FirstChild.ToString()
                };
                city.Father = this;
                if (element.FirstChild.FirstChild.HasAttribute("href"))
                {
                    //var stringArray = element.FirstChild.FirstChild.Attributes.GetAttribute("href").Split('/');
                    var stringArray = URL.Split('/');
                    stringArray[stringArray.Length - 1] = element.FirstChild.FirstChild.Attributes.GetAttribute("href");
                    var newstring = String.Join("/", stringArray);
                    city.URL = newstring;
                    //city.Father.Father
                }
                cities.Add(city);
            }
            return cities;
        }
        public virtual List<Place> TryGetCityChildren(string htmlResult)
        {
            CQ elements = new CQ(htmlResult)[".countytr"];
            List<Place> counties = new List<Place>();
            foreach (var element in elements)
            {
                Place county = new Place();
                county.PlaceType = PlaceType.County;
                if (element.FirstChild.FirstChild.HasAttribute("href"))
                {
                    var stringArray = URL.Split('/');
                    stringArray[stringArray.Length - 1] = element.FirstChild.FirstChild.Attributes.GetAttribute("href");
                    var newString = String.Join("/", stringArray);
                    county.URL = newString;
                    county.Name = element.ChildNodes[1].FirstChild.FirstChild.ToString();
                }
                else
                {
                    county.Name = element.ChildNodes[1].FirstChild.ToString();
                    county.Traversed = true;
                }
                county.Father = this;
                counties.Add(county);
            }
            return counties;
        }

        //public List<Place> TryGetCountyChildren(string htmlResult)
        //{
        //    CQ elements = new CQ(htmlResult)[".towntr"];
        //    List<Place> towns = new List<Place>();

        //}



    }
    class Nation : Place
    {
        public override List<Place> TryGetChildren(string htmlResult)
        {
            //CQ cq = new CQ(htmlResult);
            CQ elements = new CQ(htmlResult)[".provincetr a"];
            List<Place> provinces = new List<Place>();
            foreach (var element in elements)
            {
                Province province = new Province
                {
                    PlaceType = PlaceType.Province,
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

    class Province : Place
    {
        public override List<Place> TryGetChildren(string htmlResult)
        {
            CQ elements = new CQ(htmlResult)[".citytr"];
            List<Place> cities = new List<Place>();
            foreach (var element in elements)
            {
                City city = new City()
                {
                    PlaceType = PlaceType.City,
                    Name = element.ChildNodes[1].FirstChild.FirstChild.ToString()
                };
                city.Father = this;
                if (element.FirstChild.FirstChild.HasAttribute("href"))
                {
                    var stringArray = URL.Split('/');
                    stringArray[stringArray.Length - 1] = element.FirstChild.FirstChild.Attributes.GetAttribute("href");
                    var newstring = String.Join("/", stringArray);
                    city.URL = newstring;
                }
                cities.Add(city);
            }
            return cities;
        }
    }

    class City : Place
    {
        public override List<Place> TryGetChildren(string htmlResult)
        {
            CQ elements = new CQ(htmlResult)[".countytr"];
            List<Place> counties = new List<Place>();
            foreach (var element in elements)
            {
                Place county = new Place();
                county.PlaceType = PlaceType.County;
                if (element.FirstChild.FirstChild.HasAttribute("href"))
                {
                    var stringArray = URL.Split('/');
                    stringArray[stringArray.Length - 1] = element.FirstChild.FirstChild.Attributes.GetAttribute("href");
                    var newString = String.Join("/", stringArray);
                    county.URL = newString;
                    county.Name = element.ChildNodes[1].FirstChild.FirstChild.ToString();
                }
                else
                {
                    county.Name = element.ChildNodes[1].FirstChild.ToString();
                    county.Traversed = true;
                }
                county.Father = this;
                counties.Add(county);
            }
            return counties;
        }
    }




}
