using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace SP3
{
    class Program
    {
        static void Main(string[] args)
        {



            Place china = new Place
            {
                Name = "中华人民共和国",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/",
                Type = "Nation",
            };
            china.Start();
            Console.ReadLine();
        }
    }

    class DownloadQuene
    {
        public Queue<Place> PlacesToClick { get; set; }
        public void Start()
        {

        }
    }


}
