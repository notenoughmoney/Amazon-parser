using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace newParser
{
    class Utils
    {
        private static async Task<string> getHTML(string query, int currentPage = 1) {
            WebClient client = new();
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/111.0");
            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            client.Headers.Add("Accept-Encoding", "br");
            client.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");

            string baseUrl = "https://www.amazon.com/s?k=" + query;
            string url = baseUrl + "&page=" + currentPage;
            Stream data = await client.OpenReadTaskAsync(url);
            StreamReader reader = new(data);
            return await reader.ReadToEndAsync();
        }

        public static async Task<DataTable> getData(string query, int amount = 10) {
            Regex r = new(@"<a.*a-link-normal s-underline-text s-underline-link-text s-link-style a-text-normal.*href=[^\s](.*)""><span.*a-size-base-plus a-color-base a-text-normal[^>]*>(.*?)</span>.*(?:<a.*a-link-normal s-underline-text s-underline-link-text s-link-style[^>]*>|<span.*a-size-base[^>]*>|<span>)([^<]*)(?:</a>|</span>).*<span.*a-size-base[^>]*>(\d.\d)</span>.*([$]\d+\.\d+)");
            int gotCount = 0;
            int currentPage = 1;

            DataTable table = new();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Author", typeof(string));
            table.Columns.Add("Rating", typeof(string));
            table.Columns.Add("Price", typeof(float));

            while (gotCount < amount)
            {
                string html = await getHTML(query, currentPage);
                
                foreach (Match m in r.Matches(html))
                {
                    gotCount++;
                    DataRow row;

                    string bookUrl = "https://www.amazon.com" + m.Groups[1];

                    row = table.NewRow();

                    row[0] = m.Groups[2];
                    row[1] = m.Groups[3];
                    row[2] = m.Groups[4];
                    row[3] = m.Groups[5]
                        .ToString()
                        .Replace(".", ",")
                        .Substring(1);
                    table.Rows.Add(row);

                    if (gotCount >= amount) { return table; }
                }

                currentPage++;
            }

            return table;
        }
    }
}
