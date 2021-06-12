using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Jc.AntChain
{
    public class CertUtil
    {
        public static string Sign(string contentForSign, string privateKey)
        {
            //转换秘钥
            string netKey = GetRSAPrivateKey(privateKey);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(netKey);
            //创建一个空对象
            RSACryptoServiceProvider rsaClear = new RSACryptoServiceProvider();
            RSAParameters paras = rsa.ExportParameters(true);
            rsaClear.ImportParameters(paras);
            //签名返回
            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                byte[] signData = rsa.SignData(Encoding.UTF8.GetBytes(contentForSign), sha256);
                return BytesToHex(signData);
            }
        }

        public static string BytesToHex(byte[] data)
        {
            StringBuilder sbRet = new StringBuilder(data.Length * 2);
            for (int i = 0; i < data.Length; i++)
            {
                sbRet.Append(Convert.ToString(data[i], 16).PadLeft(2, '0'));
            }
            return sbRet.ToString();
        }

        public static string GetRSAPrivateKey(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        }
    }
}
