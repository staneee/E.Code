using E.StringEx;
using E.StringEx.Url;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHttp
{
    /// <summary>
    /// Http请求帮助类
    /// </summary>
    public static class HttpHelper
    {
        static HttpHelper()
        {
            Cookies = new CookieContainer();
        }

        /// <summary>
        /// Cookies容器
        /// </summary>
        public static CookieContainer Cookies { get; set; }

        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">请求携带的数据</param>
        /// <returns></returns>
        public static bool HttpRequest(this string url, HttpData data)
        {

            try
            {
                #region 若为GET请求并且携带数据则重建url

                if (data.Method == RequestType.GET && data.Data != null && data.Data.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.Append(url);
                    sb.Append("?");
                    bool isStart = true;
                    foreach (KeyValuePair<string, string> item in data.Data)
                    {
                        if (!isStart)
                        {
                            sb.AppendFormat("&{0}={1}", item.Key, item.Value);
                            continue;
                        }

                        isStart = false;
                        sb.AppendFormat("{0}={1}", item.Key, item.Value);
                    }
                    url = sb.ToString();
                }

                #endregion


                // 创建HTTP请求
                var request = WebRequest.Create(url) as HttpWebRequest;


                #region 写入Cookies

                // 写入Cookies
                if (data.EnabledCookie)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(data.Cookies);
                }

                #endregion


                #region HTTPS

                if (url.ToLower().StartsWith("https"))
                {
                    // https
                    //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                }

                #endregion



                #region 设置请求头

                switch (data.Method)
                {
                    case RequestType.GET:
                        request.Method = "GET";
                        break;
                    case RequestType.POST:
                        request.Method = "POST";
                        break;
                }

                request.ProtocolVersion = HttpVersion.Version11;

                if (data.EnabledIfModifiedSinceHTTP)
                    request.IfModifiedSince = data.IfModifiedSinceHTTP;

                if (!data.Accept.IsNull())
                    request.Accept = data.Accept;

                if (!data.UserAgent.IsNull())
                    request.UserAgent = data.UserAgent;

                if (!data.Host.IsNull())
                    request.Host = data.Host;

                if (!data.ContentType.IsNull())
                    request.ContentType = data.ContentType;

                if (!data.AcceptLanguage.IsNull())
                    request.Headers.Add("Accept-Language", data.AcceptLanguage);

                if (!data.AcceptEncoding.IsNull())
                    request.Headers.Add("Accept-Encoding", data.AcceptEncoding);

                if (!data.Origin.IsNull())
                    request.Headers.Add("Origin", data.Origin);

                if (!data.Referer.IsNull())
                    request.Referer = data.Referer;

                request.KeepAlive = data.KeepAlive;

                if (!data.UpgradeInsecureRequests.IsNull())
                    request.Headers.Add("Upgrade-Insecure-Requests", data.UpgradeInsecureRequests);


                #endregion



                #region POST 写入数据

                // POST 数据附加到请求
                if (data.Method == RequestType.POST && data.Data != null && data.Data.Count > 0)
                {
                    request.WriteData(data.Data);
                }

                #endregion


                // Request对象初始化完成
                data.RequestCreateComplete(data, request);


                // 发送请求
                var response = request.GetResponse() as HttpWebResponse;


                #region 记录Cookie/读取响应流

                if (!data.ResponseCreateCompleteIsNull())
                {
                    data.ResponseCreateComplete(data, new HttpEventArgs(request, response));

                }
                else
                {
                    data.Cookies = response.Cookies;
                    var stream = response.GetResponseStream();

                    try
                    {
                        stream = new GZipStream(stream, CompressionMode.Decompress);
                    }
                    catch
                    {
                    }

                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        data.ResponseData = sr.ReadToEnd();
                        stream.Close();
                        stream.Dispose();
                    }
                }
                response.Dispose();

                #endregion



                return true;
            }
            catch (Exception e)
            {
                data.HttpError(data, e);
                return false;
            }
        }


        /// <summary>
        /// 发送http请求(异步)
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">请求携带的数据</param>
        /// <returns></returns>
        public static async Task<bool> HttpRequestAsync(this string url, HttpData data)
        {

            try
            {
                #region 若为GET请求并且携带数据则重建url

                if (data.Method == RequestType.GET && data.Data != null && data.Data.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.Append(url);
                    sb.Append("?");
                    bool isStart = true;
                    foreach (KeyValuePair<string, string> item in data.Data)
                    {
                        if (!isStart)
                        {
                            sb.AppendFormat("&{0}={1}", item.Key, item.Value);
                            continue;
                        }

                        isStart = false;
                        sb.AppendFormat("{0}={1}", item.Key, item.Value);
                    }
                    url = sb.ToString();
                }

                #endregion


                // 创建HTTP请求
                var request = WebRequest.Create(url) as HttpWebRequest;


                #region 写入Cookies

                // 写入Cookies
                if (data.EnabledCookie)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(data.Cookies);
                }

                #endregion


                #region HTTPS

                if (url.ToLower().StartsWith("https"))
                {
                    // https
                    //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                }

                #endregion



                #region 设置请求头

                switch (data.Method)
                {
                    case RequestType.GET:
                        request.Method = "GET";
                        break;
                    case RequestType.POST:
                        request.Method = "POST";
                        break;
                }

                request.ProtocolVersion = HttpVersion.Version11;

                if (data.EnabledIfModifiedSinceHTTP)
                    request.IfModifiedSince = data.IfModifiedSinceHTTP;

                if (!data.Accept.IsNull())
                    request.Accept = data.Accept;

                if (!data.UserAgent.IsNull())
                    request.UserAgent = data.UserAgent;

                if (!data.Host.IsNull())
                    request.Host = data.Host;

                if (!data.ContentType.IsNull())
                    request.ContentType = data.ContentType;

                if (!data.AcceptLanguage.IsNull())
                    request.Headers.Add("Accept-Language", data.AcceptLanguage);

                if (!data.AcceptEncoding.IsNull())
                    request.Headers.Add("Accept-Encoding", data.AcceptEncoding);

                if (!data.Origin.IsNull())
                    request.Headers.Add("Origin", data.Origin);

                if (!data.Referer.IsNull())
                    request.Referer = data.Referer;

                request.KeepAlive = data.KeepAlive;

                if (!data.UpgradeInsecureRequests.IsNull())
                    request.Headers.Add("Upgrade-Insecure-Requests", data.UpgradeInsecureRequests);


                #endregion



                #region POST 写入数据

                // POST 数据附加到请求
                if (data.Method == RequestType.POST && data.Data != null && data.Data.Count > 0)
                {
                    request.WriteData(data.Data);
                }

                #endregion


                // Request对象初始化完成
                data.RequestCreateComplete(data, request);


                // 发送请求
                var response = (await request.GetResponseAsync()) as HttpWebResponse;


                #region 记录Cookie/读取响应流

                if (!data.ResponseCreateCompleteIsNull())
                {
                    data.ResponseCreateComplete(data, new HttpEventArgs(request, response));

                }
                else
                {
                    data.Cookies = response.Cookies;
                    var stream = response.GetResponseStream();

                    try
                    {
                        stream = new GZipStream(stream, CompressionMode.Decompress);
                    }
                    catch
                    {
                    }

                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        data.ResponseData = sr.ReadToEnd();
                        stream.Close();
                        stream.Dispose();
                    }
                }
                response.Dispose();

                #endregion



                return true;
            }
            catch (Exception e)
            {
                data.HttpError(data, e);
                return false;
            }
        }

        static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        /// <summary>
        /// 写入数据流到请求中
        /// </summary>
        /// <param name="request"></param>
        /// <param name="data"></param>
        static void WriteData(this HttpWebRequest request, Dictionary<string, string> data)
        {
            StringBuilder sb = null;
            var temp = string.Empty;
            foreach (var item in data)
            {
                if (null != sb)
                    sb.Append("&");
                else
                    sb = new StringBuilder();

                temp = item.Key.UrlEncode();
                sb.Append(temp);
                sb.Append("=");
                temp = item.Value.UrlEncode();
                sb.Append(temp);
            }


            byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());
            request.ContentLength = bytes.Length;
            using (var os = request.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length); //Push it out there
            }
        }
    }
}
