using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Jc.Security
{
    /// <summary>
    /// AesHelper
    /// </summary>
    public class AesHelper
    {
        /// <summary>
        /// Create Aes
        /// </summary>
        /// <param name="key"> 密码,长度为16倍数的字符串 如果启用HashKey,则不需要强制为16位的倍数 本方法中使用32位</param>
        /// <param name="iv">偏移量,长度为16的字符串,如果启用HashKey,则不需要强制为16位,如果为空,默认使用key</param>
        /// <param name="useHashKey">启用HashKey,对Key和Iv进行Hash256计算,取其前指定位数的字符串作为hashKey</param>
        /// <returns></returns>
        public static Aes CreateAes(string key, string iv = null, bool useHashKey = true)
        {
            Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.BlockSize = 128;

            string hashKey = GetSHA256Hash(key, 32);
            string hashiv;
            if (!string.IsNullOrEmpty(iv))
            {
                hashiv = GetSHA256Hash(iv, 16);
            }
            else
            {
                hashiv = GetSHA256Hash(key, 16);
            }
            aes.Key = Encoding.UTF8.GetBytes(hashKey);
            aes.IV = Encoding.UTF8.GetBytes(hashiv);
            return aes;
        }

        /// <summary>
        /// 获取字符串SHA256Hash
        /// </summary>
        /// <param name="str"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static string GetSHA256Hash(string str, int size = 32)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                byte[] hashedBytes = sha256.ComputeHash(bytes);

                // 取前32字节作为AES算法的秘钥
                string hash = Convert.ToBase64String(hashedBytes).Substring(0, size).PadLeft(size, '0');
                return hash;
            }
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="str">待加密的字符串</param>
        /// <param name="key">加密密钥,和加密密钥相同</param>
        /// <param name="iv">加密偏移向量,和加密偏移向量相同,为空则使用Key进行处理</param>
        /// <param name="useHashKey">启用HashKey,对Key和Iv进行Hash256计算,取其前指定位数的字符串作为hashKey</param>
        /// <returns>加密成功返回加密后的字符串，失败返源串</returns>
        public static string Encrypt(string str, string key, string iv = null, bool useHashKey = true)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(key))
            {
                return str;
            }
            string encryptString;

            Aes aes = CreateAes(key, iv, useHashKey);
            ICryptoTransform encryptor = aes.CreateEncryptor();
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(str);
                    }
                    encryptString = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            return encryptString;
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="str">待解密的字符串</param>
        /// <param name="key">解密密钥,和加密密钥相同</param>
        /// <param name="iv">解密偏移向量,和加密偏移向量相同,为空则使用Key进行处理</param>
        /// <param name="useHashKey">启用HashKey,对Key和Iv进行Hash256计算,取其前指定位数的字符串作为hashKey</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string Decrypt(string str, string key, string iv = null, bool useHashKey = true)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(key))
            {
                return str;
            }
            string decryptString;
            Aes aes = CreateAes(key, iv, useHashKey);
            ICryptoTransform decryptor = aes.CreateDecryptor();
            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(str)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        decryptString = srDecrypt.ReadToEnd();
                    }
                }
            }
            return decryptString;
        }

        private const ulong FileType_Tag = 0xAC010203040506CA;
        private const int Buffer_Size = 128 * 1024;

        /// <summary>
        /// 加密文件随机数生成
        /// </summary>
        private static RandomNumberGenerator rand = RandomNumberGenerator.Create();

        /// <summary>
        /// 生成指定长度的随机Byte数组
        /// </summary>
        /// <param name="count">Byte数组长度</param>
        /// <returns>随机Byte数组</returns>
        private static byte[] GenerateRandomBytes(int count)
        {
            byte[] bytes = new byte[count];
            rand.GetBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="sourceFile">待加密的文件</param>
        /// <param name="targetFile">加密后的文件</param>
        /// <param name="key">加密密钥,和加密密钥相同</param>
        /// <param name="iv">加密偏移向量,和加密偏移向量相同,为空则使用Key进行处理</param>
        /// <param name="useHashKey">启用HashKey,对Key和Iv进行Hash256计算,取其前指定位数的字符串作为hashKey</param>
        /// <returns>加密成功返回加密后的字符串，失败返源串</returns>
        public static void EncryptFile(string sourceFile, string targetFile, string key, string iv = null, bool useHashKey = true)
        {
            if (string.IsNullOrEmpty(sourceFile))
            {
                throw new Exception("待加密文件不能为空");
            }
            if (string.IsNullOrEmpty(targetFile))
            {
                throw new Exception("目标文件路径不能为空");
            }
            if (!File.Exists(sourceFile))
            {
                throw new Exception("待加密文件不存在");
            }
            if (File.Exists(targetFile))
            {
                throw new Exception("目标文件已存在");
            }
            FileInfo fileInfo = new FileInfo(sourceFile);
            if (fileInfo.Length <= 0)
            {
                throw new Exception("待加密文件大小为0");
            }

            //打开输入输出文件流
            using (FileStream fsource = File.OpenRead(sourceFile))
            using (FileStream fout = File.OpenWrite(targetFile))
            {
                // 创建加密对象
                Aes aes = CreateAes(key, iv, useHashKey);
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                using (CryptoStream cout = new CryptoStream(fout, encryptor, CryptoStreamMode.Write))
                using (BinaryWriter bw = new BinaryWriter(cout))
                {
                    //使用加密流写入标签
                    bw.Write(FileType_Tag);      // 写入文件类型标记
                    bw.Write(fsource.Length);    // 写入原始文件的长度

                    int read; // 输入文件读取数量
                    byte[] buffer = new byte[4096]; // 缓冲区大小
                    //将源文件写入加密流和散列流
                    while ((read = fsource.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        cout.Write(buffer, 0, read);
                    }

                    // 计算解密文件的哈希值
                    using (SHA256 hasher = SHA256.Create())
                    {
                        fsource.Seek(0, SeekOrigin.Begin); // 重置文件指针到开始位置
                        byte[] hash = hasher.ComputeHash(fsource);

                        // 将散列写入加密文件的末尾
                        cout.Write(hash, 0, hash.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="sourceFile">待解密的文件</param>
        /// <param name="targetFile">解密后的文件</param>
        /// <param name="key">解密密钥,和加密密钥相同</param>
        /// <param name="iv">解密偏移向量,和加密偏移向量相同,为空则使用Key进行处理</param>
        /// <param name="useHashKey">启用HashKey,对Key和Iv进行Hash256计算,取其前指定位数的字符串作为hashKey</param>
        /// <returns></returns>
        public static void DecryptFile(string sourceFile, string targetFile, string key, string iv = null, bool useHashKey = true)
        {
            if (string.IsNullOrEmpty(sourceFile))
            {
                throw new Exception("待解密文件不能为空");
            }
            if (string.IsNullOrEmpty(targetFile))
            {
                throw new Exception("目标文件路径不能为空");
            }
            if (!File.Exists(sourceFile))
            {
                throw new Exception("待解密文件不存在");
            }
            if (File.Exists(targetFile))
            {
                throw new Exception("目标文件已存在");
            }
            FileInfo fileInfo = new FileInfo(sourceFile);
            if (fileInfo.Length <= 0)
            {
                throw new Exception("待解密文件大小为0");
            }

            //打开输入输出文件流
            using (FileStream fsource = File.OpenRead(sourceFile))
            using (FileStream fout = File.Create(targetFile))
            {
                // 创建Aes对象
                Aes aes = CreateAes(key, iv, useHashKey);

                //解密CryptoStream
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                using (CryptoStream cin = new CryptoStream(fsource, decryptor, CryptoStreamMode.Read))
                using (BinaryReader br = new BinaryReader(cin))
                {
                    //使用解密流解密
                    ulong tag = br.ReadUInt64();
                    long fileSize = br.ReadInt64();

                    if (FileType_Tag != tag)
                        throw new Exception("非Aes加密文件或文件被破坏");

                    long totalRead = 0; //当前获取大小
                    byte[] buffer = new byte[4096]; //临时缓存
                    int read;

                    // 读取并解密文件数据
                    while (totalRead < fileSize && (read = cin.Read(buffer, 0, Math.Min(buffer.Length, (int)(fileSize - totalRead)))) > 0)
                    {
                        fout.Write(buffer, 0, read);
                        totalRead += read;
                    }

                    using (SHA256 hasher = SHA256.Create())
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        // 计算解密文件的哈希值
                        fout.Seek(0, SeekOrigin.Begin); // 重置文件指针到开始位置
                        byte[] currentHash = hasher.ComputeHash(fout);

                        // 读取存储在文件末尾的哈希值（存在需要多次读取的情况）
                        byte[] storedHash = new byte[hasher.HashSize / 8];
                        while ((read = cin.Read(storedHash, 0, storedHash.Length)) > 0)
                        {
                            memoryStream.Write(storedHash, 0, read);
                        }
                        storedHash = memoryStream.ToArray();

                        // 验证哈希值
                        if (!CheckByteArrays(storedHash, currentHash))
                            throw new Exception("文件哈希值验证失败，文件可能已被篡改");

                        // 检查读取的数据量是否与文件大小一致
                        if (totalRead != fileSize)
                            throw new Exception("文件大小不匹配，文件可能不完整");
                    }
                }
            }
        }

        /// <summary>
        /// 检验两个Byte数组是否相同
        /// </summary>
        /// <param name="b1">Byte数组</param>
        /// <param name="b2">Byte数组</param>
        /// <returns>true－相等</returns>
        private static bool CheckByteArrays(byte[] b1, byte[] b2)
        {
            if (b1.Length == b2.Length)
            {
                for (int i = 0; i < b1.Length; ++i)
                {
                    if (b1[i] != b2[i])
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}