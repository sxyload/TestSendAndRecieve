using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ID = System.String;
namespace TestSendAndRecieve
{
    class TaskProcedure
    {
        //使用牛逼闪闪的线程间信号量
        private ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        private static Object lockCount = new Object();
        
        private static ID GenerateID()
        {
            return Guid.NewGuid().ToString("N");
        }
        public byte[] GetResult(byte[] content) { 
            ID id = GenerateID();
            //do  write
            WriteToFile(content, id);
            ControlCenter.Instance.AddThread(id, receiveDone);
            receiveDone.WaitOne(100000);
            byte[] result = ReadFromFile(id);
            //do read
            return result;
        }
        private void WriteToFile(byte[] content, string id)
        {
            string path = ControlCenter.SourcePath+id;
            using (StreamWriter sw = File.CreateText(path))
            {
               
                sw.WriteLine(content);
            }
        }
        private byte[] ReadFromFile(string id)
        {
            string path = ControlCenter.DestinationPath + id;
            byte[] result;
            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string s = "";
                    StringBuilder sb = new StringBuilder();
                    while ((s = sr.ReadLine()) != null)
                    {
                        sb.Append(s);
                    }
                    char[] c = new char[8192];
                    sb.CopyTo(0, c, 0, sb.Length);
                    result = UTF8Encoding.UTF8.GetBytes(c);
                }
                Console.WriteLine("success");
            }
            else
            {
                result = null;
                Console.WriteLine("fail");
            }
            return result;
      
        }

    }
}
