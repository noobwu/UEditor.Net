using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;


namespace UEditor.Net
{
    /// <summary>
    /// UploadHandler 的摘要说明
    /// </summary>
    public class UploadHandler : Handler
    {
        /// <summary>
        /// 
        /// </summary>
        public const int BUFFER_SIZE = 4096;
        /// <summary>
        /// 
        /// </summary>
        public UploadConfig UploadConfig { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public UploadResult Result { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        public UploadHandler(HttpContext context, UploadConfig config)
            : base(context)
        {
            this.UploadConfig = config;
            this.Result = new UploadResult() { State = UploadState.Unknown };
        }
        /// <summary>
        /// 
        /// </summary>
        public async override Task Process()
        {
            byte[] uploadFileBytes = null;
            string uploadFileName = null;

            if (UploadConfig.Base64)
            {
                uploadFileName = UploadConfig.Base64Filename;
                uploadFileBytes = Convert.FromBase64String(Request[UploadConfig.UploadFieldName]);
            }
            else
            {
                var file = Request.Files[UploadConfig.UploadFieldName];
                uploadFileName = file.FileName;

                if (!CheckFileType(uploadFileName))
                {
                    Result.State = UploadState.TypeNotAllow;
                    WriteResult();
                    return;
                }
                if (!CheckFileSize(file.ContentLength))
                {
                    Result.State = UploadState.SizeLimitExceed;
                    WriteResult();
                    return;
                }

                uploadFileBytes = new byte[file.ContentLength];
                try
                {
                    await file.InputStream.ReadAsync(uploadFileBytes, 0, file.ContentLength);
                   //file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
                }
                catch (Exception)
                {
                    Result.State = UploadState.NetworkError;
                    WriteResult();
                }
            }

            Result.OriginFileName = uploadFileName;

            var savePath = PathFormatter.Format(uploadFileName, UploadConfig.PathFormat);
            if (!string.IsNullOrEmpty(UploadConfig.UploadDir))
            {
                savePath = "/" + UploadConfig.UploadDir.Trim('/') + "/" + savePath.Trim('/');
            }
            var localPath = Server.MapPath(savePath);
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                }
                //File.WriteAllBytes(localPath, uploadFileBytes);
                using (var fs = new FileStream(path: localPath,mode: FileMode.Create,
                                      access: FileAccess.Write,
                                      share: FileShare.None,
                                      bufferSize: 4096,
                                      useAsync: true))
                {
                     await fs.WriteAsync(uploadFileBytes, 0, uploadFileBytes.Length);
                }
                Result.Url = savePath;
                Result.State = UploadState.Success;
            }
            catch (Exception e)
            {
                Result.State = UploadState.FileAccessError;
                Result.ErrorMessage = e.Message;
            }
            finally
            {
                WriteResult();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void WriteResult()
        {
            this.WriteJson(new
            {
                state = GetStateMessage(Result.State),
                url = Result.Url,
                title = Result.OriginFileName,
                original = Result.OriginFileName,
                error = Result.ErrorMessage
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private string GetStateMessage(UploadState state)
        {
            switch (state)
            {
                case UploadState.Success:
                    return "SUCCESS";
                case UploadState.FileAccessError:
                    return "文件访问出错，请检查写入权限";
                case UploadState.SizeLimitExceed:
                    return "文件大小超出服务器限制";
                case UploadState.TypeNotAllow:
                    return "不允许的文件格式";
                case UploadState.NetworkError:
                    return "网络错误";
            }
            return "未知错误";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool CheckFileType(string filename)
        {
            var fileExtension = Path.GetExtension(filename).ToLower();
            return UploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool CheckFileSize(int size)
        {
            return size < UploadConfig.SizeLimit;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class UploadConfig
    {
        /// <summary>
        /// 上传目录
        /// </summary>
        public string UploadDir { get; set; }
        /// <summary>
        /// 文件命名规则
        /// </summary>
        public string PathFormat { get; set; }

        /// <summary>
        /// 上传表单域名称
        /// </summary>
        public string UploadFieldName { get; set; }

        /// <summary>
        /// 上传大小限制
        /// </summary>
        public int SizeLimit { get; set; }

        /// <summary>
        /// 上传允许的文件格式
        /// </summary>
        public string[] AllowExtensions { get; set; }

        /// <summary>
        /// 文件是否以 Base64 的形式上传
        /// </summary>
        public bool Base64 { get; set; }

        /// <summary>
        /// Base64 字符串所表示的文件名
        /// </summary>
        public string Base64Filename { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class UploadResult
    {
        public UploadState State { get; set; }
        public string Url { get; set; }
        public string OriginFileName { get; set; }

        public string ErrorMessage { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public enum UploadState
    {
        Success = 0,
        SizeLimitExceed = -1,
        TypeNotAllow = -2,
        FileAccessError = -3,
        NetworkError = -4,
        Unknown = 1,
    }
}

