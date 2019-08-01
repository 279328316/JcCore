using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace Jc.Core.Helper
{
    /// <summary>
    /// 有关HTTP请求的辅助类
    /// </summary>
    public class HttpHelper
    {
        #region Properties
        /// <summary>
        /// 请求编码
        /// </summary>
        public Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 获取或设置BaseUrl
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// 超时时间 默认60S
        /// </summary>
        public int Timeout { get; set; } = 60000;

        /// <summary>
        /// 用户代理
        /// </summary>
        public string UserAgent { get; set; } = ".Net App Client";

        /// <summary>
        /// Cookie
        /// </summary>
        public CookieCollection Cookies { get; set; } = new CookieCollection();

        /// <summary>
        /// 获取或设置 默认Headers
        /// </summary>
        public WebHeaderCollection Headers { get; set; } = new WebHeaderCollection();
        #endregion

        #region SetMethods

        /// <summary>
        /// Ctor
        /// </summary>
        public HttpHelper()
        {
        }

        /// <summary>
        /// 设置urlPrefix url前缀
        /// </summary>
        /// <param name="prefixUrl"></param>
        public HttpHelper SetBaseUrl(string prefixUrl)
        {
            BaseUrl = prefixUrl;
            return this;
        }

        /// <summary>
        /// 设置Header
        /// </summary>
        public HttpHelper SetHeaders(IDictionary<string, string> headerParams = null)
        {
            Headers.Clear();
            if (headerParams != null && headerParams.Count > 0)
            {
                foreach (KeyValuePair<string, string> param in headerParams)
                {
                    Headers.Add(param.Key, param.Value);
                }
            }
            return this;
        }

        /// <summary>
        /// 设置Header
        /// </summary>
        public HttpHelper SetHeaders(string key,string value)
        {
            if (!Headers.AllKeys.Contains(key))
            {
                Headers.Add(key, value);
            }
            else
            {
                Headers.Set(key, value);
            }
            return this;
        }
        #endregion

        #region Get请求

        /// <summary>
        /// 执行Get请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求URL</param>
        /// <param name="requestParams">请求参数</param>
        /// <returns></returns>
        public T Get<T>(string url, object requestParams = null)
        {
            string resultString = Get(url, requestParams);
            return JsonHelper.DeserializeObject<T>(resultString);
        }

        /// <summary>
        /// 执行Get请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求URL</param>
        /// <param name="requestParams">请求参数</param>
        /// <returns></returns>
        public AfterDto<T> GetAsync<T>(string url, object requestParams = null, IDictionary<string, string> headerParams = null, int? timeout = 60000, string userAgent = null, CookieCollection cookies = null)
        {
            AfterDto<T> afterDto = null;
            Task t = new Task(() =>
            {
                string resultString = Get(url, requestParams);
                if (typeof(T) == typeof(string))
                {
                    T result = (T)Convert.ChangeType(resultString, typeof(T));
                    afterDto.DoAfter(result);
                }
                else
                {
                    afterDto.DoAfter(JsonHelper.DeserializeObject<T>(resultString));
                }
            });
            t.Start();
            return afterDto;
        }

        /// <summary>
        /// 执行Get请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="requestParams">请求参数</param>
        /// <returns></returns>
        public string Get(string url, object requestParams = null)
        {
            string result = "";
            try
            {
                HttpWebResponse response = CreateGetHttpResponse(url, requestParams);
                string encoding = string.IsNullOrEmpty(response.ContentEncoding) ? response.CharacterSet : response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                //读取响应流
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                result = reader.ReadToEnd();
                reader.Dispose();
                response.Close();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
            return result;
        }

        #endregion

        #region Post请求

        /// <summary>
        /// 执行Post请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求的url</param>
        /// <param name="requestParams">请求参数</param>
        /// <param name="contentType">请求内容类型</param>
        /// <returns></returns>
        public T Post<T>(string url, object requestParams = null, HttpContentType contentType = HttpContentType.Json)
        {
            string resultString = Post(url, requestParams, contentType);
            return JsonHelper.DeserializeObject<T>(resultString);
        }

        /// <summary>
        /// 执行Post请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求url</param>
        /// <param name="requestParams">请求参数</param>
        /// <param name="contentType">请求内容类型</param>
        /// <returns></returns>
        public AfterDto<T> PostAsync<T>(string url, object requestParams = null, HttpContentType contentType = HttpContentType.Json)
        {
            AfterDto<T> afterDto = null;
            Task t = new Task(() =>
            {
                string resultString = Post(url, requestParams, contentType);
                if (typeof(T) == typeof(string))
                {
                    T result = (T)Convert.ChangeType(resultString, typeof(T));
                    afterDto.DoAfter(result);
                }
                else
                {
                    afterDto.DoAfter(JsonHelper.DeserializeObject<T>(resultString));
                }
            });
            t.Start();
            return afterDto;
        }

        /// <summary>
        /// 执行Post请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="requestParams">请求参数</param>
        /// <param name="contentType">请求内容类型</param>
        /// <returns></returns>
        public string Post(string url, object requestParams = null, HttpContentType contentType = HttpContentType.Json)
        {
            string result = "";
            try
            {
                HttpWebResponse response = CreatePostHttpResponse(url, requestParams, contentType);
                string encoding = string.IsNullOrEmpty(response.ContentEncoding) ? response.CharacterSet : response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                //读取响应流
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                result = reader.ReadToEnd();
                reader.Dispose();
                response.Close();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
            return result;
        }

        #endregion


        #region UploadFile请求

        /// <summary>
        /// 执行UploadFile请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求url</param>
        /// <param name="requestParams">请求参数</param>
        /// <param name="fileList">文件路径列表</param>
        /// <returns></returns>
        public T UploadFile<T>(string url, object requestParams = null,List<string> fileList = null)
        {
            string resultString = UploadFile(url, requestParams, fileList);
            return JsonHelper.DeserializeObject<T>(resultString);
        }
        /// <summary>
        /// 执行UploadFile请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求url</param>
        /// <param name="requestParams">请求参数</param>
        /// <param name="fileList">文件路径列表</param>
        /// <returns></returns>
        public AfterDto<T> UploadFileAsync<T>(string url, object requestParams = null, List<string> fileList = null)
        {
            AfterDto<T> afterDto = null;
            Task t = new Task(() =>
            {
                string resultString = UploadFile(url, requestParams, fileList);
                if (typeof(T) == typeof(string))
                {
                    T result = (T)Convert.ChangeType(resultString, typeof(T));
                    afterDto.DoAfter(result);
                }
                else
                {
                    afterDto.DoAfter(JsonHelper.DeserializeObject<T>(resultString));
                }
            });
            t.Start();
            return afterDto;
        }

        /// <summary>
        /// 执行UploadFile请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求url</param>
        /// <param name="requestParams">请求参数</param>
        /// <param name="fileList">文件路径列表</param>
        /// <returns></returns>
        public string UploadFile(string url, object requestParams = null, List<string> fileList = null)
        {
            string result = "";
            try
            {
                HttpWebResponse response = CreateUploadFileHttpResponse(url, requestParams, fileList);
                string encoding = string.IsNullOrEmpty(response.ContentEncoding) ? response.CharacterSet : response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                //读取响应流
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                result = reader.ReadToEnd();
                reader.Dispose();
                response.Close();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
            return result;
        }
        #endregion

        #region 创建请求
        /// <summary>
        /// 创建GET方式的HTTP请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="requestParams">请求参数</param>
        /// <returns></returns>
        public HttpWebResponse CreateGetHttpResponse(string url,object requestParams = null)
        {
            if (!url.ToLower().StartsWith("http") && !string.IsNullOrEmpty(BaseUrl))
            {
                url = BaseUrl + url;
            }
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            string paramStr = GetParameterString(requestParams);
            if(!string.IsNullOrEmpty(paramStr))
            {
                url += (url.IndexOf('?') <= -1 ? "?" : "&") + paramStr;
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //如果是发送HTTPS请求
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request.ProtocolVersion = HttpVersion.Version10;
                request.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            request.Method = "GET";
            #region 设置请求Header
            request.UserAgent = UserAgent;

            if (Headers != null && Headers.Count > 0)
            {
                foreach (string key in Headers.AllKeys)
                {
                    request.Headers.Add(key, Headers[key]);
                }
            }
            request.Timeout = Timeout;
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(Cookies);
            #endregion
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 创建Post方式的HTTP请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="requestParams">请求参数</param>
        /// <param name="contentType">请求内容类型</param>
        /// <returns></returns>
        public HttpWebResponse CreatePostHttpResponse(string url,object requestParams = null, HttpContentType contentType = HttpContentType.Json)
        {
            if (!url.ToLower().StartsWith("http") && !string.IsNullOrEmpty(BaseUrl))
            {
                url = BaseUrl + url;
            }
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;            
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {   //如果是发送HTTPS请求
                request.ProtocolVersion = HttpVersion.Version10;
                request.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            request.Method = "POST";
            if (contentType == HttpContentType.Json)
            {
                request.Accept = "application/json";
                request.ContentType = "application/json;charset=utf-8;";
            }
            else if (contentType == HttpContentType.Form)
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }

            request.UserAgent = UserAgent;

            if (Headers != null && Headers.Count > 0)
            {
                foreach (string key in Headers.AllKeys)
                {
                    request.Headers.Add(key, Headers[key]);
                }
            }
            request.Timeout = Timeout;
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(Cookies);

            //如果需要Post数据
            if (requestParams != null)
            {
                string paramStr = null;
                if (contentType == HttpContentType.Json)
                {
                    paramStr = JsonHelper.ObjToJson(requestParams);
                }
                else
                {
                    paramStr = GetParameterString(requestParams);
                }
                if (!string.IsNullOrEmpty(paramStr))
                {
                    byte[] data = DefaultEncoding.GetBytes(paramStr);
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }


        /// <summary>
        /// 创建UploadFile请求
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="requestParams">请求参数</param>
        /// <param name="fileList">文件路径列表</param>
        /// <returns></returns>
        public HttpWebResponse CreateUploadFileHttpResponse(string url, object requestParams = null,List<string> fileList = null)
        {
            if (!url.ToLower().StartsWith("http") && !string.IsNullOrEmpty(BaseUrl))
            {
                url = BaseUrl + url;
            }
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {   //如果是发送HTTPS请求
                request.ProtocolVersion = HttpVersion.Version10;
                request.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }

            request.UserAgent = UserAgent;

            if (Headers != null && Headers.Count > 0)
            {
                foreach (string key in Headers.AllKeys)
                {
                    request.Headers.Add(key, Headers[key]);
                }
            }
            request.Timeout = Timeout;
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(Cookies);

            request.Method = "POST";

            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
            request.ContentType = "multipart/form-data;boundary=" + boundary;   //请求类型
            request.AllowWriteStreamBuffering = false;  //对发送的数据不使用缓存

            // 最后的结束符  
            byte[] endBoundary = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            //文件数据
            string fileFormdataTemplate =
                "\r\n--" + boundary +
                "\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{0}\"" +
                "\r\nContent-Type: application/octet-stream\r\n\r\n";

            //文本数据 
            string dataFormdataTemplate =
                "\r\n--" + boundary +
                "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                "\r\n\r\n{1}";

            #region 上传数据 文件

            if (requestParams!=null || fileList != null && fileList.Count > 0)
            {
                using (Stream stream = request.GetRequestStream())
                {
                    //如果需要Post数据
                    if (requestParams!=null)
                    {
                        #region 写入其他表单参数
                        IDictionary<string, object> items = GetParameterDictionary(requestParams);
                        foreach (KeyValuePair<string, object> key in items)
                        {
                            var p = string.Format(dataFormdataTemplate, key.Key, key.Value);
                            var bp = Encoding.UTF8.GetBytes(p);
                            stream.Write(bp, 0, bp.Length);
                        }
                        #endregion
                    }
                    if (fileList != null && fileList.Count > 0)
                    {
                        for (int i = 0; i < fileList.Count; i++)
                        {
                            if (!File.Exists(fileList[i]))
                            {
                                throw new Exception($"文件不存在:{fileList[i]}");
                            }
                            string strPostHeader = string.Format(fileFormdataTemplate, fileList[i]);
                            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);
                            stream.Write(postHeaderBytes, 0, postHeaderBytes.Length);  //把头部转为数据流放入到请求流中去
                            
                            // 将文件流转为数据流放入到请求流中去
                            using (FileStream fileStream = new FileStream(fileList[i], FileMode.Open, FileAccess.Read))
                            {
                                byte[] buffur = new byte[fileStream.Length];
                                fileStream.Read(buffur, 0, (int)fileStream.Length);
                                stream.Write(buffur, 0, buffur.Length);
                            }
                        }
                        //结尾加上结束分隔符
                        byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                        stream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);  
                    }
                }
            }
            #endregion
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 检查证书 暂未使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            bool result = true;
            if (errors != SslPolicyErrors.None)
            {   //暂时不验证
                //result = false;
            }
            return result;
            //return true; //总是接受
        }
        #endregion


        /// <summary>
        /// 构造Form请求参数String
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <param name="requestParams">参数 IDictionary,IEnumerable,string,object</param>
        /// <returns>a=a1&amp;b=b1</returns>
        public string GetParameterString(object requestParams)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (requestParams != null)
            {
                if (requestParams is IDictionary<string, object>)
                {
                    #region 处理IDictionary<string, object>类型数据
                    IDictionary<string, object> dic = requestParams as IDictionary<string, object>;
                    foreach (string key in dic.Keys)
                    {
                        if (strBuilder.Length <= 0)
                        {
                            strBuilder.AppendFormat("{0}={1}", key, dic[key]);
                        }
                        else
                        {
                            strBuilder.AppendFormat("&{0}={1}", key, dic[key]);
                        }
                    }
                    #endregion
                }
                else if (requestParams is IEnumerable<object>)
                {
                    #region 处理数组类型数据
                    IEnumerable<object> list = requestParams as IEnumerable<object>;

                    if (list.Count() > 0)
                    {
                        if (list.First() is KeyValuePair<string, object>)
                        {
                            #region 处理List<KeyValuePair<string, object>> 类型数据
                            foreach (KeyValuePair<string, object> kv in list)
                            {
                                if (strBuilder.Length > 0)
                                {
                                    strBuilder.AppendFormat("&");
                                }
                                strBuilder.AppendFormat("{0}={1}", kv.Key, kv.Value);
                            }
                            #endregion
                        }
                        else
                        {
                            strBuilder.Append(JsonHelper.ObjToJson(requestParams));
                        }
                    }
                    #endregion
                }
                else if (requestParams is ValueType || requestParams is string)
                {
                    strBuilder.Append(requestParams);
                }
                else
                {
                    #region 处理Obj类型数据
                    List<PropertyInfo> piList = requestParams.GetType().GetProperties()
                        .Where(pi => pi.CanRead).ToList();
                    if (piList != null && piList.Count > 0)
                    {
                        for (int i = 0; i < piList.Count; i++)
                        {
                            string valueStr;
                            object value = piList[i].GetValue(requestParams);
                            if (value is ValueType || value is string)
                            {
                                valueStr = value?.ToString();
                            }
                            else
                            {
                                valueStr = JsonHelper.ObjToJson(value);
                            }
                            if (strBuilder.Length > 0)
                            {
                                strBuilder.AppendFormat("&");
                            }
                            strBuilder.AppendFormat("{0}={1}", piList[i].Name.ToLower(), valueStr);
                        }
                    }
                    #endregion
                }
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 构造请求参数IDictionary&lt;string,object&gt;
        /// 可接受IDictionary&lt;string,object&gt;,IEnumerable&lt;keyvaluePair&lt;string,object&gt;&gt;,
        /// IEnumerable&lt;object&gt;,string(a=a1&amp;b=b1),object等类型参数
        /// </summary>
        /// <param name="requestParams">参数 IDictionary,IEnumerable,string,object</param>
        /// <returns></returns>
        public IDictionary<string, object> GetParameterDictionary(object requestParams)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>();
            if (requestParams != null)
            {
                if (requestParams is IDictionary<string, object>)
                {   //处理IDictionary<string, object>类型数据
                    dic = requestParams as IDictionary<string, object>;
                }
                else if (requestParams is IEnumerable<object>)
                {
                    #region 处理数组类型数据
                    IEnumerable<object> list = requestParams as IEnumerable<object>;

                    if (list.Count() > 0)
                    {
                        if (list.First() is KeyValuePair<string, object>)
                        {   //处理List<KeyValuePair<string, object>> 类型数据
                            foreach (KeyValuePair<string, object> kv in list)
                            {
                                dic.Add(kv.Key, kv.Value);
                            }
                        }
                        else
                        {
                            dic.Add("params", JsonHelper.ObjToJson(requestParams));
                        }
                    }
                    #endregion
                }
                else if (requestParams is ValueType)
                {
                    dic.Add("params", requestParams);
                }
                else if (requestParams is string)
                {
                    string paramStr = requestParams.ToString();
                    if (paramStr.Contains("="))
                    {   //处理 a=value1&b=value2 格式
                        string[] paramArray = paramStr.Split('&');
                        foreach(string str in paramArray)
                        {
                            if(string.IsNullOrEmpty(str))
                            {   //忽略 空字符
                                continue;
                            }
                            int index = str.IndexOf('=');
                            if(index!=-1)
                            {
                                string key = str.Substring(0, index);
                                string value = null;
                                if(index < str.Length - 1)
                                {   //=不是最后一个字符,返回数据;
                                    value = str.Substring(index + 1);
                                }
                                dic.Add(key, value);
                            }
                        }
                    }
                    else
                    {
                        dic.Add("params", requestParams);
                    }
                }
                else
                {
                    #region 处理Obj类型数据
                    List<PropertyInfo> piList = requestParams.GetType().GetProperties()
                        .Where(pi => pi.CanRead).ToList();
                    if (piList != null && piList.Count > 0)
                    {
                        for (int i = 0; i < piList.Count; i++)
                        {
                            string valueStr;
                            object value = piList[i].GetValue(requestParams);
                            if (value is ValueType || value is string)
                            {
                                valueStr = value?.ToString();
                            }
                            else
                            {
                                valueStr = JsonHelper.ObjToJson(value);
                            }
                            dic.Add(piList[i].Name.ToLower(), valueStr);
                        }
                    }
                    #endregion
                }
            }
            return dic;
        }
        /// <summary>
        /// 注册CodePagesEncoding 解决中文乱码问题
        /// </summary>
        public static void RegisterEncodingProvider()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}