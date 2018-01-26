using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EHttp
{
    public class Listener
    {
        static HttpListener _listener { get; set; }
        public bool Start(string url, out string msg)
        {
            msg = string.Empty;

            if (_listener == null)
            {
                _listener = new HttpListener();
            }
            if (_listener.IsListening)
            {
                goto success;
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                msg = "url为空";
                goto failure;
            }

            if (!url.EndsWith("/"))
                url = $"{url}/";



            try
            {
                // 指定身份验证 Anonymous匿名访问
                _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                _listener.Prefixes.Add(url);
                _listener.Start();

                Task.Factory.StartNew((listener) =>
                {
                    Start((HttpListener)listener);
                }, _listener);
            }
            catch (Exception ex)
            {
                msg = $"监听失败！错误信息:{ex.ToString()}";
                goto failure;
            }


            success:
            return true;


            failure:
            return false;
        }

        void Start(HttpListener listener)
        {
            while (true)
            {
                var ctx = listener.GetContext();
            }
        }
    }
}
