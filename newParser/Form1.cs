﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace newParser
{
    public partial class Form1 : Form
    {

        private ListViewColumnSorter lvwColumnSorter;
        public Form1()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(
                    "Вы не ввели название книги!",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            this.button1.Enabled = false;

            string query = textBox1.Text;
            DataTable table = await Utils.getData(query);

            foreach (DataRow row in table.Rows)
            {
                ListViewItem item = new ListViewItem(row[0].ToString());
                for (int i = 1; i < table.Columns.Count - 1; i++)
                {
                    item.SubItems.Add(row[i].ToString());
                }
                item.Tag = row[4].ToString();
                listView1.Items.Add(item);
            }

            this.button1.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ColumnHeader columnheader;// Used for creating column headers.
            ListViewItem listviewitem;// Used for creating listview items.

            // Ensure that the view is set to show details.
            listView1.View = View.Details;

            // Create some column headers for the data.
            columnheader = new ColumnHeader();
            columnheader.Text = "Название книги";
            columnheader.Width = 600;
            this.listView1.Columns.Add(columnheader);

            columnheader = new ColumnHeader();
            columnheader.Text = "Автор";
            columnheader.Width = 300;
            this.listView1.Columns.Add(columnheader);

            columnheader = new ColumnHeader();
            columnheader.Text = "Оценка";
            columnheader.Width = 100;
            this.listView1.Columns.Add(columnheader);

            columnheader = new ColumnHeader();
            columnheader.Tag = "Float";
            columnheader.Text = "Цена, $";
            columnheader.Width = 200;
            this.listView1.Columns.Add(columnheader);

            columnheader = new ColumnHeader();
            columnheader.Text = "Ссылка";
            columnheader.Width = 1;
            this.listView1.Columns.Add(columnheader);
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                var rectangle = listView1.GetItemRect(i);
                if (rectangle.Contains(e.Location))
                {
                    String link = listView1.Items[i].Tag.ToString();
                    try
                    {
                        Process.Start(link);
                    }
                    catch
                    {
                        // hack because of this: https://github.com/dotnet/corefx/issues/10361
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            link = link.Replace("&", "^&");
                            Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            Process.Start("xdg-open", link);
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            Process.Start("open", link);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return;
                }
            }
        }
    }
}
