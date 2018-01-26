using EHttp;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HttpSimulateForWPF.ViewModels
{
    public class MainWindowModel : NotificationObject
    {
        #region Props

        private bool _useFilter;
        /// <summary>
        /// 拦截启用
        /// </summary>
        public bool UseFilter
        {
            get => _useFilter;
            set
            {
                _useFilter = value;
                RaisePropertyChanged("UseFilter");
            }
        }
        /// <summary>
        /// 是否可响应
        /// </summary>
        public bool CanReponse { get; set; }

        private string _btnFilterTxt = "等待请求";

        public string BtnFilterTxt
        {
            get => _btnFilterTxt;
            set
            {
                _btnFilterTxt = value;
                RaisePropertyChanged("BtnFilterTxt");
            }
        }

        private string _host = string.Empty;
        /// <summary>
        /// 监听的地址
        /// </summary>
        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                RaisePropertyChanged("Host");
            }
        }

        private string _requestHeader = string.Empty;

        /// <summary>
        /// 请求头
        /// </summary>
        public string RequestHeader
        {
            get => _requestHeader;

            set
            {
                _requestHeader = value;
                RaisePropertyChanged("RequestHeader");
            }
        }

        private string _requestBody = string.Empty;
        /// <summary>
        /// 请求体
        /// </summary>
        public string RequestBody
        {
            get => _requestBody;
            set
            {
                _requestBody = value;
                RaisePropertyChanged("RequestBody");
            }
        }

        private string _responseHeader = string.Empty;
        /// <summary>
        /// 响应头
        /// </summary>
        public string ResponseHeader
        {
            get => _responseHeader;
            set
            {
                _responseHeader = value;
                RaisePropertyChanged("ResponseHeader");
            }
        }

        private string _responseBody = string.Empty;
        /// <summary>
        /// 响应体
        /// </summary>
        public string ResponseBody
        {
            get => _responseBody;
            set
            {
                _responseBody = value;
                RaisePropertyChanged("ResponseBody");
            }
        }

        #endregion

        public MainWindowModel()
        {
            _useFilter = false;
        }

        static HttpListener Listener { get; set; }

        public bool Start(out string msg)
        {
            msg = string.Empty;

            if (Listener == null)
            {
                Listener = new HttpListener();
            }
            if (Listener.IsListening)
            {
                goto success;
            }

            if (string.IsNullOrWhiteSpace(Host))
            {
                msg = "url为空";
                goto failure;
            }

            if (!Host.EndsWith("/"))
                Host = $"{Host}/";



            try
            {
                // 指定身份验证 Anonymous匿名访问
                Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                Listener.Prefixes.Add(Host);
                Listener.Start();

                Task.Factory.StartNew((listener) =>
                {
                    StartListener((HttpListener)listener);
                }, Listener);
            }
            catch (Exception ex)
            {
                msg = $"监听失败！错误信息:{ex}";
                goto failure;
            }


            success:
            return true;


            failure:
            return false;
        }



        private void StartListener(HttpListener listener)
        {
            while (true)
            {
                try
                {
                    var ctx = listener.GetContext();
                    if (UseFilter)
                    {
                        BtnFilterTxt = "释放拦截!";
                        CanReponse = false;
                    }
                    HttpExec(ctx);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        void HttpExec(HttpListenerContext httpListenerContext)
        {
            var context = new EHttpContext(httpListenerContext);
            GetRequestContent(context);

            while (UseFilter && !CanReponse)
            {
                Thread.Sleep(500);
            }

            SetReponseContent(context);
        }

        private void GetRequestContent(EHttpContext context)
        {
            var sb = new StringBuilder();
            foreach (var key in context.Headers.AllKeys)
            {
                sb.AppendLine($"{key}:{context.Headers[key]}");
            }
            RequestHeader = sb.ToString();
            sb.Clear();

            sb.AppendLine("--------- GetData ---------");
            foreach (var key in context.RequestGetData.AllKeys)
            {
                sb.AppendLine($"{key}:{context.RequestGetData[key]}");
            }

            sb.AppendLine("\r\n--------- PostData ---------");
            foreach (var key in context.RequestPostData.AllKeys)
            {
                sb.AppendLine($"{key}:{context.RequestPostData[key]}");
            }

            sb.AppendLine("\r\n--------- RequestBody ---------");
            sb.Append(context.RequestBody);

            RequestBody = sb.ToString();
        }


        static readonly char[] SpliteLine = { '\r', '\n' };
        static readonly char[] SpliteItem = { ':', '：' };
        void SetReponseContent(EHttpContext context)
        {
            #region AddHeader

            var lines = ResponseHeader.Trim().Split(SpliteLine, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 1)
            {
                foreach (var line in lines)
                {
                    var tmpHeader = line.Split(SpliteItem, StringSplitOptions.RemoveEmptyEntries);
                    if (tmpHeader.Length != 2)
                    {
                        MessageBox.Show($"响应头信息错误!\r\n{line}");
                        return;
                    }
                    else if (string.IsNullOrWhiteSpace(tmpHeader[0].Trim()))
                    {
                        MessageBox.Show($"响应头信息错误!\r\n{line}");
                        return;
                    }
                    else
                    {
                        if (tmpHeader[0].Trim().ToLower() == "context-type")
                        {
                            context.OutContentType = tmpHeader[1].Trim();
                        }
                        else if (tmpHeader[0].Trim().ToLower() == "status code")
                        {
                            if (!int.TryParse(tmpHeader[1].Trim(), out int statusCode))
                            {
                                statusCode = 200;
                            }
                            context.OutStatusCode = statusCode;
                        }
                        else
                            context.OutHeaders.Add(tmpHeader[0].Trim(), tmpHeader[1].Trim());
                    }
                }
                context.OutHeaders.Add("Access-Control-Allow-Methods", "*");
                context.OutHeaders.Add("Access-Control-Allow-Headers", "*");
                context.OutHeaders.Add("Access-Control-Allow-Origin", "*");
            }
            #endregion


            context.ResponseWriteEnd(ResponseBody);
        }

        public void Stop()
        {
            if (Listener != null)
            {
                Listener.Stop();
                Listener.Close();
                Listener = null;
            }
        }

        public void SetCanResponse()
        {
            if (!CanReponse)
            {
                BtnFilterTxt = "等待请求";
                CanReponse = true;
            }
        }
    }
}
