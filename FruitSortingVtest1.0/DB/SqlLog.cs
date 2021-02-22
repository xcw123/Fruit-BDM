using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FruitSortingVtest1.DB
{
    public class SqlLog
    {
        public SqlLog()
        {
            if (!Directory.Exists("ErrorLog"))
                Directory.CreateDirectory("ErrorLog");
        }

        public void WriteSqlLog(string str)
        {
            try
            {
                string date = DateTime.Now.ToString("yyyyMMdd");
                StreamWriter log = new StreamWriter("ErrorLog\\SqlLog_" + date + ".txt", true);
                log.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "   " + str);
                log.Close();
            }
            catch (Exception ex) { }
        }

        public void WriteHttpLog(string str)
        {
            try
            {
                string date = DateTime.Now.ToString("yyyyMMdd");
                StreamWriter log = new StreamWriter("ErrorLog\\HttpLog_" + date + ".txt", true);
                log.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "   " + str);
                log.Close();
            }
            catch (Exception ex) { }
        }
    }
}
