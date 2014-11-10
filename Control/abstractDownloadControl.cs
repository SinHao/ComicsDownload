using ComicsDownload.Class;
using System;
using System.Text.RegularExpressions;


namespace ComicsDownload.Control
{
    public abstract class abstractDownloadControl:IDownloadControl
    {
        /// <summary>
        /// 任務屬性        
        /// </summary>
        public TaskInfo Task { get; set; }

        /// <summary>
        /// 下載時在使用的(包含抓起HtmlCode)
        /// </summary>
        public DownloadParameter CurrentParameter { get; set; }        

        /// <summary>
        /// 確認網址實作
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public abstract bool CheckDownloadControl(string url);
        
        /// <summary>
        /// 下載實作
        /// </summary>
        /// <returns></returns>
        public abstract bool StartDownload();

        /// <summary>
        /// 網址內部分析
        /// 可在這邊設定 Task.SectionStart,Task.LastSection,Task.PageCount,Task.PageStart,Task.LastPage
        /// </summary>
        public abstract void Analysis();
    }
}
