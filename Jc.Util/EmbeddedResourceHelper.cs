using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Jc.Util
{
    /// <summary>
    /// 内嵌资源使用Helper
    /// </summary>
    public class EmbeddedResourceHelper
    {
        /// <summary>
        /// 程序集,组件
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// 资源命名空间
        /// </summary>
        public string ResourceNamespace { get; private set; }

        /// <summary>
        /// 所有资源列表
        /// </summary>
        public List<string> ResourceList { get; private set; }

        private Dictionary<string, byte[]> resDic = new Dictionary<string, byte[]>();

        /// <summary>
        /// Ctor
        /// 资源命名空间为空 则读取所有资源列表
        /// </summary>
        /// <param name="assembly">程序集,组件Assembly</param>
        /// <param name="resourceNamespace">资源命名空间</param>
        public EmbeddedResourceHelper(Assembly assembly, string resourceNamespace = null)
        {
            this.Assembly = assembly;
            this.ResourceNamespace = resourceNamespace;
            if (string.IsNullOrEmpty(resourceNamespace))
            {
                this.ResourceList = assembly.GetManifestResourceNames().ToList();
            }
            else
            {
                this.ResourceList = assembly.GetManifestResourceNames()
                                    .Where(resName=>resName.StartsWith(resourceNamespace)).ToList();
            }
        }

        /// <summary>
        /// 获取资源内容
        /// 资源路径 Lib\a.txt
        /// 缓存该资源 默认开启 如果非频繁读取的资源可以设置为false不进行缓存
        /// </summary>
        /// <param name="resPath">资源路径</param>
        /// <param name="cache">缓存该资源</param>
        /// <returns>资源内容 byte[]</returns>
        public byte[] GetResource(string resPath, bool cache = true)
        {
            byte[] content = null;
            string resName = GetResourceName(resPath);
            if(resDic.Keys.Contains(resName))
            {
                content = resDic[resName];
            }
            else
            {
                if(this.ResourceList.Contains(resName))
                {
                    using (Stream stream = Assembly.GetManifestResourceStream(resName))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            content = memoryStream.ToArray();
                            if (cache)
                            {
                                resDic.Add(resName, content);
                            }
                        }
                    }
                }
            }
            return content;
        }

        /// <summary>
        /// 获取该路径下所有资源
        /// </summary>
        /// <param name="resPath">资源路径</param>
        /// <returns></returns>
        public List<string> GetResources(string resPath)
        {
            string resName = GetResourceName(resPath);
            List<string> resList = this.ResourceList = ResourceList.Where(a => a.StartsWith(resName)).ToList();
            return resList;
        }
        
        /// <summary>
        /// 获取资源名称
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public string GetResourceName(string resPath)
        {
            return resPath.Replace("/",".").Replace("-", "_");
        }
    }
}
