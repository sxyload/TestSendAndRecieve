using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSendAndRecieve
{
    public class Info
    {
        public static InfoLevel Level = InfoLevel.LOG;
        /// <summary>
        /// 将日志输出（如果输出的级别高于当前日志的级别）
        /// </summary>
        /// <param name="level">输出的级别</param>
        /// <param name="id">任务ID号</param>
        /// <param name="content">输出内容</param>
        public static void Output(InfoLevel level, string id, string content)
        {
            DateTime dt = DateTime.Now;
            if (level <= Level)
            {
                Console.WriteLine("["+level+"] "+dt.ToString("yyyy-MM-dd HH:mm:ss fff") + " " + "(" + id + ") " + content);
            }
        }
        /// <summary>
        /// 设置日志打印级别，低于此级别的均被打印
        /// DEBUG为最高级
        /// LOG次之
        /// </summary>
        /// <param name="s">日志打印级别</param>
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
