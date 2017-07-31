using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using UEditor.Net;

namespace UEditor.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class UEditorBaseApiController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        protected  string _uploadDir = "Uploads";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadDir"></param>
        public UEditorBaseApiController()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadDir"></param>
        protected void InitUploadDir(string uploadDir= "Uploads",string configPath= "~/App_Data/Configs/UEditorConfig.json")
        {
            if (string.IsNullOrEmpty(uploadDir)) throw new ArgumentNullException("uploadDir  is  null");
            _uploadDir = uploadDir;
            Config.InitConfig(configPath);
        }
        // GET: UEditor
        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [HttpPost]
        public void Index()
        {
            if (string.IsNullOrEmpty(_uploadDir))
                throw new ArgumentNullException("uploadDir  is  null");
            if (string.IsNullOrEmpty(Config.ConfigPath))
                throw new ArgumentNullException("uploadDir  is  null");
            if (!Path.IsPathRooted(Config.ConfigPath))
            {
                throw new ArgumentNullException("configPath   Invalid");
            }
            Handler actionHanler = null;
            var context = System.Web.HttpContext.Current;
            string action = context.Request["action"];
            switch (action)
            {
                case "config":
                    actionHanler = new ConfigHandler(context);
                    break;
                case "uploadimage":
                    actionHanler = new UploadHandler(context, new UploadConfig()
                    {
                        UploadDir=_uploadDir,
                        AllowExtensions = Config.GetStringList("imageAllowFiles"),
                        PathFormat = Config.GetString("imagePathFormat"),
                        SizeLimit = Config.GetInt("imageMaxSize"),
                        UploadFieldName = Config.GetString("imageFieldName")
                    });
                    break;
                case "uploadscrawl":
                    actionHanler = new UploadHandler(context, new UploadConfig()
                    {
                        UploadDir = _uploadDir,
                        AllowExtensions = new string[] { ".png" },
                        PathFormat = Config.GetString("scrawlPathFormat"),
                        SizeLimit = Config.GetInt("scrawlMaxSize"),
                        UploadFieldName = Config.GetString("scrawlFieldName"),
                        Base64 = true,
                        Base64Filename = "scrawl.png"
                    });
                    break;
                case "uploadvideo":
                    actionHanler = new UploadHandler(context, new UploadConfig()
                    {
                        UploadDir = _uploadDir,
                        AllowExtensions = Config.GetStringList("videoAllowFiles"),
                        PathFormat = Config.GetString("videoPathFormat"),
                        SizeLimit = Config.GetInt("videoMaxSize"),
                        UploadFieldName = Config.GetString("videoFieldName")
                    });
                    break;
                case "uploadfile":
                    actionHanler = new UploadHandler(context, new UploadConfig()
                    {
                        UploadDir = _uploadDir,
                        AllowExtensions = Config.GetStringList("fileAllowFiles"),
                        PathFormat = Config.GetString("filePathFormat"),
                        SizeLimit = Config.GetInt("fileMaxSize"),
                        UploadFieldName = Config.GetString("fileFieldName")
                    });
                    break;
                case "listimage":
                    actionHanler = new ListFileManager(context, _uploadDir, Config.GetString("imageManagerListPath"), Config.GetStringList("imageManagerAllowFiles"));
                    break;
                case "listfile":
                    actionHanler = new ListFileManager(context,_uploadDir, Config.GetString("fileManagerListPath"), Config.GetStringList("fileManagerAllowFiles"));
                    break;
                case "catchimage":
                    actionHanler = new CrawlerHandler(context);
                    break;
                default:
                    actionHanler = new NotSupportedHandler(context);
                    break;
            }
            actionHanler.Process();
            // return Ok<string>(actionHanler.Process());
            //context.ApplicationInstance.CompleteRequest();
            //context.Response.End();
        }
    }
}