using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace UEditor.Net
{

    /// <summary>
    /// Crawler 的摘要说明
    /// </summary>
    public class CrawlerHandler : Handler
    {
        private string[] Sources;
        private List<Crawler> Crawlers;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public CrawlerHandler(HttpContext context) : base(context) { }

        /// <summary>
        /// 
        /// </summary>
        public async override Task Process()
        {
            Sources = Request.Form.GetValues("source[]");
            if (Sources == null || Sources.Length == 0)
            {
                WriteJson(new
                {
                    state = "参数错误：没有指定抓取源"
                });
                return;
            }
            Crawlers = new List<Crawler>();
            foreach (var item in Sources)
            {
                Crawlers.Add(await new Crawler(item, Server).Fetch());
            }
            WriteJson(new
            {
                state = "SUCCESS",
                list = Crawlers.Select(x => new
                {
                    state = x.State,
                    source = x.SourceUrl,
                    url = x.ServerUrl
                })
            });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Crawler
    {
        /// <summary>
        /// 
        /// </summary>
        public string SourceUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ServerUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string State { get; set; }

        private HttpServerUtility Server { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceUrl"></param>
        /// <param name="server"></param>
        public Crawler(string sourceUrl, HttpServerUtility server)
        {
            this.SourceUrl = sourceUrl;
            this.Server = server;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Crawler> Fetch()
        {
            var request = HttpWebRequest.Create(this.SourceUrl) as HttpWebRequest;
            using (var response =await request.GetResponseAsync() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    State = "Url returns " + response.StatusCode + ", " + response.StatusDescription;
                    return this;
                }
                if (response.ContentType.IndexOf("image") == -1)
                {
                    State = "Url is not an image";
                    return this;
                }
                ServerUrl = PathFormatter.Format(Path.GetFileName(this.SourceUrl), Config.GetString("catcherPathFormat"));
                var savePath = Server.MapPath(ServerUrl);
                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                }
                try
                {
                    var stream = response.GetResponseStream();
                    var reader = new BinaryReader(stream);
                    byte[] bytes;
                    using (var ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        int count;
                        while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            ms.Write(buffer, 0, count);
                        }
                        bytes = ms.ToArray();
                    }
                    //File.WriteAllBytes(savePath, bytes);
                    using (var fs = new FileStream(path: savePath, mode: FileMode.Create,
                                    access: FileAccess.Write,
                                    share: FileShare.None,
                                    bufferSize: 4096,
                                    useAsync: true))
                    {
                        await fs.WriteAsync(bytes, 0, bytes.Length);
                    }
                    State = "SUCCESS";
                }
                catch (Exception e)
                {
                    State = "抓取错误：" + e.Message;
                }
                return this;
            }
        }
    }
}