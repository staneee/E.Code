using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace EHttp
{
    /// <summary>
    /// HTTP请求头信息常量类
    /// </summary>
    public class HttpHeader
    {
        /// <summary>
        /// Content-type
        /// </summary>
        public const string CONTENT_TYPE = "application/x-www-form-urlencoded";

        /// <summary>
        /// User-Agent
        /// </summary>
        public const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";

        /// <summary>
        /// Accept
        /// </summary>
        public const string ACCEPT = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";

        /// <summary>
        /// Accept-Languang
        /// </summary>
        public const string ACCEPT_LANGUANG = "zh-CN,zh;q=0.9";

        /// <summary>
        /// Accept-Encoding
        /// </summary>
        public const string ACCEPT_ENCODING = "gzip, deflate, br";

        /// <summary>
        /// Https证书类型 Tls
        /// </summary>
        public const SecurityProtocolType HTTPS_Tls = SecurityProtocolType.Tls;

        /// <summary>
        /// Https证书类型 Tls11
        /// </summary>
        public const SecurityProtocolType HTTPS_Tls11 = SecurityProtocolType.Tls11;

        /// <summary>
        /// Https证书类型 Tls12
        /// </summary>
        public const SecurityProtocolType HTTPS_Tls12 = SecurityProtocolType.Tls12;

        /// <summary>
        /// Https证书类型 Ssl3
        /// </summary>
        public const SecurityProtocolType HTTPS_Ssl3 = SecurityProtocolType.Ssl3;
    }
}
