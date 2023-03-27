using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace newParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string html = Utils.getHTML(textBox1.Text);
            DataTable table = Utils.getData(html);

            foreach (DataRow row in table.Rows)
            {
                ListViewItem item = new ListViewItem(row[0].ToString());
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    item.SubItems.Add(row[i].ToString());
                }
                listView1.Items.Add(item);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.Columns.Add("Название книги", 500);
            listView1.Columns.Add("Автор", 200);
            listView1.Columns.Add("Оценка", 100);
            listView1.Columns.Add("Дата выхода", 200);
            listView1.Columns.Add("Цена", 200);
        }
    }
}
