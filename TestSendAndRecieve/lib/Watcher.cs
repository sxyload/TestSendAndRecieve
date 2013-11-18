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
                            // Create a new FileSystemWatcher and set its properties.
                            m_Monitor.Path = @"E:\Program Data\Visual Studio 2010\Projects\Git\TestSendAndRecieve\TestSendAndRecieve\bin\Debug\destinationDir\";
                                /* Watch for changes in LastAccess and LastWrite times, and
                                   the renaming of files or directories. */
                            m_Monitor.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                            // Only watch text files.
                            m_Monitor.Filter = "*";
                            // Begin watching.
                            m_Monitor.EnableRaisingEvents = true;
                        }
                    }
                }
                return m_Monitor;
            }
        }
        public Watcher()
        {
            Console.WriteLine("Create a new Watcher instance");
        }
        public void Run()
        {
            // Add event handlers.
            Monitor.Changed += new FileSystemEventHandler(OnChanged);
            Monitor.Created += new FileSystemEventHandler(OnChanged);
            Monitor.Renamed += new RenamedEventHandler(OnRenamed);
        }

        // Define the event handlers. 
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.Name + " " + e.ChangeType);
            ManualResetEvent mre = ControlCenter.Instance.RemoveThread(e.Name);
            if (mre != null)
            {
                Console.WriteLine(e.Name + "unlock");
                mre.Set();
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: " + e.OldFullPath + " renamed to " + e.FullPath);
        }
    }
}