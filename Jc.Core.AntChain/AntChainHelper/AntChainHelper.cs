
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Jc.Core.AntChain
{
    /// <summary>
    /// AntChainHelper
    /// </summary>
    public class AntChainHelper
    {
        #region Fields&Properties

        private string accessToken = null;

        /// <summary>
        /// 开放联盟链的链 ID
        /// </summary>
        public string Bizid { get { return "a00e36c5"; } }

        /// <summary>
        /// 加密算法，EC 表示采用椭圆曲线数字签名算法
        /// </summary>
        public string CipherSuit { get { return "ec"; } }

        /// <summary>
        /// ApiUrl
        /// </summary>
        public string ApiUrl { get { return "https://rest.baas.alipay.com/"; } }
        
        /// <summary>
        /// Config
        /// </summary>
        public AntChainConfig Config { get; set; }

        /// <summary>
        /// AccessToken 有效期为30分钟
        /// </summary>
        public string AccessToken
        {
            get
            {
                if (DateTime.Now.AddSeconds(60) >= AccessTokenExpireTime)
                {   //还有1分钟到期,刷新AccessToken
                    RefreshAccessToken();
                }
                return accessToken;
            }
        }

        public DateTime AccessTokenExpireTime { get; private set; } = DateTime.Now;

        #endregion


        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public AntChainHelper(AntChainConfig config)
        {
            this.Config = config;
            RefreshAccessToken();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public AntChainHelper(string accessId, string accessKey, string tenantId, string account, string mykmsKeyId)
        {
            Config = new AntChainConfig();
            Config.AccessId = accessId;
            Config.AccessKey = accessKey;
            Config.TenantId = tenantId;
            Config.Account = account;
            Config.MykmsKeyId = mykmsKeyId;
            RefreshAccessToken();
        }

        #endregion

        #region Methods

        #region 授权Methods

        /// <summary>
        /// 刷新AccessToken
        /// </summary>
        private void RefreshAccessToken()
        {
            string url = $"{ApiUrl}api/contract/shakeHand";

            DateTime dt = DateTime.Now;
            long time = ConvertDateTime(dt);
            string secret = CertUtil.Sign($"{Config.AccessId}{time}", Config.AccessKey);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("accessId", Config.AccessId);
            dic.Add("time", time);
            dic.Add("secret", secret);
            AntChainRequestResult result = Post<AntChainRequestResult>(url, dic);
            if (!result.Success)
            {
                throw new Exception($"Error Code :{result.Code}");
            }
            accessToken = result.Data;
            AccessTokenExpireTime = DateTime.Now.AddSeconds(30 * 60);
        }

        /// <summary>  
        /// 将DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        private long ConvertDateTime(DateTime time)
        {
            DateTime startTime = System.TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
        #endregion

        /// <summary>
        /// DeploySolidity合约交易
        /// </summary>
        public AntChainRequestResult DeploySolidityContract(Method method, string orderId, string contractName, string methodSignature, string paramListStr, string outTypes = "[\"string\"]")
        {
            string url = $"{ApiUrl}api/contract/chainCallForBiz";

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("orderId", orderId);
            dic.Add("bizid", Bizid);
            dic.Add("account", Config.Account);
            dic.Add("contractName", contractName);
            dic.Add("methodSignature", methodSignature);
            dic.Add("mykmsKeyId", Config.MykmsKeyId);
            dic.Add("method", method.ToString());
            dic.Add("inputParamListStr", paramListStr);
            dic.Add("outTypes", outTypes);
            dic.Add("accessId", Config.AccessId);
            dic.Add("token", AccessToken);
            dic.Add("gas", 10000000);
            dic.Add("tenantid", Config.TenantId);
            AntChainRequestResult result = Post<AntChainRequestResult>(url, dic);
            return result;
        }

        /// <summary>
        /// 执行Solidity合约交易
        /// </summary>
        public AntChainRequestResult CallSolidityContract(string orderId, string contractName, string methodSignature, string paramListStr, string outTypes = "[\"string\"]")
        {
            string url = $"{ApiUrl}api/contract/chainCallForBiz";

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("orderId", orderId);
            dic.Add("bizid", Bizid);
            dic.Add("account", Config.Account);
            dic.Add("contractName", contractName);
            dic.Add("methodSignature", methodSignature);
            dic.Add("mykmsKeyId", Config.MykmsKeyId);
            dic.Add("method", Method.CALLCONTRACTBIZASYNC.ToString());
            dic.Add("inputParamListStr", paramListStr);
            dic.Add("outTypes", outTypes);
            dic.Add("accessId", Config.AccessId);
            dic.Add("token", AccessToken);
            dic.Add("gas", 10000000);
            dic.Add("tenantid", Config.TenantId);
            AntChainRequestResult result = Post<AntChainRequestResult>(url, dic);
            return result;
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
        public T Post<T>(string url, object requestParams = null)
        {
            T result = default(T);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json;charset=utf-8;";

            //如果需要Post数据
            if (requestParams != null)
            {
                string paramStr = JsonSerializer.Serialize(requestParams);
                if (!string.IsNullOrEmpty(paramStr))
                {
                    byte[] data = Encoding.UTF8.GetBytes(paramStr);
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
            WebResponse response = request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string resultString = reader.ReadToEnd();
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                result = JsonSerializer.Deserialize<T>(resultString, options);
            }
            return result;
        }
        #endregion
    }
}