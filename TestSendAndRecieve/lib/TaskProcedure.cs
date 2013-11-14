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
        private AutoResetEvent receiveDone =
            new AutoResetEvent(false);
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
            receiveDone.WaitOne(waitMiliSeconds);
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
            }
        }
        private byte[] ReadFromFile(string id)
        {
            string path = ControlCenter.DestinationPath + id;
            byte[] result;

            if (File.Exists(path))
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
                }
            }
            else
            {
                result = null;
            }
            return result;
      
        }

    }
}
