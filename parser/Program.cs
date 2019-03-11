using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace parser
{
    internal class Program
    {
        public static HttpClient Client;
        public static NameGenerator Generator;
        public static List<string> prevResult;

        private static void Main(string[] args)
        {
            Init();
            List<string> ids = new List<string>();
            int count = 0;

            while (true)
            {
                 var  id = GetUniqueId();
                 prevResult.Add(id);
                var delay = new Random().Next(2 * 1000, 10 * 1000);
                GetRandomImg(id);
                Console.WriteLine($"Img #{count++} With id: {id} completed next delay: {delay}");
                Thread.Sleep(delay);
           
            }



        }


        public static string GetUniqueId()
        {
            var id = Generator.Generate();
            if (prevResult.Contains(id))
                id = GetUniqueId(); 
            return id;
        }

        public static void GetRandomImg(string id)
        {
            var html = SendRequest(id);
            var url = ParseImage(html,id);
            DownloadImage(id, url);
        }

        public static string SendRequest(string id)
        {
            return
                Client.GetAsync(new Uri($@"https://prnt.sc/{id}"))
                    .Result
                    .Content
                    .ReadAsStringAsync()
                    .Result;

        }

        public static string ParseImage(string html,string id)
        {
            HtmlDocument document = new HtmlDocument();
            HtmlWeb web = new HtmlWeb();
            document.LoadHtml(html);
            try
            {
             return   document.DocumentNode.SelectSingleNode("//img[@class='no-click screenshot-image']")
                    .Attributes["src"].Value;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static void DownloadImage(string id,string url)
        {
            try
            {
                File.WriteAllBytes($@"imgs\{id}.png",
                    Client.GetAsync(new Uri(url))
                        .Result
                        .Content
                        .ReadAsByteArrayAsync()
                        .Result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
           
        }

        public static void Init()
        {
            InitClient();
            prevResult = new List<string>();
            Generator = new NameGenerator(4, "msjkbk");
        }

        public static void InitClient()
        {

            //"https://prnt.sc/aaa04z" -H "authority: prnt.sc" -H "pragma: no-cache" -H "cache-control: no-cache" -H "upgrade-insecure-requests: 1"
            //-H "user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36"
            //-H "accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8" -H "accept-encoding: gzip, deflate, br" -H "accept-language: ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7" -H "cookie: __cfduid=dc704cd5c5d553b6bb98aacc6c3c093b61552325986; _ga=GA1.2.1615548791.1552325988; _gid=GA1.2.1017565181.1552325988" --compressed

            Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("authority","prnt.sc");
            Client.DefaultRequestHeaders.Add("pragma", "no-cache");
            Client.DefaultRequestHeaders.Add("cache-control", "no-cache");
            Client.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            //Client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
            Client.DefaultRequestHeaders.Add("accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            Client.DefaultRequestHeaders.Add("cookie", "__cfduid=dc704cd5c5d553b6bb98aacc6c3c093b61552325986; _ga=GA1.2.1615548791.1552325988; _gid=GA1.2.1017565181.1552325988");
            Client.DefaultRequestHeaders
                .Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36");

        }
    }


    class NameGenerator
    {
        public int NumberOfSymbols { get; set; }
        public string Template { get; set; }
        public const int MaxNumberOfSymbols  = 6;

        public NameGenerator(int num,string template)
        {
            NumberOfSymbols = num;
            Template = template;
        }

        public string Generate()
        {
            var temp = Template.Substring(0,NumberOfSymbols);
            for (int i = NumberOfSymbols; i < MaxNumberOfSymbols; i++)
            {
                temp = temp + Generator();
                Thread.Sleep(50);

            }
            return temp;
        }

        public char Generator()
        {
            return Convert.ToChar(new Random().Next(0, 2) == 0 ? new Random().Next(48, 58) : new Random().Next(97, 123));
        }
    }
}
