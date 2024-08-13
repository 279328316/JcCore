using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Jc
{
    /// <summary>
    /// File操作Helper
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 获取文件扩展名 默认包含.号
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="withPoint">是否包含.号 默认值true</param>
        /// <returns></returns>
        public static string GetFileExtension(string fileName, bool withPoint = true)
        {
            string exStr = "";
            int index = fileName.LastIndexOf(".");
            if (index > 0 && index < fileName.Length - 1)
            {
                int subIndex = withPoint ? index : index + 1;
                exStr = fileName.Substring(subIndex);
            }
            return exStr;
        }

        /// <summary>
        /// 返回带单位的文件大小 取部分
        /// </summary>
        /// <param name="fileSize">字节数</param>
        /// <returns></returns>
        public static string FormatFileSize(long fileSize)
        {
            if (fileSize < 0)
            {
                throw new ArgumentOutOfRangeException("fileSize");
            }
            else if (fileSize >= 1024 * 1024 * 1024)
            {
                return string.Format("{0:########0.00} GB", ((Double)fileSize) / (1024 * 1024 * 1024));
            }
            else if (fileSize >= 1024 * 1024)
            {
                return string.Format("{0:####0.00} MB", ((Double)fileSize) / (1024 * 1024));
            }
            else if (fileSize >= 1024)
            {
                return string.Format("{0:####0.00} KB", ((Double)fileSize) / 1024);
            }
            else
            {
                return string.Format("{0} bytes", fileSize);
            }
        }

        /// <summary>
        /// 复制文件夹中的所有内容
        /// </summary>
        /// <param name="sourceDir">源文件夹目录</param>
        /// <param name="targetDir">指定文件夹目录</param>
        /// <param name="overwrite">是否覆盖已存在文件</param>
        public static void CopyDirectory(string sourceDir, string targetDir, bool overwrite = false)
        {
            try
            {
                if (!Directory.Exists(sourceDir))
                {
                    return;
                }
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                DirectoryInfo dir = new DirectoryInfo(sourceDir);
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string targetFilePath = Path.Combine(targetDir, file.Name);
                    if (File.Exists(targetFilePath) && !overwrite)
                        throw new Exception($"文件{targetFilePath}已存在");
                    file.CopyTo(targetFilePath, true);
                }

                DirectoryInfo[] childDirs = dir.GetDirectories();
                foreach (DirectoryInfo childDir in childDirs)
                {
                    CopyDirectory(childDir.FullName, Path.Combine(targetDir, childDir.Name));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取指定路径的大小
        /// </summary>
        /// <param name="dirPath">路径</param>
        /// <returns></returns>
        public static long GetDirectorySize(string dirPath)
        {
            long len = 0;
            //判断该路径是否存在（是否为文件夹）
            if (!Directory.Exists(dirPath))
            {
                //查询文件的大小
                len = GetFileSize(dirPath);
            }
            else
            {
                //定义一个DirectoryInfo对象
                DirectoryInfo di = new DirectoryInfo(dirPath);

                //通过GetFiles方法，获取di目录中的所有文件的大小
                foreach (FileInfo fi in di.GetFiles())
                {
                    len += fi.Length;
                }
                //获取di中所有的文件夹，并存到一个新的对象数组中，以进行递归
                DirectoryInfo[] dis = di.GetDirectories();
                if (dis.Length > 0)
                {
                    for (int i = 0; i < dis.Length; i++)
                    {
                        len += GetDirectorySize(dis[i].FullName);
                    }
                }
            }
            return len;
        }

        /// <summary>
        /// 获取指定路径的占用空间
        /// </summary>
        /// <param name="dirPath">路径</param>
        /// <returns></returns>
        public static long GetDirectorySpace(string dirPath)
        {
            //返回值
            long len = 0;
            //判断该路径是否存在（是否为文件夹）
            if (!Directory.Exists(dirPath))
            {
                //如果是文件，则调用
                len = GetFileSpace(dirPath);
            }
            else
            {
                //定义一个DirectoryInfo对象
                DirectoryInfo di = new DirectoryInfo(dirPath);
                //本机的簇值
                long clusterSize = GetClusterSize(di);
                //遍历目录下的文件，获取总占用空间
                foreach (FileInfo fi in di.GetFiles())
                {
                    //文件大小除以簇，余若不为0
                    if (fi.Length % clusterSize != 0)
                    {
                        decimal res = fi.Length / clusterSize;
                        //文件大小除以簇，取整数加1。为该文件占用簇的值
                        int clu = Convert.ToInt32(Math.Ceiling(res)) + 1;
                        long result = clusterSize * clu;
                        len += result;
                    }
                    else
                    {
                        //余若为0，则占用空间等于文件大小
                        len += fi.Length;
                    }
                }
                //获取di中所有的文件夹，并存到一个新的对象数组中，以进行递归
                DirectoryInfo[] dis = di.GetDirectories();
                if (dis.Length > 0)
                {
                    for (int i = 0; i < dis.Length; i++)
                    {
                        len += GetDirectorySpace(dis[i].FullName);
                    }
                }
            }
            return len;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long GetFileSize(string filePath)
        {
            //定义一个FileInfo对象，是指与filePath所指向的文件相关联，以获取其大小
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        /// <summary>
        /// 获取文件占用空间
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long GetFileSpace(string filePath)
        {
            long temp = 0;
            //定义一个FileInfo对象，是指与filePath所指向的文件相关联，以获取其大小
            FileInfo fileInfo = new FileInfo(filePath);
            long clusterSize = GetClusterSize(fileInfo);
            if (fileInfo.Length % clusterSize != 0)
            {
                decimal res = fileInfo.Length / clusterSize;
                int clu = Convert.ToInt32(Math.Ceiling(res)) + 1;
                temp = clusterSize * clu;
            }
            else
            {
                return fileInfo.Length;
            }
            return temp;
        }

        //调用windows API获取磁盘空闲空间
        //导入库
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetDiskFreeSpace([MarshalAs(UnmanagedType.LPTStr)] string rootPathName,
        ref int sectorsPerCluster, ref int bytesPerSector, ref int numberOfFreeClusters, ref int totalNumbeOfClusters);

        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <param name="rootPathName"></param>
        /// <returns></returns>
        public static DiskInfo GetDiskInfo(string rootPathName)
        {
            DiskInfo diskInfo = new DiskInfo();
            int sectorsPerCluster = 0, bytesPerSector = 0, numberOfFreeClusters = 0, totalNumberOfClusters = 0;
            GetDiskFreeSpace(rootPathName, ref sectorsPerCluster, ref bytesPerSector, ref numberOfFreeClusters, ref totalNumberOfClusters);

            //每簇的扇区数
            diskInfo.SectorsPerCluster = sectorsPerCluster;
            //每扇区字节
            diskInfo.BytesPerSector = bytesPerSector;

            return diskInfo;
        }

        /// <summary>
        /// 结构 硬盘信息
        /// </summary>
        public struct DiskInfo
        {
            public string RootPathName;
            //每簇的扇区数
            public int SectorsPerCluster;
            //每扇区字节
            public int BytesPerSector;
            public int NumberOfFreeClusters;
            public int TotalNumberOfClusters;
        }

        /// <summary>
        /// 获取每簇的字节
        /// </summary>
        /// <param name="file">指定文件</param>
        /// <returns></returns>
        public static long GetClusterSize(FileInfo file)
        {
            long clusterSize = 0;
            DiskInfo diskInfo = new DiskInfo();
            diskInfo = GetDiskInfo(file.Directory.Root.FullName);
            clusterSize = (diskInfo.BytesPerSector * diskInfo.SectorsPerCluster);
            return clusterSize;
        }
        /// <summary>
        /// 获取每簇的字节
        /// </summary>
        /// <param name="dir">指定目录</param>
        /// <returns></returns>
        public static long GetClusterSize(DirectoryInfo dir)
        {
            long clusterSize = 0;
            DiskInfo diskInfo = new DiskInfo();
            diskInfo = GetDiskInfo(dir.Root.FullName);
            clusterSize = (diskInfo.BytesPerSector * diskInfo.SectorsPerCluster);
            return clusterSize;
        }
    }
}
