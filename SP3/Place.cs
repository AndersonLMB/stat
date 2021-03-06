﻿using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using CsQuery;
using System.Threading;
using NpgsqlTypes;
using Npgsql.PostgresTypes;

namespace SP3
{
    public class PlaceDBModel
    {
        public string placecode { get; set; }
        public string placename { get; set; }
        public string placetype { get; set; }
        public int placeson { get; set; }
        public int placesontraversed { get; set; }
        public bool placetraversed { get; set; }
        public bool placepagesuccess { get; set; }
        public string placecxtype { get; set; }
        public string placeurl { get; set; }
        public int placefather { get; set; }
    }

    public enum PlaceType
    {
        Nation, Province, City, County, Town, Village
    }
    public delegate void TraversedDelegate(object sender, TraversedEventArgs e);
    public delegate void PageSuccessDelegate(object sender, PageSuccessEventArgs e);
    public delegate void TraversedAddedDelegate(object sender, TraversedAddedEventArgs e);

    public class PageSuccessEventArgs
    {
        public Place ThisPlace = new Place();
        public List<Place> ThisChildrenPlace = new List<Place>();
        public PageSuccessEventArgs(Place place)
        {
            ThisPlace = place;
            ThisChildrenPlace = place.ChildrenCollection;
        }
        public string TestPrint
        {
            get
            {
                return null;
            }

            set {; }
        }
    }

    public class TraversedEventArgs
    {
        public Place ThisPlace { get; set; }
        public TraversedEventArgs()
        {

        }
    }

    public class TraversedAddedEventArgs
    {
        public Place ThisPlace = new Place();
        public Place AddedPlace = new Place();
    }

    public class Place
    {
        public bool IsEndPoint { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public PlaceType PlaceType { get; set; }
        public Place Father { get; set; }
        public List<Place> ChildrenCollection { get; set; }

        public int ChildrenShouldHas { get; set; }

        public int ChildrenCurrentTraversed { get; set; }

        private int _childrenCurrentTraversed = 0;

        protected int GetChildrenCurrentTraversed()
        {
            return _childrenCurrentTraversed;
        }

        protected void SetChildrenCurrentTraversed(int value)
        {
            _childrenCurrentTraversed = value;
        }

        public bool PageSuccess { get; set; }

        private bool _traversed = false;
        public bool Traversed
        {
            get { return _traversed; }
            set
            {
                _traversed = value;
                if (value)
                {
                    if (OnTraversed != null)
                    {
                        OnTraversed(this, new TraversedEventArgs() { ThisPlace = this });
                    }
                    if (Father != null)
                    {
                        Father.ChildrenCurrentTraversed++;
                        if (Father.OnTraversedAdded != null)
                        {
                            Father.OnTraversedAdded(this, new TraversedAddedEventArgs() { AddedPlace = this, ThisPlace = Father });
                        }
                        //Console.WriteLine(Father.FullName() + " " + Father.ChildrenCurrentTraversed + "/" + Father.ChildrenShouldHas);
                        if (Father.ChildrenShouldHas == Father.ChildrenCurrentTraversed)
                        {
                            Father.Traversed = true;
                        }
                    }
                }
            }
        }

        public Place()
        {
            ChildrenCurrentTraversed = 0;
            //Traversed = false;
        }

        public event PageSuccessDelegate OnPageSuccess;

        public event TraversedDelegate OnTraversed;

        public event TraversedAddedDelegate OnTraversedAdded;

        public async Task ClickInAsync()
        {
            WebClient client = new WebClient();
            string a = await client.DownloadStringTaskAsync(URL);
            Console.WriteLine(a);
        }

        public void Start()
        {
            //URL不为空
            if (URL != null)
            {
                TryGetPage();
                IsEndPoint = false;
            }
            else
            {
                Traversed = true;
                IsEndPoint = true;
            }

        }

        private void TryGetPage()
        {
            //Thread.Sleep(3);
            WebClient client = new WebClient();

            client.DownloadStringCompleted -= OnDownloaded;
            client.DownloadStringAsync(new Uri(URL));
            client.DownloadStringCompleted += OnDownloaded;
            //Thread.Sleep(30);
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
                PageSuccess = true;
                if (OnPageSuccess != null)
                {
                    PageSuccessEventArgs pageSuccessEventArgs = new PageSuccessEventArgs(this);
                    OnPageSuccess(this, pageSuccessEventArgs);
                }
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

        public string FullName() => Father != null ? Father.FullName() + Name : Name;
        public string FullName(string split) => Father != null ? Father.FullName() + split + Name : Name;
        public string FullName(char split) => Father != null ? Father.FullName() + split + Name : Name;

        public override string ToString()
        {
            return FullName();
        }


        //返回插入字段
        public string InsertSelf()
        {
            string sqlInsert;
            ////string temp = "\'{0}\', \'{1}\', \'{2}\', {3}, {4}, {5},{6} , '{7}', '{8}', '{9}', '{10}'";
            //string temp = "INSERT INTO public.places(placecode, placename, placetype, placeson, placesontraversed, placetraversed, placepagesuccess, placecxtype, placeurl, placefather, placeisendpoint) VALUES('{0}', '{1}', '{2}', {3}, {4}, {5},{6} , '{7}', '{8}', '{9}', '{10}';";
            ////string sqlInsert = string.Format(temp, [Code.ToString(), Name.ToString(), PlaceType.ToString(), ChildrenShouldHas.ToString(), ChildrenCurrentTraversed.ToString(), Traversed.ToString(), PageSuccess.ToString(), "", URL.ToString(), IsEndPoint.ToString()]);
            if (URL != null)
            {
                sqlInsert = "INSERT INTO public.places(placecode, placename, placetype, placeson, placesontraversed, placetraversed, placepagesuccess, placeurl, placefather, placeisendpoint) VALUES('" + Code.ToString() + "',"
     + "'" + Name.ToString() + "',"
     + "'" + PlaceType.ToString() + "',"
     + "" + ChildrenShouldHas.ToString() + ","
     + "" + ChildrenCurrentTraversed.ToString() + ","
     + "" + Traversed.ToString() + ","
     + "" + PageSuccess.ToString() + ","
     + "'" + URL.ToString() + "',"
     + "'" + Father.Code.ToString() + "',"
     + "" + IsEndPoint.ToString() + ");";
            }
            else
            {
                sqlInsert = "INSERT INTO public.places(placecode, placename, placetype, placeson, placesontraversed, placetraversed, placepagesuccess, placefather, placeisendpoint) VALUES('" + Code.ToString() + "',"
+ "'" + Name.ToString() + "',"
+ "'" + PlaceType.ToString() + "',"
+ "" + ChildrenShouldHas.ToString() + ","
+ "" + ChildrenCurrentTraversed.ToString() + ","
+ "" + Traversed.ToString() + ","
+ "" + PageSuccess.ToString() + ","
+ "'" + Father.Code.ToString() + "',"
+ "" + IsEndPoint.ToString() + ");";
            }


            ////string sqlInsert= string.Format()
            //string temp1 = "{0},{1}";
            //string t1 = string.Format(temp1, "s", "ss");
            ////"INSERT INTO public.places(placecode, placename, placetype, placeson, placesontraversed, placetraversed, placepagesuccess, placecxtype, placeurl, placefather, placeisendpoint) VALUES('s', 's', 's', 1, 1, true, true, '1', 'ss', 'ssss', false);";
            return sqlInsert;
            ////return null;
        }

        public string UpdateSelfTraversed()
        {
            string sqlUpdate;
            //UPDATE public.places SET placecode=?, placename=?, placetype=?, placeson=?, placesontraversed=?, placetraversed=?, placepagesuccess=?, placecxtype=?, placeurl=?, placefather=?, placeisendpoint WHERE<condition>;
            sqlUpdate = "UPDATE public.places SET placetraversed=true WHERE placecode='" + Code.ToString() + "';";
            return sqlUpdate;
        }

        public string UpdateSelfSonsCount()
        {
            string sqlUpdate;
            sqlUpdate = "UPDATE public.places SET placeson=" + ChildrenShouldHas.ToString() + " WHERE placecode='" + Code.ToString() + "';";
            return sqlUpdate;
        }

        public string UpdateSelfTraversedAdded()
        {
            string sqlUpdate;
            sqlUpdate = "UPDATE public.places SET placesontraversed=" + ChildrenCurrentTraversed.ToString() + " WHERE placecode='" + Code.ToString() + "';";
            return sqlUpdate;
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
            ChildrenShouldHas = elements.Length;
            List<Place> provinces = new List<Place>();
            foreach (var element in elements)
            {
                ProvincePlace province = new ProvincePlace
                {
                    Father = this,
                    Name = element.FirstChild.ToString()
                };
                if (element.HasAttribute("href"))
                {
                    province.URL = this.URL + element.Attributes.GetAttribute("href");
                    province.Code = province.URL.Split('/')[province.URL.Split('/').Length - 1].Split('.')[0] + "0000000000";
                }

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
            ChildrenShouldHas = elements.Length;
            List<Place> cities = new List<Place>();
            foreach (var element in elements)
            {
                CityPlace city = new CityPlace()
                {
                    Father = this,
                    Name = element.ChildNodes[1].FirstChild.FirstChild.ToString(),
                    Code = element.FirstChild.FirstChild.FirstChild.ToString(),
                };

                //if (city.Father.Code == null)
                //{
                //    city.Father.Code = city.Code.ToCharArray()[0].ToString() + city.Code.ToCharArray()[1].ToString() + "00000000";
                //}
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
            ChildrenShouldHas = elements.Length;
            List<Place> counties = new List<Place>();
            foreach (var element in elements)
            {
                CountyPlace county = new CountyPlace();
                county.Father = this;
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
                }
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
            ChildrenShouldHas = elements.Length;
            List<Place> towns = new List<Place>();
            foreach (var element in elements)
            {
                TownPlace town = new TownPlace();
                town.Father = this;
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
                    //town.Traversed = true;
                }

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
            ChildrenShouldHas = elements.Length;
            List<Place> villages = new List<Place>();
            foreach (var element in elements)
            {
                villages.Add(new VillagePlace()
                {
                    Name = element.ChildNodes[2].FirstChild.ToString(),
                    Code = element.FirstChild.FirstChild.ToString(),
                    CXType = element.ChildNodes[1].FirstChild.ToString(),
                    Father = this,
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

        }
        public override List<Place> TryGetChildren(string result)
        {
            return base.TryGetChildren(result);
        }

    }

}
