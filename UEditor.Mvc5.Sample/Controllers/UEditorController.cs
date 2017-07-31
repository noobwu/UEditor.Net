using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UEditor.Mvc5.Sample.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class UEditorController : WebApi.Controllers.UEditorBaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        public UEditorController()
        {
            InitUploadDir("Uploads", "~/App_Data/Configs/UEditorConfig.json");
        }
    }
}
