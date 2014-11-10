using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ComicsDownload.Class
{
    public class ThreadManager
    {
        public static Dictionary<int, Thread> ThreadList = new Dictionary<int, Thread>();
    }
}
