﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace UEditor.Net
{
    /// <summary>
    /// Config 的摘要说明
    /// </summary>
    public class ConfigHandler : Handler
    {
        public ConfigHandler(HttpContext context) : base(context) { }

        public async override Task Process()
        {
            await Task.Factory.StartNew(()=>{
                WriteJson(Config.Items);
            });
        }
    }
}