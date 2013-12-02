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
        /// <summary>
        /// 按照时间（精确到毫秒）加GUID的格式生成唯一的ID（大概率唯一）
        /// </summary>
        /// <returns>生成的ID</returns>
        private static ID GenerateID()
        {
            DateTime dt = DateTime.Now;
            string id = dt.Year+"Y"+dt.Month+"M"+dt.Day+"D"+dt.Hour+"h"+dt.Minute+"m"+dt.Second+"s"+dt.Millisecond+"ms"+Guid.NewGuid().ToString("N");
            return id ;
        }
        /// <summary>
        /// 主要的执行过程，将输入的字节数组经过执行转化为输出的字节数组
        /// </summary>
        /// <param name="content">输入的字节数组</param>
        /// <returns>返回的字节数组</returns>
        public byte[] GetResult(byte[] content) {
            DateTime st = DateTime.Now;

            ID id = GenerateID();

            //将内容写在文件中
            WriteToFile(content, id);
            
            //将本线程信号添加到管理列表中
            ControlCenter.Instance.AddThreadSignal(id, receiveDone);
            
            //线程阻塞 等待信号被释放
            try
            {
                receiveDone.WaitOne(Configure.Instance.WaitMilliseconds);
            }
            catch (Exception e)
            {
                Info.Output(InfoLevel.LOG, id, e.Message);
            }
            
            //将本线程信号从管理列表中移除
            ControlCenter.Instance.RemoveThreadSignal(id);
            
            //从文件中读取内容
            byte[] result = ReadFromFile(id, Configure.Instance.WaitTime, Configure.Instance.SleepTime);
            
            //删除本线程产生的中间文件
            DeleteFile(id, Configure.Instance.DestinationPath);
            DeleteFile(id, Configure.Instance.SourcePath);

            //打印日志
            DateTime se = DateTime.Now;
            TimeSpan s = se - st;
            if (result == null) Info.Output(InfoLevel.LOG, id ," cost " + s.TotalMilliseconds + "ms  Request Time Out!");
            else    Info.Output(InfoLevel.LOG, id, " cost " + s.TotalMilliseconds + "ms  Request Success!");
            return result;
        }
        /// <summary>
        /// 删除给定的路径下指定的文件
        /// </summary>
        /// <param name="id">待删除的文件</param>
        /// <param name="path">给定的路径</param>
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
        /// <summary>
        /// 将给定的内容写入到指定的文件中
        /// </summary>
        /// <param name="content">给定的内容</param>
        /// <param name="id">指定的文件</param>
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
        /// 从指定的文件中读取内容，有一定的失败后循环尝试次数和每次失败后的暂停时间
        /// </summary>
        /// <param name="id">读取的文件名（线程的唯一标示）</param>
        /// <param name="times">打不开文件时的等待次数</param>
        /// <param name="sleepTime">打不开文件时的每次睡眠时间（millisecond)</param>
        /// <returns>返回读取内容，若文件不存在则为空</returns>
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
