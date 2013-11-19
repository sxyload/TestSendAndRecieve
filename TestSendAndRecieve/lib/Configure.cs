using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace TestSendAndRecieve
{
    public class Configure
    {
        public string SourcePath;
        public string DestinationPath;
        public int WaitMilliseconds;
        public int WaitTime;
        public int SleepTime;
        public int ConcurrentThreadLimit;
        public int CurrentThread;
        private static object LockInstance = new object();
        private static Configure m_Instance;
        public static Configure Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (LockInstance)
                    {
                        if (m_Instance == null)
                        {
                            m_Instance = new Configure();
                        }
                    }
                }
                return m_Instance;
            }
        }
        public Configure()
        {
            ReadConfigure();
        }
        private void ReadConfigure()
        {
            XmlTextReader xtr = null;
            Console.WriteLine("read configure");
            try
            {
                xtr = new XmlTextReader("Configure.xml");
                string name = "";
                while (xtr.Read())
                {
                    switch (xtr.NodeType)
                    {
                        case XmlNodeType.Element:
                            name = xtr.Name;
                            break;
                        case XmlNodeType.Text:
                            break;
                        case XmlNodeType.XmlDeclaration:
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            break;
                        case XmlNodeType.Comment:
                            break;
                        case XmlNodeType.EndElement:
                            break;
                        case XmlNodeType.CDATA:
                            switch (name)
                            {
                                case "SourcePath":
                                    SourcePath = xtr.Value;
                                    break;
                                case "DestinationPath":
                                    DestinationPath = xtr.Value;
                                    break;
                                case "WaitMilliseconds":
                                    //WaitMilliseconds = 1000;
                                    WaitMilliseconds = Int32.Parse(xtr.Value);
                                    break;
                                case "WaitTime":
                                    //WaitTime = 100;
                                    WaitTime = Int32.Parse(xtr.Value);
                                    break;
                                case "SleepTime":
                                    //SleepTime = 10;
                                    SleepTime = Int32.Parse(xtr.Value);
                                    break;
                                case "ConcurrentThreadLimit":
                                    //ConcurrentThreadLimit = 1000;
                                    ConcurrentThreadLimit = Int32.Parse(xtr.Value);
                                    break;
                                case "CurrentThread":
                                    //CurrentThread = 0;
                                    CurrentThread = Int32.Parse(xtr.Value);
                                    break;
                                default://
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
