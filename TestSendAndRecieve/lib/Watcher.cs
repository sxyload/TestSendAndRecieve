using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Threading;

namespace TestSendAndRecieve
{
    public class Watcher
    {
        [IODescriptionAttribute("FSW_ChangedFilter")]
        public NotifyFilters NotifyFilter { get; set; }

        //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static object LockWatcher = new object();
        private static Watcher m_Instance;
        public static Watcher Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (LockWatcher)
                    {
                        if (m_Instance == null)
                        {
                            m_Instance = new Watcher();
                        }
                    }
                }
                return m_Instance;
            }
        }
        private object LockMonitor = new object();
        private FileSystemWatcher m_Monitor;
        public FileSystemWatcher Monitor
        {
            get
            {
                if (m_Monitor == null)
                {
                    lock (LockMonitor)
                    {
                        if (m_Monitor == null)
                        {
                            m_Monitor = new FileSystemWatcher();
                        }
                    }
                }
                return m_Monitor;
            }
        }
        public Watcher()
        {
            SetConfigure();
            SetEvent();
        }
        public void SetConfigure()
        {
            // Create a new FileSystemWatcher and set its properties.
            Monitor.Path = Configure.Instance.DestinationPath;
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            Monitor.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            Monitor.Filter = "*";
            // Begin watching.
            Monitor.EnableRaisingEvents = true;
        }
        public void SetEvent()
        {
            // Add event handlers.
            Monitor.Changed += new FileSystemEventHandler(OnReleaseResetEvent);
            Monitor.Created += new FileSystemEventHandler(OnReleaseResetEvent);
            Monitor.Renamed += new RenamedEventHandler(OnReleaseResetEvent);
        }
        private object LockEvent = new object();
        // Define the event handlers. 
        /// <summary>
        /// 检测到文件变动后，释放对应的信号
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e">文件变动事件</param>
        private void OnReleaseResetEvent(object source, FileSystemEventArgs e)
        {
            // make sure that excute reset one by one
            lock (LockEvent)
            {
                // Specify what is done when a file is changed, created, or deleted.
                ManualResetEvent mre = ControlCenter.Instance.GetThreadSignal(e.Name);
                if (mre != null)
                {
                    Info.Output(InfoLevel.DEBUG, e.Name, e.Name + " unlock");
                    mre.Set();
                }
                else
                {
                    Info.Output(InfoLevel.LOG, e.Name, e.Name + " unlock FAIL");
                }
            }
        }
    }
}