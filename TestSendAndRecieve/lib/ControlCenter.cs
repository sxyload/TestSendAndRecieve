using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Schema;


namespace TestSendAndRecieve
{
    class ControlCenter
    {
        

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
                            m_Instance = new ControlCenter();
                        }
                    }
                }
                return m_Instance;
            }
        }
        private object m_WaitingThreadPoolLock = new object();
        private ConcurrentDictionary<string, ManualResetEvent> m_WaitingThreadPool;
        public ConcurrentDictionary<string, ManualResetEvent> WaitingThreadPool
        {
            get
            {
                if (m_WaitingThreadPool == null)
                {
                    lock (m_WaitingThreadPoolLock)
                    {
                        if (m_WaitingThreadPool == null)
                        {
                            m_WaitingThreadPool = new ConcurrentDictionary<string, ManualResetEvent>();
                        }
                    }
                }
                return m_WaitingThreadPool;
            }
        }
        private Watcher Watch;
        public ControlCenter()
        {//默认的构造函数
            Console.WriteLine("Create ControlCenter");
            Watch = Watcher.Instance;
        }
        

        public void AddThread(string id, ManualResetEvent mre)
        {
            WaitingThreadPool.TryAdd(id, mre);
        }
        
        public bool RemoveThread(string id)
        {
            ManualResetEvent mre = null;
            bool result = WaitingThreadPool.TryRemove(id, out mre);
            return result;
        }

        public ManualResetEvent GetThread(string id)
        {
            ManualResetEvent mre = null;
            WaitingThreadPool.TryGetValue(id, out mre);
            return mre;
        }
    }
}
