using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestSendAndRecieve
{
    class ControlCenter
    {
        public static string SourcePath = @"sourceDir\";
        public static string DestinationPath = @"destinationDir\";
        private static object m_ControlLock = new object();
        private static ControlCenter m_Instance;
        public static ControlCenter Instance
        {
            get
            {
                /**
                 * 下面采用的判断方法是
                 * 1:先判断是否为空
                 * 2:一般情况不为空   为提高效率  此处不同步
                 *   若为空  则同步上锁  并且再次检测是否为空（防止其他线程已经生成实例)
                 * 3:生成实例
                 */ 
                if (m_Instance == null)
                {
                    lock (m_ControlLock)
                    {
                        if (m_Instance == null)
                        {
                            Console.WriteLine("Create ControlCenter");
                            m_Instance = new ControlCenter();
                        }
                    }
                }
                return m_Instance;
            }
        }
        private object m_WaitingThreadPoolLock = new object();
        private Dictionary<string, ManualResetEvent> m_WaitingThreadPool;
        public Dictionary<string, ManualResetEvent> WaitingThreadPool
        {
            get
            {
                if (m_WaitingThreadPool == null)
                {
                    lock (m_WaitingThreadPoolLock)
                    {
                        if (m_WaitingThreadPool == null)
                        {
                            m_WaitingThreadPool = new Dictionary<string, ManualResetEvent>();
                        }
                    }
                }
                return m_WaitingThreadPool;
            }
        }
        private Watcher Watch;
        public ControlCenter()
        {//默认的构造函数
            Watch = Watcher.Instance;
            Thread t = new Thread(Watch.Run);
            t.Start();
        }
        public ControlCenter(string sourceDir, string destinationDir)
        {
        }
        /// <summary>
        /// 同步地向表中添加信号量
        /// </summary>
        /// <param name="id">线程唯一id号</param>
        /// <param name="mre">线程锁</param>
        public void AddThread(string id, ManualResetEvent mre)
        {
            lock (m_WaitingThreadPoolLock)
            {
                WaitingThreadPool.Add(id, mre);
            }
        }
        /// <summary>
        /// 同步地从表中删除对应信号量 并且返回对应的信号量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ManualResetEvent RemoveThread(string id)
        {
            ManualResetEvent mre = null; 
            lock (m_WaitingThreadPoolLock)
            {
                if (WaitingThreadPool.ContainsKey(id))
                {
                    mre = WaitingThreadPool[id];
                    WaitingThreadPool.Remove(id);
                }
            }
            return mre;
        }
    }
}
