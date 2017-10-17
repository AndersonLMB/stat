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
        static void Main(string[] args)
        {


            NationPlace china = new NationPlace
            {
                Name = "中华人民共和国",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/",
                //Type = "NationPlace",
                //PlaceType = PlaceType.NationPlace,
                Traversed = false,
            };
            //Task task = china.ClickInAsync();
            //Console.WriteLine(task.AsyncState);
            china.Start();
            ProvincePlace hebei = new ProvincePlace
            {
                Name = "河北省",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13.html",
                //Type = "NationPlace",
                PlaceType = PlaceType.Province,
                Traversed = false,
            };
            hebei.Start();
            CityPlace shijiazhuang = new CityPlace
            {
                Name = "石家庄市",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13/1301.html",
                //Type = "NationPlace",
                PlaceType = PlaceType.City,
                Traversed = false,
            };
            shijiazhuang.Start();

            Thread.Sleep(2000);
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
