using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace EHttp
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpEventArgs : EventArgs
    {
        public HttpEventArgs(HttpWebRequest request, HttpWebResponse response)
        {
            Request = request;
            Reponse = response;
        }

        /// <summary>
        /// 请求
        /// </summary>
        public HttpWebRequest Request { get; set; }

        /// <summary>
        /// 响应
        /// </summary>
        public HttpWebResponse Reponse { get; set; }
    }
}
