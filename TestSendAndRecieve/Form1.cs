using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            TaskProcedure tp = new TaskProcedure();
            string s = textBox1.Text;
            byte[] b = UTF8Encoding.UTF8.GetBytes(s);
            byte[] result = tp.GetResult(b);
            if (result == null)
            {
                Console.WriteLine("fail");
            }
            else
            {
                Console.WriteLine(UTF8Encoding.UTF8.GetString(result, 0, result.Length));
            }
        }
    }
}
