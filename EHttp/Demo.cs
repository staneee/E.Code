using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EHttp
{
    /// <summary>
    /// Demo
    /// </summary>
    public class Demo
    {

        public async Task Test()
        {
            string url = "http://www.baidu.com";

            HttpData data = new HttpData();

            // 注册事件
            data.OnRequestCreateComplete += Data_OnRequestCreateComplete;
            data.OnResponseCreateComplete += Data_OnResponseCreateComplete;
            data.OnHttpError += Data_OnHttpError;

            // 普通
            if (url.HttpRequest(data))
            {
                // xxxxxxxxx


            }

            // 异步
            if (await url.HttpRequestAsync(data))
            {
                // xxxxxxxxx


            }
        }

        private void Data_OnHttpError(object sender, Exception e)
        {

        }

        private void Data_OnResponseCreateComplete(object sender, HttpEventArgs e)
        {

        }

        private void Data_OnRequestCreateComplete(object sender, System.Net.HttpWebRequest e)
        {

        }
    }
}
