using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FruitSortingVtest1
{
    /// <summary>
    /// 请求信息帮助
    /// </summary>
    public partial class HttpHelper
    {
        //body是要传递的参数,格式"roleId=1&uid=2"
        //post的cotentType填写:
        //"application/x-www-form-urlencoded"
        //soap填写:"text/xml; charset=utf-8"
        public static string PostHttp(string url, string body, string contentType)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 20000;
            byte[] btBodys = Encoding.UTF8.GetBytes(body);
            httpWebRequest.ContentLength = btBodys.Length;
            httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();
            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();
            return responseContent;


        }

        //POST方法(httpWebRequest)
        /// <summary>
        /// 通过WebClient类Post数据到远程地址，需要Basic认证；
        /// 调用端自己处理异常
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="paramStr">name=张三&age=20</param>
        /// <param name="encoding">请先确认目标网页的编码方式</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Request_WebClient(string uri, string paramStr, Encoding encoding, string username, string password)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            string result = string.Empty;
            TimeoutWebClient wc = new TimeoutWebClient(2000, new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0));
            // 采取POST方式必须加的Header
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] postData = encoding.GetBytes(paramStr);
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                wc.Credentials = GetCredentialCache(uri, username, password);
                wc.Headers.Add("Authorization", GetAuthorization(username, password));
            }

            byte[] responseData = wc.UploadData(uri, "POST", postData); // 得到返回字符流
            return encoding.GetString(responseData);// 解码                  
        }

        //POST方法(WebClient)
        public static string GetHttp(string url, HttpContext httpContext)
        {
            string queryString = "?";
            foreach (string key in httpContext.Request.QueryString.AllKeys)
            {
                queryString += key + "=" + httpContext.Request.QueryString[key] + "&";
            }
            queryString = queryString.Substring(0, queryString.Length - 1);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url + queryString);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 20000;
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();
            httpWebResponse.Close();
            streamReader.Close();
            return responseContent;
        }


        #region public static string OpenReadWithHttps(string URL, string strPostdata,int timeout, IPEndPoint ip ) 采用https协议访问网络
        ///<summary>
        ///采用https协议访问网络
        ///</summary>
        ///<param name="url">url地址</param>
        ///<param name="postdata">发送的数据</param>
        ///<returns></returns>
        public static string OpenReadWithHttps(string url, string postdata, int timeout, IPEndPoint ip)
        {
            try
            {
                IPEndPoint m_OutIPEndPoint;
                m_OutIPEndPoint = ip;
                Encoding encoding = Encoding.UTF8;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = null;
                //request.Timeout = 240000;
                request.Timeout = timeout;
                request.ServicePoint.BindIPEndPointDelegate = (servicePoint, remoteEndPoint, retryCount) =>
                {
                    return m_OutIPEndPoint;
                };

                request.CookieContainer = new CookieContainer();
                request.Method = "POST";
                request.Accept = "*/*";
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] buffer = encoding.GetBytes(postdata);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StringBuilder sb = new StringBuilder();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    const int length = 1024;
                    char[] c = new char[length];
                    int index = reader.Read(c, 0, c.Length);
                    while (index > 0)
                    {
                        var list = c.ToList();
                        list.RemoveAll(c_ => c_ <= 0);
                        c = list.ToArray();
                        sb.Append(c);
                        c = new char[length];
                        index = reader.Read(c, 0, c.Length);
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {

                return "网络异常" + ex.Message;
            }
        }
        #endregion

        //Get方法(HttpWebRequest)
        /// <summary>
        /// 通过 WebRequest/WebResponse 类访问远程地址并返回结果，需要Basic认证；
        /// 调用端自己处理异常
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="timeout">访问超时时间，单位毫秒；如果不设置超时时间，传入0</param>
        /// <param name="encoding">如果不知道具体的编码，传入null</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Request_WebRequest(string uri, int timeout, Encoding encoding, string username, string password)
        {
            string result = string.Empty;
            WebRequest request = WebRequest.Create(new Uri(uri));
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                request.Credentials = GetCredentialCache(uri, username, password);
                request.Headers.Add("Authorization", GetAuthorization(username, password));
            }
            if (timeout > 0)
                request.Timeout = timeout;
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = encoding == null ? new StreamReader(stream) : new StreamReader(stream, encoding);
            result = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return result;
        }

        #region # 生成 Http Basic 访问凭证 #

        private static CredentialCache GetCredentialCache(string uri, string username, string password)
        {
            string authorization = string.Format("{0}:{1}", username, password);
            CredentialCache credCache = new CredentialCache();
            credCache.Add(new Uri(uri), "Basic", new NetworkCredential(username, password));
            return credCache;
        }

        private static string GetAuthorization(string username, string password)
        {
            string authorization = string.Format("{0}:{1}", username, password);
            return "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(authorization));
        }
        #endregion
        //basic验证的WebRequest/WebResponse

        //public static void OpenReadWithHttps()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
