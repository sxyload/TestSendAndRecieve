using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace TestSendAndRecieve
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int cases = 1000;
            for (int i = 0; i < cases; i++)
            {
                Thread t = new Thread(new Test().TestSample);
                t.Start();
            }
        }
    }
    class Test
    {
        private static Random rand = new Random();
        private static object Lock = new object();
        public static int success = 0;
        public Test()
        {
        }
        public void TestSample()
        {
            int limit = 9999;
            TaskProcedure tp = new TaskProcedure();
            string s = rand.Next(limit).ToString();
            byte[] b = UTF8Encoding.UTF8.GetBytes(s);
            byte[] result = tp.GetResult(b);
            if (result == null)
            {
                Console.WriteLine(s + " Fail");
            }
            else
            {
                Console.WriteLine(s+" "+UTF8Encoding.UTF8.GetString(result, 0, result.Length));
                if (s.Equals(UTF8Encoding.UTF8.GetString(result, 0, result.Length)))
                {
                    lock (Lock)
                    {
                        success++;
                    }
                }
                Console.WriteLine("total success "+success);
            }
            
        }
    }
}
