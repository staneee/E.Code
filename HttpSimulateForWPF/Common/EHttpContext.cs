using System.IO;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.Web;

namespace EHttp
{
    public class EHttpContext
    {
        public string RequestBody { get; set; }
        /// <summary>
        /// Get请求的数据
        /// </summary>
        public NameValueCollection RequestGetData { get; set; }
        /// <summary>
        /// post请求的数据
        /// </summary>
        public NameValueCollection RequestPostData { get; set; }
        /// <summary>
        /// Response Writer
        /// </summary>
        public StreamWriter ResponseWriter { get; set; }
        /// <summary>
        /// 请求上下文
        /// </summary>
        public HttpListenerContext Context { get; set; }
        /// <summary>
        /// Context Encoding
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 请求头
        /// </summary>
        public NameValueCollection Headers
        {
            get => Context.Request.Headers;
        }

        /// <summary>
        /// ResponseStatusCode
        /// </summary>
        public int OutStatusCode
        {
            get => Context.Response.StatusCode;
            set => Context.Response.StatusCode = value;
        }
        /// <summary>
        ///  ResponseContentType
        /// </summary>
        public string OutContentType
        {
            get => Context.Response.ContentType;
            set => Context.Response.ContentType = value;
        }
        /// <summary>
        /// ResponseHeaders
        /// </summary>
        public WebHeaderCollection OutHeaders
        {
            get => Context.Response.Headers;
            set => Context.Response.Headers = value;
        }

        public EHttpContext(HttpListenerContext context)
        {
            // 记录请求上下文
            Context = context;

            // 记录 Url 携带的 QueryString
            RequestGetData = Context.Request.QueryString;

            // 记录 Body 的数据
            var stream = Context.Request.InputStream;
            var reader = new StreamReader(stream, Encoding);
            RequestBody = reader.ReadToEnd();

            RequestPostData = HttpUtility.ParseQueryString(RequestBody);

            //// 
            OutContentType = "text/plain";
            OutStatusCode = 200;
            //


        }

        /// <summary>
        /// 获取请求携带的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="requestType">请求类型,默认为get(get/post)</param>
        /// <returns>取值,未找到值返回null</returns>
        public string Request(string key, RequestType requestType = RequestType.Get)
        {
            string result = null;

            switch (requestType)
            {
                case RequestType.Get:
                    result = RequestGetData.Get(key);
                    break;
                case RequestType.Post:
                    result = RequestPostData.Get(key);
                    break;
            }

            return result;
        }


        ///// <summary>
        ///// 写入数据到响应流
        ///// </summary>
        ///// <param name="value"></param>
        //public void ResponseWrite(string value) => ResponseWriter.Write(value);

        /// <summary>
        /// 结束响应
        /// </summary>
        public void ResponseEnd()
        {
            ResponseWriter.Close();
            Context.Response.Close();
        }

        /// <summary>
        /// 写入数据到响应流冰结束响应
        /// </summary>
        /// <param name="value"></param>
        public void ResponseWriteEnd(object value)
        {
            if (ResponseWriter == null)
            {
                ResponseWriter = new StreamWriter(Context.Response.OutputStream, Encoding);
            }
            ResponseWriter.Write(value);
            ResponseEnd();
        }
    }
}
