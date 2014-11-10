using ComicsDownload.Class;

namespace ComicsDownload.Control
{
    public interface IDownloadControl
    {
        /// <summary>
        /// 任務
        /// </summary>
        TaskInfo Task { get; set; }

        /// <summary>
        /// 下載時在使用的(包含抓起HtmlCode)
        /// </summary>
        DownloadParameter CurrentParameter { get; set; }

        /// <summary>
        /// 確認網址實作
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool CheckDownloadControl(string url);

        /// <summary>
        /// 下載實作
        /// </summary>
        /// <returns></returns>
        bool StartDownload();

        /// <summary>
        /// 網址內部分析
        /// 可在這邊設定 Task.SectionStart,Task.LastSection,Task.PageCount,Task.PageStart,Task.LastPage
        /// </summary>
        void Analysis();
    }
}
