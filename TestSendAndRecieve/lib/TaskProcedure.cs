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
        //线程间信号量
        private ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        private static Object lockCount = new Object();
        private static ID GenerateID()
        {
            DateTime dt = DateTime.Now;
            string id = dt.Year+"Y"+dt.Month+"M"+dt.Day+"D"+dt.Hour+"h"+dt.Minute+"m"+dt.Second+"s"+dt.Millisecond+"ms"+Guid.NewGuid().ToString("N");
            return id ;
        }
        public byte[] GetResult(byte[] content) {
            DateTime st = DateTime.Now;

            ID id = GenerateID();
            //do  write
            WriteToFile(content, id);
            //set manualsetEvent
            ControlCenter.Instance.AddThreadSignal(id, receiveDone);
            receiveDone.WaitOne(Configure.Instance.WaitMilliseconds);
            ControlCenter.Instance.RemoveThreadSignal(id);
            //do read
            byte[] result = ReadFromFile(id, Configure.Instance.WaitTime, Configure.Instance.SleepTime);
            //delete File 
            DeleteFile(id, Configure.Instance.DestinationPath);
            DeleteFile(id, Configure.Instance.SourcePath);

            DateTime se = DateTime.Now;
            TimeSpan s = se - st;
            if (result == null) Info.Output(InfoLevel.LOG, id ," cost " + s.TotalMilliseconds + "ms  Request Time Out!");
            else    Info.Output(InfoLevel.LOG, id, " cost " + s.TotalMilliseconds + "ms  Request Success!");
            return result;
        }
        private void DeleteFile(string id, string path)
        {
            try
            {
                File.Delete(path + id);
                Info.Output(InfoLevel.DEBUG, id, "File Delete Success");
            }
            catch (IOException e)
            {
                Info.Output(InfoLevel.LOG, id, "File Delete Fail "+e.Message);
            }
        }
        private void WriteToFile(byte[] content, string id)
        {
            string path = Configure.Instance.SourcePath + id;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(content, 0, content.Length);
                    fs.Close();
                }
            }
            catch (IOException e)
            {
               Info.Output(InfoLevel.LOG, id, " Write Request To File Fail");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">读取的文件名（线程的唯一标示）</param>
        /// <param name="times">打不开文件时的等待次数</param>
        /// <param name="sleepTime">打不开文件时的每次睡眠时间（millisecond)</param>
        /// <returns></returns>
        private byte[] ReadFromFile(string id, int times, int sleepTime)
        {
            string path = Configure.Instance.DestinationPath + id;
            byte[] result = null;

            if (File.Exists(path))
            {
                bool open = false;
                while (!open && (times-- >= 0))
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
                        Info.Output(InfoLevel.DEBUG, id, e.Message);
                        Thread.Sleep(sleepTime);
                    }
                }
                if (!open)
                {
                    Info.Output(InfoLevel.LOG, id, " Result File Open Fail");
                }
            }
            else
            {
                Info.Output(InfoLevel.LOG, id, " Result File Not Found");
            }
            return result;
        }
    }
}
