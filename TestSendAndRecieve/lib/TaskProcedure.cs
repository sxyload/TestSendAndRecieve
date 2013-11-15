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
        public byte[] GetResult(byte[] content, int waitMiliSeconds) { 
            ID id = GenerateID();
            //do  write
            WriteToFile(content, id);
            ControlCenter.Instance.AddThread(id, receiveDone);
            DateTime st = DateTime.Now;
            receiveDone.WaitOne();
            DateTime se = DateTime.Now;
            TimeSpan s = se - st;
            Console.WriteLine(s.Milliseconds);
            byte[] result = ReadFromFile(id);
            //do read
            return result;
        }
        private void WriteToFile(byte[] content, string id)
        {
            string path = ControlCenter.SourcePath + id;
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(content, 0, content.Length);
                fs.Close();
            }
        }
        private byte[] ReadFromFile(string id)
        {
            string path = ControlCenter.DestinationPath + id;
            byte[] result = null;

            if (File.Exists(path))
            {
                Console.WriteLine(path);
                bool open = false;
                while (!open)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            result = new byte[fs.Length];
                            int numBytesToRead = (int)fs.Length;
                            int numBytesRead = 0;
                            while (numBytesToRead > 0)
                            {
                                int n = fs.Read(result, numBytesRead, numBytesToRead);
                                if (n == 0) break;
                                numBytesToRead -= n;
                                numBytesRead += n;
                            }
                            fs.Close();
                            open = true;
                        }
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                        Thread.Sleep(10);
                    }
                }
            }
            return result;
      
        }

    }
}
