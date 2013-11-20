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
        private ConcurrentDictionary<string, ManualResetEvent> m_WaitingThreadSignalPool;
        public ConcurrentDictionary<string, ManualResetEvent> WaitingThreadSignalPool
        {
            get
            {
                if (m_WaitingThreadSignalPool == null)
                {
                    lock (m_WaitingThreadPoolLock)
                    {
                        if (m_WaitingThreadSignalPool == null)
                        {
                            m_WaitingThreadSignalPool = new ConcurrentDictionary<string, ManualResetEvent>();
                        }
                    }
                }
                return m_WaitingThreadSignalPool;
            }
        }
        private Watcher Watch;
        public ControlCenter()
        {//默认的构造函数
            Info.Output(InfoLevel.LOG, "ControlCenter Instance", "Create ControlCenter");
            Watch = Watcher.Instance;
        }
        

        public bool AddThreadSignal(string id, ManualResetEvent mre)
        {
            lock (WaitingThreadSignalPool)
            {
                bool result = WaitingThreadSignalPool.TryAdd(id, mre);
                if (!result)
                {
                    Info.Output(InfoLevel.LOG, id, " thread add fail!");
                }
                else
                {
                    Info.Output(InfoLevel.DEBUG, id, " thread add success");
                }
                return result;
            }
        }
        
        public bool RemoveThreadSignal(string id)
        {
            lock (WaitingThreadSignalPool)
            {
                ManualResetEvent mre = null;
                bool result = WaitingThreadSignalPool.TryRemove(id, out mre);
                if (!result)
                {
                    Info.Output(InfoLevel.LOG, id, " thread remove fail!");
                }
                else
                {
                    Info.Output(InfoLevel.DEBUG, id, " thread remove success");
                }
                return result;
            }
        }

        public ManualResetEvent GetThreadSignal(string id)
        {
            lock (WaitingThreadSignalPool)
            {
                ManualResetEvent mre = null;
                bool result = WaitingThreadSignalPool.TryGetValue(id, out mre);
                if (!result)
                {
                    Info.Output(InfoLevel.LOG, id, " thread get fail!");
                }
                else
                {
                    Info.Output(InfoLevel.DEBUG, id, " thread get success");
                }
                return mre;
            }
        }
    }
}
