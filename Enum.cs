using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ComicsDownload
{
    public enum DownloadStatus
    {
        新任務,任務分析中, 分析完畢, 出現錯誤, 下載中,下載完成,下載中斷,下載停止,刪除失敗
    }
}
