using System;
using System.Collections.Generic;
using System.IO;

namespace SP2
{
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

}
