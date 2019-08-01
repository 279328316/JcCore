
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Jc.Core.Helper
{
    /// <summary>
    /// 请求辅助Helper
    /// </summary>
    public class RobjRequestHelper
    {
        /// <summary>
        /// Post方式访问Url
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <param name="postDataStr">PostDataStr Json序列化</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postDataStr)
        {
            string result = null;
            try
            {
                byte[] dataArray = Encoding.UTF8.GetBytes(postDataStr);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = dataArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(dataArray, 0, dataArray.Length);
                dataStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = reader.ReadToEnd();
                reader.Close();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 下载资源文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="downLoadFilePath">下载文件路径 包含文件名</param>
        public static bool DownLoadRes(string url, string downLoadFilePath)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    string fileDir = Path.GetDirectoryName(downLoadFilePath);
                    if (!Directory.Exists(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AllowAutoRedirect = true;

                    WebResponse response = request.GetResponse();

                    using (Stream st = response.GetResponseStream())//获取服务器 文件流
                    {
                        //创建本地文件
                        Stream so = new FileStream(downLoadFilePath, FileMode.Create);
                        //进度条累加变量 = 已下载字节
                        long totalDownloadedByte = 0;
                        byte[] by = new byte[1024];
                        int osize = st.Read(by, 0, (int)by.Length);
                        //循环读取文件
                        while (osize > 0)
                        {
                            //累加
                            totalDownloadedByte = osize + totalDownloadedByte;
                            //写入文件
                            so.Write(by, 0, osize);
                            //读取服务器文件流
                            osize = st.Read(by, 0, (int)by.Length);
                        }
                        so.Close();
                        st.Close();
                        result = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return result;
        }

        //private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        //默认UserAgent
        private static readonly string defaultUserAgent = ".NET Framework App Client";

        //默认请求Header
        private static WebHeaderCollection defalutHeaders = new WebHeaderCollection();

        //设置请求编码
        private static Encoding requestEncoding = Encoding.UTF8;

        /// <summary>
        /// 设置默认Header
        /// </summary>
        private static void SetDefaultHeaders(IDictionary<string, string> headerParameters = null)
        {
            defalutHeaders.Clear();
            if (headerParameters != null && headerParameters.Count > 0)
            {
                foreach (KeyValuePair<string, string> param in headerParameters)
                {
                    defalutHeaders.Add(param.Key, param.Value);
                }
            }
        }

        /// <summary>
        /// 执行Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static Robj<T> Get<T>(string url, string paramsStr, IDictionary<string, string> headerParameters = null, int? timeout = 60000, string userAgent = null, CookieCollection cookies = null)
        {
            Robj<T> robj = new Robj<T>();
            try
            {
                // Display the contents of the page to the console.
                HttpWebResponse response = CreateGetHttpResponse(url + "?" + paramsStr, headerParameters, timeout, userAgent, cookies);
                string encoding = string.IsNullOrEmpty(response.ContentEncoding) ? response.CharacterSet : response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                //读取响应流
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string returnData = reader.ReadToEnd();
                robj = JsonHelper.DeserializeObject<Robj<T>>(returnData);
                reader.Dispose();
                response.Close();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
            return robj;
        }

        /// <summary>
        /// 同步执行Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requestEncoding"></param>
        /// <param name="parameters"></param>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="cookies"></param>
        /// <returns>Robj<T></returns>
        private static Robj<T> Post<T>(string url, IDictionary<string, string> parameters = null, HttpContentType contentType = HttpContentType.Json, IDictionary<string, string> headerParameters = null, int? timeout = 3000, string userAgent = null, CookieCollection cookies = null)
        {
            Robj<T> robj = new Robj<T>();
            try
            {
                HttpWebResponse response = CreatePostHttpResponse(url, parameters, contentType, headerParameters, timeout, userAgent, cookies);
                string encoding = string.IsNullOrEmpty(response.ContentEncoding) ? response.CharacterSet : response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                //读取响应流
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string returnData = reader.ReadToEnd();
                robj = JsonHelper.DeserializeObject<Robj<T>>(returnData);
                reader.Dispose();
                response.Close();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
            return robj;
        }

        /// <summary>
        /// 异步执行Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requestEncoding"></param>
        /// <param name="parameters"></param>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static Robj<T> Post<T>(string url, string parametersStr, HttpContentType contentType = HttpContentType.Json, IDictionary<string, string> headerParameters = null, int? timeout = 3000, string userAgent = null, CookieCollection cookies = null)
        {
            Robj<T> robj = new Robj<T>();
            try
            {
                HttpWebResponse response = CreatePostHttpResponse(url, parametersStr, contentType, headerParameters, timeout, userAgent, cookies);
                string encoding = string.IsNullOrEmpty(response.ContentEncoding) ? response.CharacterSet : response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                //读取响应流
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string returnData = reader.ReadToEnd();
                robj = JsonHelper.DeserializeObject<Robj<T>>(returnData);
                reader.Dispose();
                response.Close();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
            return robj;
        }
        
        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns></returns>
        private static HttpWebResponse CreateGetHttpResponse(string url, IDictionary<string, string> headerParameters = null, int? timeout = 60000, string userAgent = null, CookieCollection cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = defaultUserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }

            if (headerParameters != null)
            {
                foreach (KeyValuePair<string, string> param in headerParameters)
                {
                    request.Headers.Add(param.Key, param.Value);
                }
            }
            else
            {
                request.Headers = defalutHeaders;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 创建POST方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns></returns>
        private static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters = null, HttpContentType contentType = HttpContentType.Json, IDictionary<string, string> headerParameters = null, int? timeout = 3000, string userAgent = null, CookieCollection cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //如果是发送HTTPS请求
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            if (headerParameters != null)
            {
                foreach (KeyValuePair<string, string> param in headerParameters)
                {
                    request.Headers.Add(param.Key, param.Value);
                }
            }
            else
            {
                request.Headers = defalutHeaders;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = defaultUserAgent;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = requestEncoding.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 创建POST方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns></returns>
        private static HttpWebResponse CreatePostHttpResponse(string url, string parametersStr = null, HttpContentType contentType = HttpContentType.Json, IDictionary<string, string> headerParameters = null, int? timeout = 3000, string userAgent = null, CookieCollection cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //如果是发送HTTPS请求
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            if (headerParameters != null)
            {
                foreach (KeyValuePair<string, string> param in headerParameters)
                {
                    request.Headers.Add(param.Key, param.Value);
                }
            }
            else
            {
                request.Headers = defalutHeaders;
            }
            request.Method = "POST";
            if (contentType == HttpContentType.Json)
            {
                request.Accept = "application/json";
                request.ContentType = "application/json; charset=utf-8";
            }
            else if (contentType == HttpContentType.Form)
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = defaultUserAgent;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据
            byte[] data = requestEncoding.GetBytes(parametersStr);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string OpenReadWithHttps(string url, string postData)
        {
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "post";
            request.Headers.Add("access_key", "your access_key");
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=utf-8";
            byte[] buffer = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 返回Json数据
        /// </summary>
        /// <param name="jsonData">要处理的JSON数据</param>
        /// <param name="url">要提交的URL</param>
        /// <returns>返回的JSON处理字符串</returns>
        public string GetResponseData(string url, string jsonData)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(jsonData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentLength = bytes.Length;
            request.ContentType = "application/json;charset=UTF-8";
            Stream reqstream = request.GetRequestStream();
            reqstream.Write(bytes, 0, bytes.Length);

            //声明一个HttpWebRequest请求
            request.Timeout = 90000;
            //设置连接超时时间
            request.Headers.Set("Pragma", "no-cache");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamReceive = response.GetResponseStream();
            Encoding encoding = Encoding.UTF8;

            StreamReader streamReader = new StreamReader(streamReceive, encoding);
            string strResult = streamReader.ReadToEnd();
            streamReceive.Dispose();
            streamReader.Dispose();

            return strResult;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }
    }


    /// <summary>
    /// 类型ContentType
    /// </summary>
    public enum HttpContentType
    {
        /// <summary>
        /// Json
        /// </summary>
        Json = 1,
        /// <summary>
        /// Form
        /// </summary>
        Form = 2
    }

}
