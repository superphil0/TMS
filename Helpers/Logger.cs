using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TMS
{
    public class Logger
    {
        public static void Exception(Exception e)
        {
            try
            {
                StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\log.txt", true);
                sw.Write("------------" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "-----------");
                sw.WriteLine("MESSAGE: " + e.Message);
                sw.WriteLine("STACK TRACE: " + e.StackTrace);
                sw.Close();
            }
            catch (Exception)
            {
                //JBG :)
            }
        }
    }
}
