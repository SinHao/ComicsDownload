using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace ComicsDownload.Class
{

    /// <summary>
    /// 下载参数
    /// </summary>
    public class DownloadParameter
    {
        /// <summary>
        /// 资源的网络位置
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 要创建的本地文件位置
        /// </summary>
        public string FilePath { get; set; }


        /// <summary>
        /// 是否停止下载(可以在下载过程中进行设置，用来控制下载过程的停止)
        /// </summary>
        public bool IsStop { get; set; }

        /// <summary>
        /// 读取或设置发出请求时使用的Cookie
        /// </summary>
        public CookieContainer Cookies { get; set; }

        /// <summary>
        /// 读取或设置下载请求所使用的Referer值
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// 读取或设置下载请求所使用的User-Agent值
        /// </summary>
        public string UserAgent { get { return "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)"; } }

        public int Timeout { get; set; }
    }

}
