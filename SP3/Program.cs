using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

namespace SP3
{
    class Program
    {


        public static void DoSomething(object sender, PageSuccessEventArgs e)
        {
            Console.WriteLine(e.ThisPlace + " Page Success!");
        }

        public static void DoSomethingAfterTraversed(object sender, TraversedEventArgs e)
        {
            Console.WriteLine(sender + " traversed");
        }


        public static void Main(string[] args)
        {


            NationPlace china = new NationPlace
            {
                Name = "中华人民共和国",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/",
                PlaceType = PlaceType.Nation,
                Traversed = false,
            };
            china.Start();


            china.OnPageSuccess += new PageSuccessDelegate(DoSomething);

            ProvincePlace hebei = new ProvincePlace
            {
                Name = "河北省",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13.html",
                PlaceType = PlaceType.Province,
                Traversed = false,
            };
            hebei.Start();
            hebei.OnPageSuccess += new PageSuccessDelegate(DoSomething);

            CityPlace shijiazhuang = new CityPlace
            {
                Name = "石家庄市",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13/1301.html",
                PlaceType = PlaceType.City,
                Traversed = false,
            };
            shijiazhuang.Start();
            shijiazhuang.OnPageSuccess += new PageSuccessDelegate(DoSomething);
            shijiazhuang.OnTraversed += new TraversedDelegate(DoSomethingAfterTraversed);

            TownPlace 建北街道办事处 = new TownPlace()
            {
                Name = "建北街道办事处",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13/01/02/130102001.html",
                PlaceType = PlaceType.Town,
                Traversed = false
            };
            建北街道办事处.Start();
            建北街道办事处.OnPageSuccess += new PageSuccessDelegate(DoSomething);
            建北街道办事处.OnTraversed += new TraversedDelegate(DoSomethingAfterTraversed);
            Thread.Sleep(2000);
            //Console.WriteLine(china.PlaceType.ToString());
            Console.ReadLine();
        }
    }

    static class DownloadQuene
    {
        static public Queue<Place> PlacesToClick { get; set; }
        static public void Start()
        {

        }
    }


}
