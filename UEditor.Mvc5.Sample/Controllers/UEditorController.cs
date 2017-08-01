using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace UEditor.Mvc5.Sample
{
    /// <summary>
    /// 
    /// </summary>
    public class UEditorController : UEditor.WebApi.Controllers.UEditorBaseApiController
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
