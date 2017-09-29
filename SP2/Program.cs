using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using CsQuery;

namespace SP2
{
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

            Province hebei = new Province()
            {
                Name = "河北省",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13.html",
                Type = "Province",
            };
            hebei.Start();

            City shijiazhuang = new City()
            {
                Name = "石家庄",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13/1301.html",
                Type = "City"
            };
            shijiazhuang.Start();

            County changanqu = new County()
            {
                Name = "长安区",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13/01/130102.html",
                Type = "County"
            };
            changanqu.Start();

            Town jianbeijiedaobanshichu = new Town()
            {
                Name = "建北街道办事处",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13/01/02/130102001.html",
                Type = "Town"
            };
            jianbeijiedaobanshichu.Start();
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

        public Place Father { get; set; }
        public List<Place> Children = new List<Place>();


        public string GetFullName() => Father != null ? Father.GetFullName() + Name : Name;

        public void Start()
        {
            Download();
        }

        public void Download()
        {
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri(URL));
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
            foreach (var element in elements)
            {
                Province province = new Province
                {
                    Father = this,
                    Name = element.FirstChild.ToString(),
                    URL = URL + element.Attributes.GetAttribute("href"),
                    Type = "Province"
                };
                Children.Add(province);
                Console.WriteLine(province.GetFullName());
            }
        }
    }

    class Province : Place
    {
        public override void OnDownloadSuccess(object sender, DownloadStringCompletedEventArgs e)
        {
            CQ doc = e.Result;
            CQ elements = doc[".citytr"];
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
                Children.Add(city);
                Console.WriteLine(city.GetFullName());
                //URL = element.FirstChild.FirstChild.Attributes.GetAttribute("href");
            }
        }
    }

    class City : Place
    {
        public override void OnDownloadSuccess(object sender, DownloadStringCompletedEventArgs e)
        {
            base.OnDownloadSuccess(sender, e);
            CQ doc = e.Result;
            CQ elements = doc[".countytr"];
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
                Children.Add(county);
                Console.WriteLine(county.GetFullName());
            }
        }
    }

    class County : Place
    {
        public override void OnDownloadSuccess(object sender, DownloadStringCompletedEventArgs e)
        {
            base.OnDownloadSuccess(sender, e);
            CQ doc = e.Result;
            CQ elements = doc[".towntr"];
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
                Children.Add(town);
                Console.WriteLine(town.GetFullName());
            }
        }
    }

    class Town : Place
    {
        public override void OnDownloadSuccess(object sender, DownloadStringCompletedEventArgs e)
        {
            base.OnDownloadSuccess(sender, e);
            CQ doc = e.Result;
            CQ elements = doc[".villagetr"];
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
                Children.Add(village);
                Console.WriteLine(village.GetFullName());
            }
        }
    }
    class Village : Place
    {
        public string CXType { get; set; }
        public override void OnDownloadSuccess(object sender, DownloadStringCompletedEventArgs e)
        {
            base.OnDownloadSuccess(sender, e);
        }
    }

}
