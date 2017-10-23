using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using Npgsql;
using Npgsql.Logging;
using NpgsqlTypes;
using Npgsql.PostgresTypes;
using Dapper;
using System.Configuration;
using System.Data.Common;
using System.Data;



namespace SP3
{
    class Program
    {
        private static Object thisLock = new Object();

        public static void DoSomethingAfterPageSuccess(object sender, PageSuccessEventArgs e)
        {
            lock (thisLock)
            {
                #region UI
                Console.Write(e.ThisPlace.Code + " " + e.ThisPlace + " : ");
                e.ThisChildrenPlace.ForEach(i => Console.Write("{0} ", i.Name));
                Console.Write("\n");
                #endregion
            }
            e.ThisChildrenPlace.ForEach(child =>
            {
                Thread.Sleep(300);
                child.OnPageSuccess += new PageSuccessDelegate(DoSomethingAfterPageSuccess);
                child.OnTraversed += new TraversedDelegate(DoSomethingAfterTraversed);
                child.OnTraversedAdded += new TraversedAddedDelegate(DoSomethingAfterTraversedAdded);
                child.Start();

            });
        }

        public static void DoSomethingAfterTraversed(object sender, TraversedEventArgs e)
        {
            lock (thisLock)
            {
                #region UI
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(e.ThisPlace.Code + " " + sender + " traversed");
                Console.ResetColor();
                #endregion
            }
        }

        public static void DoSomethingAfterTraversedAdded(object sender, TraversedAddedEventArgs e)
        {
            lock (thisLock)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(e.AddedPlace.Code + " " + e.AddedPlace + " + , " + e.ThisPlace + " ({0}/{1}) ", e.ThisPlace.ChildrenCurrentTraversed, e.ThisPlace.ChildrenShouldHas);
                if (e.ThisPlace.ChildrenCurrentTraversed > e.ThisPlace.ChildrenShouldHas)
                {

                }
                Console.ResetColor();
            }
        }

        public static void Main(string[] args)
        {

            //------
            NationPlace china = new NationPlace
            {
                Name = "中华人民共和国",
                URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/",
                PlaceType = PlaceType.Nation,
                Traversed = false,
                Code = "000000000000"
            };
            china.OnPageSuccess += new PageSuccessDelegate(DoSomethingAfterPageSuccess);
            china.OnTraversed += new TraversedDelegate(DoSomethingAfterTraversed);
            china.OnTraversedAdded += new TraversedAddedDelegate(DoSomethingAfterTraversedAdded);
            china.Start();
            //------

            //var connection = new Npgsql.NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["places"].ConnectionString);
            //connection.Open();
            //var table = connection.Query<PlaceDBModel>("SELECT * FROM public.places");
            //INSERT INTO public.places(    placecode, placename, placetype, placeson, placesontraversed, placetraversed, placepagesuccess, placecxtype, placeurl, placefather)    VALUES('111', 'ddd', 'sss', 2, 1, false, false, '11', 'sd', 'sdefx');
            //IDbCommand command = connection.CreateCommand();
            //string sql = "INSERT INTO public.places(placecode, placename, placetype, placeson, placesontraversed, placetraversed, placepagesuccess, placecxtype, placeurl, placefather) " +
            //    "VALUES(@placecode,@place   )";

            //string query= "INSERT INTO "
            //NpgsqlCommand command = new NpgsqlCommand();
            //command.Parameters.Add()

            ;

            //ProvincePlace hebei = new ProvincePlace
            //{
            //    Name = "河北省",
            //    URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13.html",
            //    PlaceType = PlaceType.Province,
            //    Traversed = false,
            //};
            //hebei.Start();
            //hebei.OnPageSuccess += new PageSuccessDelegate(DoSomethingAfterPageSuccess);

            //CityPlace shijiazhuang = new CityPlace
            //{
            //    Name = "石家庄市",
            //    URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13/1301.html",
            //    PlaceType = PlaceType.City,
            //    Traversed = false,
            //};
            //shijiazhuang.Start();
            //shijiazhuang.OnPageSuccess += new PageSuccessDelegate(DoSomethingAfterPageSuccess);
            //shijiazhuang.OnTraversed += new TraversedDelegate(DoSomethingAfterTraversed);

            //TownPlace 建北街道办事处 = new TownPlace()
            //{
            //    Name = "建北街道办事处",
            //    URL = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/13/01/02/130102001.html",
            //    PlaceType = PlaceType.Town,
            //    Traversed = false
            //};
            //建北街道办事处.Start();
            //建北街道办事处.OnPageSuccess += new PageSuccessDelegate(DoSomethingAfterPageSuccess);
            //建北街道办事处.OnTraversed += new TraversedDelegate(DoSomethingAfterTraversed);
            Thread.Sleep(2000);
            //Console.WriteLine(china.PlaceType.ToString());
            Console.ReadLine();
        }
    }

    static class DownloadQuene
    {
        static public Queue<Place> PlacesToClick = new Queue<Place>();
        static public void Start()
        {

        }
    }


}
