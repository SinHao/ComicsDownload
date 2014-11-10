using System.Collections.Generic;
using ComicsDownload.Control;

namespace ComicsDownload.Class
{
    public class DownloadControlManager
    {
        public static List<abstractDownloadControl> DownloadControlList
        {
            get
            {
                return new List<abstractDownloadControl>
                {
                    new eightComics()
                };
            }
        }       
    
        public static abstractDownloadControl GetDownloadControl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                return DownloadControlList.Find(downloadconrto => downloadconrto.CheckDownloadControl(url));
            }
            return null;
        }
    }
}
