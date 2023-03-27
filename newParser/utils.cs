using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace newParser
{
    class Utils
    {
        public static string getHTML(string query) {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/111.0");
            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            client.Headers.Add("Accept-Encoding", "br");
            client.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");

            string baseUrl = "https://www.amazon.com/s?k=" + query;
            int currentPage = 1;
            string url = baseUrl + "&page=" + currentPage;
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            return reader.ReadToEnd();
        }

        public static DataTable getData(string html, int amount = 10) {
            Regex r = new Regex(@"<span.*a-size-base-plus a-color-base a-text-normal[^>]*>(.*?)</span>.*(?:<a.*a-link-normal s-underline-text s-underline-link-text s-link-style[^>]*>|<span.*a-size-base[^>]*>|<span>)([^<]*)(?:</a>|</span>).*<span.*a-size-base[^>]*>(\d.\d)</span>.*([$]\d+\.\d+)");
            int gotCount = 0;

            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Author", typeof(string));
            table.Columns.Add("Rating", typeof(string));
            table.Columns.Add("Price", typeof(string));
            table.Columns.Add("Date", typeof(string));

            foreach (Match m in r.Matches(html))
            {
                gotCount++;
                DataRow row;
            
                row = table.NewRow();
                row[0] = m.Groups[1];
                row[1] = m.Groups[2];
                row[2] = m.Groups[3];
                row[3] = m.Groups[4];
                row[4] = m.Groups[5];
                table.Rows.Add(row);

                if (gotCount >= amount) { return table; }
            }
            return table;
        }
    }
}
