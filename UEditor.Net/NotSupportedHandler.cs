using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace UEditor.Net
{
    /// <summary>
    /// NotSupportedHandler 的摘要说明
    /// </summary>
    public class NotSupportedHandler : Handler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public NotSupportedHandler(HttpContext context)
            : base(context)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async override Task Process()
        {
            await Task.Factory.StartNew(() =>
            {
                WriteJson(new
                {
                    state = "action 参数为空或者 action 不被支持。"
                });
            });

        }
    }
}