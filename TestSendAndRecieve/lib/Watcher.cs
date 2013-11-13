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

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run()
        {

            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = @"E:/ProgramData/visual studio 2010/Projects/TestSendAndRecieve/destinationDir/";
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
            while (true) { Thread.Sleep(10000); Console.WriteLine("goon"); };

        }

        // Define the event handlers. 
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.Name + " " + e.ChangeType);
            ManualResetEvent mre = ControlCenter.Instance.RemoveThread(e.Name);
            lock (mre) { if (mre != null) mre.Set(); }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: " + e.OldFullPath + " renamed to " + e.FullPath);
        }
    }
}