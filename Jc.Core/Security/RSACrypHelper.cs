using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;

using Jc.Core.Helper;

namespace Jc.Core.Security
{
    public class RSACrypHelper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Encode(string content, string publicToken = null)
        {
            string result = "";
            string publicKey = "";
            if (string.IsNullOrEmpty(publicToken))
            {
                if (ConfigurationManager.AppSettings["PublicToken"] != null)
                {
                    publicKey = ConfigurationManager.AppSettings["PublicToken"];
                    publicKey = EncodeHelper.Base64StringToString(publicKey);
                }
            }
            else
            {
                publicKey = EncodeHelper.Base64StringToString(publicToken); ;
            }
            RSACryptoServiceProvider myrsa = new RSACryptoServiceProvider();
            myrsa.FromXmlString(publicKey);
            byte[] mybyte = Encoding.Unicode.GetBytes(content);
            byte[] myency = myrsa.Encrypt(mybyte, true);
            result = Convert.ToBase64String(myency);
            return result;
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string DeEncode(string content, string masterToken = null)
        {
            string result = "";
            string masterKey = "";
            if (string.IsNullOrEmpty(masterToken))
            {
                if (ConfigurationManager.AppSettings["MasterToken"] != null)
                {
                    masterKey = ConfigurationManager.AppSettings["MasterToken"];
                    masterKey = EncodeHelper.Base64StringToString(masterKey);
                }
            }
            else
            {
                masterKey = EncodeHelper.Base64StringToString(masterToken);
            }
            RSACryptoServiceProvider myrsa = new RSACryptoServiceProvider();
            myrsa.FromXmlString(masterKey);
            byte[] mdec = Convert.FromBase64String(content);
            byte[] mybyte = myrsa.Decrypt(mdec, true);
            result = Encoding.Unicode.GetString(mybyte);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> GenerateToken()
        {
            string publicToken = "";
            string masterToken = "";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            publicToken = EncodeHelper.StringToBase64String(rsa.ToXmlString(false));
            masterToken = EncodeHelper.StringToBase64String(rsa.ToXmlString(true));
            KeyValuePair<string, string> result = new KeyValuePair<string, string>(publicToken, masterToken);
            return result;
        }
    }
}
