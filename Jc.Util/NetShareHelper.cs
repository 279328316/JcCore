using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Jc
{
    /// <summary>
    /// 网络共享Helper
    /// </summary>
    public class NetShareHelper
    {
        #region WNetUseConnection枚举参数
        //dwScope
        const int RESOURCE_CONNECTED = 0x00000001;  //枚举已连接的资源（忽略dwUsage）
        const int RESOURCE_GLOBALNET = 0x00000002;  //枚举所有资源
        const int RESOURCE_REMEMBERED = 0x00000003; //只枚举永久性连接

        //dwType
        const int RESOURCETYPE_ANY = 0x00000000;    //枚举所有类型的网络资源
        const int RESOURCETYPE_DISK = 0x00000001;   //枚举磁盘资源
        const int RESOURCETYPE_PRINT = 0x00000002;  //枚举打印资源

        //dwDisplayType
        const int RESOURCEDISPLAYTYPE_GENERIC = 0x00000000;
        const int RESOURCEDISPLAYTYPE_DOMAIN = 0x00000001;
        const int RESOURCEDISPLAYTYPE_SERVER = 0x00000002;
        const int RESOURCEDISPLAYTYPE_SHARE = 0x00000003;
        const int RESOURCEDISPLAYTYPE_FILE = 0x00000004;
        const int RESOURCEDISPLAYTYPE_GROUP = 0x00000005;

        //dwUsage
        const int RESOURCEUSAGE_CONNECTABLE = 0x00000001;
        const int RESOURCEUSAGE_CONTAINER = 0x00000002;

        //dwFlags
        const int CONNECT_INTERACTIVE = 0x00000008;
        const int CONNECT_PROMPT = 0x00000010;
        const int CONNECT_REDIRECT = 0x00000080;
        const int CONNECT_UPDATE_PROFILE = 0x00000001;
        const int CONNECT_COMMANDLINE = 0x00000800;
        const int CONNECT_CMD_SAVECRED = 0x00001000;

        const int CONNECT_LOCALDRIVE = 0x00000100;
        #endregion

        #region Errors参数
        const int NO_ERROR = 0;

        const int ERROR_ACCESS_DENIED = 5;
        const int ERROR_ALREADY_ASSIGNED = 85;
        const int ERROR_BAD_NETWORKPATH = 53;
        const int ERROR_BAD_DEVICE = 1200;
        const int ERROR_BAD_NET_NAME = 67;
        const int ERROR_BAD_PROVIDER = 1204;
        const int ERROR_CANCELLED = 1223;
        const int ERROR_EXTENDED_ERROR = 1208;
        const int ERROR_INVALID_ADDRESS = 487;
        const int ERROR_INVALID_PARAMETER = 87;
        const int ERROR_UNKNOWUSER_OR_INVALIDPASSWORD = 86;
        const int ERROR_INVALID_PASSWORD = 1216;
        const int ERROR_MORE_DATA = 234;
        const int ERROR_NO_MORE_ITEMS = 259;
        const int ERROR_NO_NET_OR_BAD_PATH = 1203;
        const int ERROR_NO_NETWORK = 1222;
        const int ERROR_MORE_CONNECT = 1219;

        const int ERROR_BAD_PROFILE = 1206;
        const int ERROR_CANNOT_OPEN_PROFILE = 1205;
        const int ERROR_DEVICE_IN_USE = 2404;
        const int ERROR_NOT_CONNECTED = 2250;
        const int ERROR_OPEN_FILES = 2401;

        private struct ErrorInfo
        {
            //定义错误类结构体
            public int num;
            public string message;
            public ErrorInfo(int num, string message)
            {
                this.num = num;
                this.message = message;
            }
        }


        //连接失败信息汇总
        private static ErrorInfo[] ERROR_LIST = new ErrorInfo[] {
            new ErrorInfo(ERROR_ACCESS_DENIED, "Error: Access Denied"),
            new ErrorInfo(ERROR_ALREADY_ASSIGNED, "Error: Already Assigned"),
            new ErrorInfo(ERROR_BAD_DEVICE, "Error: Bad Device"),
            new ErrorInfo(ERROR_BAD_NET_NAME, "Error: Bad Net Name"),
            new ErrorInfo(ERROR_BAD_PROVIDER, "Error: Bad Provider"),
            new ErrorInfo(ERROR_CANCELLED, "Error: Cancelled"),
            new ErrorInfo(ERROR_EXTENDED_ERROR, "Error: Extended Error"),
            new ErrorInfo(ERROR_INVALID_ADDRESS, "Error: Invalid Address"),
            new ErrorInfo(ERROR_INVALID_PARAMETER, "Error: Invalid Parameter"),
            new ErrorInfo(ERROR_INVALID_PASSWORD, "Error: Invalid Password"),
            new ErrorInfo(ERROR_MORE_DATA, "Error: More Data"),
            new ErrorInfo(ERROR_NO_MORE_ITEMS, "Error: No More Items"),
            new ErrorInfo(ERROR_NO_NET_OR_BAD_PATH, "Error: No Net Or Bad Path"),
            new ErrorInfo(ERROR_NO_NETWORK, "Error: No Network"),
            new ErrorInfo(ERROR_BAD_PROFILE, "Error: Bad Profile"),
            new ErrorInfo(ERROR_CANNOT_OPEN_PROFILE, "Error: Cannot Open Profile"),
            new ErrorInfo(ERROR_DEVICE_IN_USE, "Error: Device In Use"),
            new ErrorInfo(ERROR_EXTENDED_ERROR, "Error: Extended Error"),
            new ErrorInfo(ERROR_NOT_CONNECTED, "Error: Not Connected"),
            new ErrorInfo(ERROR_OPEN_FILES, "Error: Open Files"),
            new ErrorInfo(ERROR_BAD_NETWORKPATH, "Error: 找不到网络"),
            new ErrorInfo(ERROR_UNKNOWUSER_OR_INVALIDPASSWORD, "Error: 未知的用户名或密码错误"),
            new ErrorInfo(ERROR_MORE_CONNECT, "Error: 不允许一个用户使用一个以上用户名与一个服务器或共享资源的多重连接。中断与此服务器或共享资源的所有连接，然后再试一次"),
        };

        private static string GetError(int errNum)
        {
            //遍历获得错误代码
            foreach (ErrorInfo er in ERROR_LIST)
            {
                if (er.num == errNum) return er.message;
            }
            return "Error: Unknown, " + errNum;
        }
        #endregion

        #region DllImport 引入Dll
        //调用系统函数WNetUseConnection
        //用于连接共享
        [DllImport("Mpr.dll")]
        private static extern int WNetUseConnection(
            IntPtr hwndOwner,
            NETRESOURCE lpNetResource,
            string lpPassword,
            string lpUserID,
            int dwFlags,
            string lpAccessName,
            string lpBufferSize,
            string lpResult
            );
        //用于删除连接
        [DllImport("Mpr.dll")]
        private static extern int WNetCancelConnection2(
            string lpName,
            int dwFlags,
            bool fForce
            );
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public int dwScope = 0;
            public int dwType = 0;
            public int dwDisplayType = 0;
            public int dwUsage = 0;
            public string lpLocalName = "";//映射到本地的盘符，如"Z:"。不做驱动器映射，可为空
            public string lpRemoteName = "";//共享的网络路径
            public string lpComment = "";
            public string lpProvider = "";
        }

        /// <summary>
        /// 连接共享
        /// SharePath不能带\结尾.带\结尾会发生53 Error错误的网络路径
        /// </summary>
        /// <param name="sharePath">共享网络路径 \\127.0.0.1\share</param>
        /// <param name="username">登录用户名 不需要登录时,忽略</param>
        /// <param name="password">密码 不需要登录时,忽略</param>
        /// <param name="localDiskName">本地磁盘路径 Z: 不需要映射时,忽略</param>
        /// <returns></returns>
        public static void ConnectShare(string sharePath, string username = null, string password = null,string localDiskName = null)
        {
            NETRESOURCE nr = new NETRESOURCE
            {
                dwType = RESOURCETYPE_ANY,
                dwDisplayType = RESOURCEDISPLAYTYPE_SHARE,
                lpRemoteName = sharePath,
                lpLocalName = localDiskName
            };

            int ret;
            int dwFlag = 0;
            //if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            //{
            //    dwFlag = CONNECT_INTERACTIVE | CONNECT_PROMPT;
            //}
            ret = WNetUseConnection(IntPtr.Zero, nr, password, username, dwFlag, null, null, null);
            if (ret != NO_ERROR)
            {
                if (ret == ERROR_ALREADY_ASSIGNED || ret == ERROR_MORE_CONNECT)
                {   //已存在 或 用户多连接访问错误
                }
                else
                {
                    string error = GetError(ret);
                    throw new Exception(error);
                }
            }
        }

        /// <summary>
        /// 断开共享链接
        /// </summary>
        /// <param name="sharePath">共享网络路径 or 映射磁盘 T:</param>
        /// <param name="fForce">强制断开</param>
        /// <returns></returns>
        public static void DisconnectShare(string sharePath,bool fForce = true)
        {
            int ret = WNetCancelConnection2(sharePath, CONNECT_UPDATE_PROFILE, fForce);
            if (ret != NO_ERROR && ret != ERROR_NOT_CONNECTED)
            {
                string error = GetError(ret);
                throw new Exception(error);
            }
        }
    }
}
