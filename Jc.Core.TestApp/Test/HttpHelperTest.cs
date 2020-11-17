using Jc.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jc.Core.TestApp.Test
{
    public class HttpHelperTest
    {
        /// <summary>
        /// 文件上传测试
        /// </summary>
        public void UploadFileTest()
        {
            HttpHelper httpHelper = new HttpHelper();

            string url = "http://127.0.0.1:5000/DeviceConfigFile/SyncConfigFile";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"Test\1.exe");
            List<string> fileList = new List<string>() { filePath };
            string paramStr = "groupName=groupName&groupCode=groupCode&ConfigCode=ConfigCode";
            string result = httpHelper.UploadFile(url, paramStr, fileList);
        }

        public void PostTest()
        {
            string url = "https://ieeexplore.ieee.org/rest/search";
            Dictionary<string, string> headerParams = new Dictionary<string, string>();
            headerParams.Add("Referer", "https://ieeexplore.ieee.org/search/searchresult.jsp?action=search&newsearch=true&searchField=Search_All&matchBoolean=true&queryText=()");
            string queryText = "(\"Document Title\":3d)";

            Dictionary<string, object> queryParams = new Dictionary<string, object>()
                 .Append("action", "search")
                 .Append("newsearch", true)
                 .Append("searchField", "Search_All")
                 .Append("matchBoolean", true)
                 .Append("queryText", queryText)
                 .Append("pageNumber", 1);    //查询条件
            HttpHelper httpHelper = new HttpHelper();
            httpHelper.SetHeaders(headerParams);
            string result = httpHelper.Post(url, queryParams, HttpContentType.Json);
        }

    }
}
