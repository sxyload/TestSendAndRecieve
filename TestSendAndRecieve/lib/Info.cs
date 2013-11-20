using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSendAndRecieve
{
    public class Info
    {
        public static InfoLevel Level = InfoLevel.LOG;
        public static void Output(InfoLevel level, string id, string content)
        {
            DateTime dt = DateTime.Now;
            if (level <= Level)
            {
                Console.WriteLine("["+level+"] "+dt.ToString("yyyy-MM-dd HH:mm:ss fff") + " " + "(" + id + ") " + content);
            }
        }
        public static void SetLevel(string s)
        {
            switch (s)
            {
                case "Log":
                    Info.Level = InfoLevel.LOG;
                    break;
                case "Debug":
                    Info.Level = InfoLevel.DEBUG;
                    break;
                default :
                    break;
            }
        }
    }
    public enum InfoLevel {LOG,DEBUG};
}
