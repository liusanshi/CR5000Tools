using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Zuken.Common.Net
{
    /// <summary>
    /// 网页 助手
    /// </summary>
    public static class WebPageHelper
    {
        /// <summary>
        /// 获取CooKie
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="postdata"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static CookieContainer GetCooKie(string loginUrl, string postdata, HttpHeader header)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                CookieContainer cc = new CookieContainer();
                request = (HttpWebRequest)WebRequest.Create(loginUrl);
                request.Method = header.method;
                request.ContentType = header.contentType;
                byte[] postdatabyte = Encoding.UTF8.GetBytes(postdata);
                request.ContentLength = postdatabyte.Length;
                request.AllowAutoRedirect = false;
                request.CookieContainer = cc;
                request.KeepAlive = true;

                //提交请求
                Stream stream;
                stream = request.GetRequestStream();
                stream.Write(postdatabyte, 0, postdatabyte.Length);
                stream.Close();

                //接收响应
                response = (HttpWebResponse)request.GetResponse();
                response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);

                //CookieCollection cook = response.Cookies;
                //Cookie字符串格式
                //string strcrook = request.CookieContainer.GetCookieHeader(request.RequestUri);

                return cc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (request != null) request.Abort();
                if (response != null) response.Close();
            }
        }

        /// <summary>
        /// 从服务器获取数据
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string GetData(string getUrl, CookieContainer cookieContainer, HttpHeader header)
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(getUrl);
                httpWebRequest.CookieContainer = cookieContainer ?? new CookieContainer();
                httpWebRequest.ContentType = "text/html;";
                httpWebRequest.ServicePoint.ConnectionLimit = header.maxTry;
                httpWebRequest.Referer = getUrl;
                httpWebRequest.Accept = header.accept;
                httpWebRequest.UserAgent = header.userAgent;
                httpWebRequest.Method = "GET";
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
            }
        }
        /// <summary>
        /// 向服务器发送数据
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="postdata"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string PostData(string getUrl, string postdata, CookieContainer cookieContainer, HttpHeader header)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(getUrl);
                request.Method = "POST";
                request.ContentType = header.contentType;
                byte[] postdatabyte = Encoding.UTF8.GetBytes(postdata);
                request.ContentLength = postdatabyte.Length;
                request.AllowAutoRedirect = false;
                request.CookieContainer = cookieContainer ?? new CookieContainer();
                request.KeepAlive = true;

                //提交请求
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postdatabyte, 0, postdatabyte.Length);
                }

                //接收响应
                response = (HttpWebResponse)request.GetResponse();
                using(Stream responseStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (request != null) request.Abort();
                if (response != null) response.Close();
            }
        }
    }


    /// <summary>
    /// HttpHeader
    /// </summary>
    public class HttpHeader
    {
        private static HttpHeader mDefaultHeader = null;
        /// <summary>
        /// 默认的 HttpHeader
        /// </summary>
        public static HttpHeader DefaultHeader
        {
            get
            {
                if (mDefaultHeader == null)
                {
                    mDefaultHeader = new HttpHeader();
                    mDefaultHeader.accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, */*";
                    mDefaultHeader.contentType = "application/x-www-form-urlencoded";
                    mDefaultHeader.method = "POST";
                    mDefaultHeader.userAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
                    mDefaultHeader.maxTry = 300;
                }
                return mDefaultHeader;
            }
        }

        public string contentType { get; set; }

        public string accept { get; set; }

        public string userAgent { get; set; }

        public string method { get; set; }

        public int maxTry { get; set; }
    }
}
