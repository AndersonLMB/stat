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


            Nation china = new Nation
            {
                Name = "中华人民共和国",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/",
                //Type = "Nation",
                PlaceType = PlaceType.Nation,
                Traversed = false,
            };
            //Task task = china.ClickInAsync();
            //Console.WriteLine(task.AsyncState);
            china.Start();
            Province hebei = new Province
            {
                Name = "河北省",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13.html",
                //Type = "Nation",
                PlaceType = PlaceType.Province,
                Traversed = false,
            };
            hebei.Start();
            City shijiazhuang = new City
            {
                Name = "石家庄市",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13/1301.html",
                //Type = "Nation",
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
