using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace EHttp
{
    /// <summary>
    /// HTTP请求携带的数据对象
    /// </summary>
    public class HttpData
    {
        public HttpData()
        {
            Cookies = new CookieCollection();
            EnabledCookie = true;

            Accept = HttpHeader.ACCEPT;
            UserAgent = HttpHeader.USER_AGENT;
            ContentType = HttpHeader.CONTENT_TYPE;
            AcceptLanguage = HttpHeader.ACCEPT_LANGUANG;
            AcceptEncoding = HttpHeader.ACCEPT_ENCODING;
            Method = RequestType.GET;
            KeepAlive = true;

            HttpsType = HttpHeader.HTTPS_Ssl3;
        }


        #region 请求头

        /// <summary>
        /// 启用cookie的状态
        /// (默认启用)
        /// </summary>
        public bool EnabledCookie { get; set; }

        /// <summary>
        /// 请求头 Cookies
        /// </summary>
        public CookieCollection Cookies { get; set; }

        /// <summary>
        /// 请求头 Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 请求头 Accept
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// 请求头 UserAgent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 请求头 ContentType
        /// </summary>
        public string ContentType { get; set; }


        /// <summary>
        /// 请求头 AcceptLanguage
        /// </summary>
        public string AcceptLanguage { get; set; }

        /// <summary>
        /// 请求头 AcceptEncoding
        /// </summary>
        public string AcceptEncoding { get; set; }

        /// <summary>
        /// 请求头 Origin
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 请求头 Referer
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        /// 请求头 KeepAlive
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// 请求头 请求方式
        /// </summary>
        public RequestType Method { get; set; }


        /// <summary>
        /// 是否启用请求头 If-Modified-SinceHTTP
        /// </summary>
        public bool EnabledIfModifiedSinceHTTP { get; set; }

        /// <summary>
        /// 请求头 If-Modified-SinceHTTP
        /// </summary>
        public DateTime IfModifiedSinceHTTP { get; set; }

        /// <summary>
        /// 请求头 Upgrade-Insecure-Requests
        /// </summary>
        public string UpgradeInsecureRequests { get; set; }

        /// <summary>
        /// HTTPS 类型,默认SSL3
        /// </summary>
        public SecurityProtocolType HttpsType { get; set; }

        /// <summary>
        /// 请求携带的数据
        /// </summary>
        public Dictionary<string, string> Data { get; set; }

        #endregion


        #region 响应信息

        /// <summary>
        /// 响应的结果数据
        /// </summary>
        public string ResponseData { get; set; }

        #endregion


        /// <summary>
        /// 请求创建完成
        /// (HttpData默认值已设置)
        /// </summary>
        public event EventHandler<HttpWebRequest> OnRequestCreateComplete;


        /// <summary>
        /// 请求响应结束的事件
        /// (使用此事件将不会启用默认的cookie记录和响应流读取)
        /// </summary>
        public event EventHandler<HttpEventArgs> OnResponseCreateComplete;

        /// <summary>
        /// 请求过程中发生错误事件
        /// </summary>
        public event EventHandler<Exception> OnHttpError;


        /// <summary>
        /// 触发 请求创建完成 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RequestCreateComplete(object sender, HttpWebRequest e)
        {
            OnRequestCreateComplete?.Invoke(sender, e);
        }


        /// <summary>
        /// 触发 请求响应结束 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ResponseCreateComplete(object sender, HttpEventArgs e)
        {
            OnResponseCreateComplete?.Invoke(sender, e);
        }


        /// <summary>
        /// 检查是否绑定了 ResponseCreateComplete 事件
        /// </summary>
        /// <returns></returns>
        public bool ResponseCreateCompleteIsNull()
        {
            return OnResponseCreateComplete == null;
        }

        /// <summary>
        /// 触请求过程中发生错误事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HttpError(object sender, Exception e)
        {
            OnHttpError?.Invoke(sender, e);
        }




        /// <summary>
        /// 读取响应流到 ResponseData
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="unGzip">是否对流进行gzip解压,默认启用</param>
        /// <param name="encoding">读取的编码格式,默认为UTF-8</param>
        /// <returns>读取成功返回null,读取失败返回错误信息</returns>
        public Exception StreamRead(Stream stream, bool unGzip = true, Encoding encoding = null)
        {
            try
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;


                // GZIP解压
                if (unGzip)
                {
                    stream = new GZipStream(stream, CompressionMode.Decompress);
                }

                // 读取流数据
                using (var sr = new StreamReader(stream, encoding))
                {
                    this.ResponseData = sr.ReadToEnd();
                }
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }

    }
}
