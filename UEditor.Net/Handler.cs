using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;


namespace UEditor.Net
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public abstract class Handler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public Handler(HttpContext context)
        {
            this.Request = context.Request;
            this.Response = context.Response;
            this.Context = context;
            this.Server = context.Server;
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract void Process();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        protected void WriteJson(object response)
        {
            string jsonpCallback = Request["callback"],
                json = JsonConvert.SerializeObject(response);
            if (String.IsNullOrWhiteSpace(jsonpCallback))
            {
                Response.AddHeader("Content-Type", "text/plain");
                Response.Write(json);
            }
            else
            {
                Response.AddHeader("Content-Type", "application/javascript");
                Response.Write(String.Format("{0}({1});", jsonpCallback, json));
            }
            Response.End();
        }
        /// <summary>
        /// 
        /// </summary>
        public HttpRequest Request { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public HttpResponse Response { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public HttpContext Context { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public HttpServerUtility Server { get; private set; }
    }
}