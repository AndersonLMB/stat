using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using CsQuery;
using Dapper;
using Npgsql;
using System.Configuration;
using System.IO;

namespace SP2
{
    public static class DB
    {
        public static NpgsqlConnection Connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["users"].ConnectionString);
    }

    public static class TXT
    {

        //public static string path = @"c:\temp\mytest.txt";
        public static StreamWriter SW = new StreamWriter("C:\\temp\\mytest.sql");
        public static Queue<string> SqlQuene = new Queue<string>();
        public static bool IsWritinng = false;
        public static void WriteSQL(string sql)
        {
            Console.WriteLine(sql);
            SqlQuene.Enqueue(sql);
            if (!IsWritinng)
            {
                WriteFile();
            }
        }
        public static void WriteFile()
        {
            IsWritinng = true;
            //SW.WriteLine(SqlQuene.Dequeue());
            if (SqlQuene.Count > 0)
            {
                SW.WriteLine(SqlQuene.Dequeue());
                WriteFile();
            }
            else
            {
                IsWritinng = false;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            #region Testing
            Nation china = new Nation
            {
                Name = "中华人民共和国",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/",
                Type = "Nation",
            };
            china.Start();
            //try
            //{

            //    //Pass the filepath and filename to the StreamWriter Constructor
            //    StreamWriter sw = new StreamWriter("C:\\temp\\mytest.txt");

            //    //Write a line of text
            //    sw.WriteLine("Hello World!!");

            //    //Write a second line of text
            //    sw.WriteLine("From the StreamWriter class");

            //    //Close the file
            //    sw.Close();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Exception: " + e.Message);
            //}
            //finally
            //{
            //    Console.WriteLine("Executing finally block.");
            //}


            //Province hebei = new Province()
            //{
            //    Name = "河北省",
            //    URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13.html",
            //    Type = "Province",
            //};
            //hebei.Start();

            //City shijiazhuang = new City()
            //{
            //    Name = "佛山市",
            //    URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/44/4406.html",
            //    Type = "City"
            //};
            //shijiazhuang.Start();

            //county changanqu = new county()
            //{
            //    name = "长安区",
            //    url = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13/01/130102.html",
            //    type = "county"
            //};
            //changanqu.start();

            //Town jianbeijiedaobanshichu = new Town()
            //{
            //    Name = "建北街道办事处",
            //    URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13/01/02/130102001.html",
            //    Type = "Town"
            //};
            //jianbeijiedaobanshichu.Start();
            #endregion

            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }
    }

    class Place
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public bool Traversed = false;
        public int ChildrenShouldHas;
        public int ChildrenCurrentHas;
        public string CXType { get; set; }

        public Place Father { get; set; }
        public List<Place> Children = new List<Place>();

        public void AppendChildPlace(Place place)
        {
            Children.Add(place);

        }


        public string GetFullName() => Father != null ? Father.GetFullName() + Name : Name;

        public void Start()
        {
            //TXT.WriteSQL("UPDATE public.placestable SET placeurl = '" + URL + "' WHERE placefullname='" + GetFullName() + "' ;");

            //Console.WriteLine(GetFullName());
            SaveToDatabase();
            //SaveToTXT();
            //Console.WriteLine(GetFullName());

            //Console.WriteLine(GetFullName());
            if (URL != null)
            {
                Download();
            }
            else
            {
                Traversed = true;

                TXT.WriteSQL("UPDATE public.placestable SET placetraversed = true WHERE placefullname='" + GetFullName() + "' ;");

                //Console.WriteLine(GetFullName());
                Father.ChildrenCurrentHas++;
                //Console.WriteLine("    " + Name + " | " + Father.GetFullName() + " : " + Father.ChildrenCurrentHas + '/' + Father.ChildrenShouldHas);
            }
            if (Father != null)
            {

                if (Father.ChildrenCurrentHas == Father.ChildrenShouldHas)
                {

                    Father.Traversed = true;
                    TXT.WriteSQL("UPDATE public.placestable SET placetraversed = true WHERE placefullname='" + Father.GetFullName() + "' ;");
                    ;
                    //if (Father.Type == "Province" || Father.Type == "Nation" || Father.Type == "City")
                    //{
                    //    Console.WriteLine(Father.GetFullName() + " traversed");
                    //    TXT.FinalWrite();
                    //}
                    //Console.WriteLine(">>> " + Father.GetFullName());
                }
            }
        }
        private void SaveToTXT()
        {
            //TXT.writer.WriteLine(GetFullName() + "\t" + Name + "\t" + Type + "\t" + Father.GetFullName() + "\t" + Code + "\t" + CXType + "\t");
        }

        private void SaveToDatabase()
        {
            //string sql = " ";
            //switch (Type)
            //{
            //    case "Nation":
            //        sql = "INSERT INTO place.nations(nationfullname,nationname) VALUES( '" + GetFullName() + "' , '" + Name + "' );";
            //        break;
            //    case "Province":
            //        sql = "INSERT INTO place.provinces(provincefullname,provincename,provincefather) VALUES( '" + GetFullName() + "' , '" + Name + "' , '" + Father.GetFullName() + "' );";
            //        break;
            //    case "City":
            //        sql = "INSERT INTO place.cities(cityfullname,cityname,cityfather,citycode) VALUES( '" + GetFullName() + "' , '" + Name + "' , '" + Father.GetFullName() + "' , '" + Code + "' );";
            //        break;
            //    case "County":
            //        sql = "INSERT INTO place.counties(countyfullname,countyname,countyfather,countycode) VALUES( '" + GetFullName() + "' , '" + Name + "' , '" + Father.GetFullName() + "' , '" + Code + "' );";
            //        break;
            //    case "Town":
            //        sql = "INSERT INTO place.towns(townfullname,townname,townfather,towncode) VALUES( '" + GetFullName() + "' , '" + Name + "' , '" + Father.GetFullName() + "' , '" + Code + "' );";
            //        break;
            //    case "Village":
            //        sql = "INSERT INTO place.villages(villagefullname,villagename,villagefather,villagecode) VALUES( '" + GetFullName() + "' , '" + Name + "' , '" + Father.GetFullName() + "' , '" + Code + "' , '" + CXType + "' );";
            //        break;
            //    default:
            //        sql = "";
            //        break;
            //}


            string sql = "INSERT INTO public.placestable(placefullname,placename,placetype,placecode,placefather) VALUES( '" + GetFullName() + "' , '" + Name + "' , '" + Type + "' , '" + (Code == null ? "non" : Code) + "' , '" + (Father == null ? "non" : Father.GetFullName()) + "' );";

            //Console.WriteLine(sql);
            //try { }
            //catch { }
            TXT.WriteSQL(sql);
            TXT.WriteSQL("UPDATE public.placestable SET placeurl = '" + URL + "' WHERE placefullname='" + GetFullName() + "' ;");
            //Console.WriteLine("  >>  " + sql);
            //try
            //{
            //    var count = DB.Connection.Execute(sql);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERR >" + GetFullName());
            //    //Console.WriteLine(ex);

            //}

            //Console.WriteLine(count);
            ;
            //var count = DB.Connection.Execute(sql, new { a = GetFullName(), b = Name, c = Type, d = Code == null ? "" : Code });

        }

        public void Download()
        {

            WebClient client = new WebClient();
            //var result = client.DownloadData(new Uri(URL));
            //string result = client.DownloadString(new Uri(URL));
            client.DownloadStringAsync(new Uri(URL));
            //client.DownloadString(new Uri(URL));
            client.DownloadStringCompleted += (sender, e) =>
            {
                if (e.Error == null)
                {
                    OnDownloadSuccess(sender, e);
                }
                else
                {
                    client.DownloadStringAsync(new Uri(URL));
                }
            };
        }

        public virtual void OnDownloadSuccess(Object sender, DownloadStringCompletedEventArgs e)
        {
            ;
        }

        public void LogSelf()
        {

        }

    }

    class Nation : Place
    {
        public override void OnDownloadSuccess(Object sender, DownloadStringCompletedEventArgs e)
        {
            CQ doc = e.Result;
            CQ elements = doc[".provincetr a"];
            ChildrenShouldHas = elements.Length;
            TXT.WriteSQL("UPDATE public.placestable SET placeson = " + ChildrenShouldHas + " WHERE placefullname='" + GetFullName() + "' ;");
            foreach (var element in elements)
            {
                Province province = new Province
                {
                    Father = this,
                    Name = element.FirstChild.ToString(),
                    URL = URL + element.Attributes.GetAttribute("href"),
                    Type = "Province"
                };

                //Children.Add(province);
                AppendChildPlace(province);
                province.Start();
                //Console.WriteLine(province.GetFullName());
                base.OnDownloadSuccess(sender, e);
            }
        }
    }

    class Province : Place
    {
        public override void OnDownloadSuccess(object sender, DownloadStringCompletedEventArgs e)
        {
            CQ doc = e.Result;
            CQ elements = doc[".citytr"];
            ChildrenShouldHas = elements.Length;
            TXT.WriteSQL("UPDATE public.placestable SET placeson = " + ChildrenShouldHas + " WHERE placefullname='" + GetFullName() + "' ;");
            foreach (var element in elements)
            {
                #region converting url
                string[] stringArray = URL.Split('/');
                stringArray[stringArray.Length - 1] = element.FirstChild.FirstChild.Attributes.GetAttribute("href");
                string newString = "";
                for (var i = 0; i < stringArray.Length; i++)
                {
                    newString += i == 0 ? stringArray[i] : '/' + stringArray[i];
                }
                #endregion
                City city = new City
                {
                    Father = this,
                    Name = element.ChildNodes[1].FirstChild.FirstChild.ToString(),
                    Code = element.FirstChild.FirstChild.FirstChild.ToString(),
                    Type = "City",
                    URL = newString
                };
                AppendChildPlace(city);
                //Children.Add(city);
                city.Start();
                //Console.WriteLine(city.GetFullName());
                //URL = element.FirstChild.FirstChild.Attributes.GetAttribute("href");

            }
            base.OnDownloadSuccess(sender, e);
        }
    }

    class City : Place
    {
        public override void OnDownloadSuccess(object sender, DownloadStringCompletedEventArgs e)
        {
            CQ doc = e.Result;
            CQ elements = doc[".countytr"];
            ChildrenShouldHas = elements.Length;
            TXT.WriteSQL("UPDATE public.placestable SET placeson = " + ChildrenShouldHas + " WHERE placefullname='" + GetFullName() + "' ;");
            foreach (var element in elements)
            {
                County county;
                if (element.FirstChild.FirstChild.HasAttribute("href"))
                {
                    #region converting url
                    string[] stringArray = URL.Split('/');
                    stringArray[stringArray.Length - 1] = element.FirstChild.FirstChild.Attributes.GetAttribute("href");
                    string newString = "";
                    for (var i = 0; i < stringArray.Length; i++)
                    {
                        newString += i == 0 ? stringArray[i] : '/' + stringArray[i];
                    }
                    #endregion
                    county = new County
                    {
                        Name = element.ChildNodes[1].FirstChild.FirstChild.ToString(),
                        Code = element.FirstChild.FirstChild.FirstChild.ToString(),
                        URL = newString,
                        Father = this,
                        Type = "County"
                    };
                }
                else
                {
                    county = new County
                    {
                        Name = element.ChildNodes[1].FirstChild.ToString(),
                        Code = element.FirstChild.FirstChild.ToString(),
                        Father = this,
                        Type = "County"
                    };
                }
                //Children.Add(county);
                AppendChildPlace(county);
                county.Start();
                //Console.WriteLine(county.GetFullName());
                base.OnDownloadSuccess(sender, e);

            }
        }
    }

    class County : Place
    {
        public override void OnDownloadSuccess(object sender, DownloadStringCompletedEventArgs e)
        {
            CQ doc = e.Result;
            CQ elements = doc[".towntr"];
            ChildrenShouldHas = elements.Length;
            TXT.WriteSQL("UPDATE public.placestable SET placeson = " + ChildrenShouldHas + " WHERE placefullname='" + GetFullName() + "' ;");
            foreach (var element in elements)
            {
                Town town;
                if (element.FirstChild.FirstChild.HasAttribute("href"))
                {
                    #region converting url
                    string[] stringArray = URL.Split('/');
                    stringArray[stringArray.Length - 1] = element.FirstChild.FirstChild.Attributes.GetAttribute("href");
                    string newString = "";
                    for (var i = 0; i < stringArray.Length; i++)
                    {
                        newString += i == 0 ? stringArray[i] : '/' + stringArray[i];
                    }
                    #endregion
                    town = new Town()
                    {
                        Name = element.ChildNodes[1].FirstChild.FirstChild.ToString(),
                        URL = newString,
                        Father = this,
                        Code = element.FirstChild.FirstChild.FirstChild.ToString(),
                        Type = "Town"
                    };
                }
                else
                {
                    town = new Town()
                    {
                        Name = element.ChildNodes[1].FirstChild.FirstChild.ToString(),
                        Father = this,
                        Code = element.FirstChild.FirstChild.FirstChild.ToString(),
                        Type = "Town"
                    };
                }
                //Children.Add(town);
                AppendChildPlace(town);


                town.Start();
                //Console.WriteLine(town.GetFullName());
                base.OnDownloadSuccess(sender, e);

            }
        }
    }

    class Town : Place
    {
        public override void OnDownloadSuccess(object sender, DownloadStringCompletedEventArgs e)
        {
            CQ doc = e.Result;
            CQ elements = doc[".villagetr"];
            ChildrenShouldHas = elements.Length;
            TXT.WriteSQL("UPDATE public.placestable SET placeson = " + ChildrenShouldHas + " WHERE placefullname='" + GetFullName() + "' ;");
            foreach (var element in elements)
            {
                Village village = new Village()
                {
                    Code = element.FirstChild.FirstChild.ToString(),
                    CXType = element.ChildNodes[1].FirstChild.ToString(),
                    Name = element.ChildNodes[2].FirstChild.ToString(),
                    Type = "Village",
                    Father = this
                };
                //Children.Add(village);
                AppendChildPlace(village);
                village.Start();

                //Console.WriteLine(village.GetFullName());
            }



            base.OnDownloadSuccess(sender, e);
        }
    }
    class Village : Place
    {
        //public Village()
        //{
        //    Traversed = true;
        //}

        public override void OnDownloadSuccess(object sender, DownloadStringCompletedEventArgs e)
        {
            base.OnDownloadSuccess(sender, e);
        }
    }

}
