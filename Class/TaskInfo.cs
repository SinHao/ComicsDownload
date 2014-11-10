using System;
using System.ComponentModel;
using ComicsDownload.Control;
using System.Threading;
using ComicsDownload.Class;

namespace ComicsDownload
{
    public class TaskInfo : CommonDataBind
    {
        public int ThreadID { get; set; }

        public Guid TaskID { get; set; }

        private DownloadStatus _Status;
        /// <summary>
        /// 下載狀態
        /// </summary>
        public DownloadStatus Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this._Status = value;
                base.OnPropertyChanged("Status");
            }
        }

        private string _Name;
        /// <summary>
        /// 漫畫名稱
        /// </summary>
        public string Name { 
            get { return this._Name; }
            set
            {
                this._Name = value;
                base.OnPropertyChanged("Name");
            }
        }

        private int _PageCount;
        /// <summary>
        /// 該話數的總頁數
        /// </summary>
        public int PageCount
        {
            get
            {
                return this._PageCount;
            }
            set
            {
                this._PageCount = value;
                base.OnPropertyChanged("PageCount");
            }
        }

        private int _PageStart;
        /// <summary>
        /// 要從第幾頁開始抓
        /// </summary>
        public int PageStart
        {
            get
            {
                return this._PageStart;
            }
            set
            {
                this._PageStart = value;
                base.OnPropertyChanged("PageStart");
            }
        }

        private int _LastPage;
        /// <summary>
        /// 要抓到第幾頁
        /// </summary>
        public int LastPage
        {
            get
            {
                return this._LastPage;
            }
            set
            {
                this._LastPage = value;
                base.OnPropertyChanged("LastPage");
            }
        }

        private string _Platform;
        /// <summary>
        /// 漫畫平台
        /// </summary>
        public string Platform
        {
            get { return this._Platform; }
            set
            {
                this._Platform = value;
                base.OnPropertyChanged("Platform");
            }
        }

        private int _SectionTotal;
        /// <summary>
        /// 全部的話數
        /// </summary>
        public int SectionTotal
        {
            get
            {
                return this._SectionTotal;
            }
            set
            {
                this._SectionTotal = value;
                base.OnPropertyChanged("SectionTotal");
            }
        }

        private int _SectionStart;
        /// <summary>
        /// 要從第幾話開始抓
        /// </summary>
        public int SectionStart
        {
            get
            {
                return this._SectionStart;
            }
            set
            {
                this._SectionStart = value;
                base.OnPropertyChanged("SectionStart");
            }
        }

        private int _LastSection;
        /// <summary>
        /// 要抓到第幾話
        /// </summary>
        public int LastSection
        {
            get
            {
                return this._LastSection;
            }
            set
            {
                this._LastSection = value;
                base.OnPropertyChanged("LastSection");
            }
        }

        /// <summary>
        /// 取得或設定是否執行
        /// </summary>
        public bool IsExecuting { get; set; }

        /// <summary>
        /// 漫畫網址
        /// </summary>
        public string Url { get; set; }

        public bool IsSingle { get; set; }

        private IDownloadControl _ControlBase;
        public IDownloadControl ControlBase { get { return this._ControlBase; } }
        public void SetControlBase(IDownloadControl downloadControl)
        {
            this._ControlBase = downloadControl;
        }

        private string _FilePath;
        /// <summary>
        /// 檔案下載位置
        /// </summary>
        public string FilePath
        {
            get
            {
                return this._FilePath;     
            }
            set
            {
                this._FilePath = value;
                base.OnPropertyChanged("FilePath");
            }
        }
        
        /// <summary>
        /// 開始下載
        /// </summary>
        public void Start()
        {
            //下載中跟分析中 點擊 開始不用任何動作
            if (Status != DownloadStatus.下載中 && Status != DownloadStatus.任務分析中)
            {
                ThreadID = GetThreadID();
                IsExecuting = true;
                Thread t = new Thread(() =>
                {
                    if (ControlBase == null)
                    {
                        Status = DownloadStatus.出現錯誤;
                    }
                    else
                    {
                        Status = DownloadStatus.下載中;
                        if (ControlBase.StartDownload())
                        {
                            Status = DownloadStatus.下載完成;
                        }
                        else
                        {
                            Status = DownloadStatus.下載中斷;
                        }
                    }
                    ThreadManager.ThreadList.Remove(ThreadID);
                });
                ThreadManager.ThreadList.Add(ThreadID, t);
                ThreadManager.ThreadList[ThreadID].Start();
            }
        }
        
        /// <summary>
        /// 停止下載
        /// </summary>
        public void Stop()
        {            
            ThreadManager.ThreadList.Remove(ThreadID);
            this.Status = DownloadStatus.下載停止;
            this.IsExecuting = false;
        }

        public void Analysis()
        {
            //下載中及分析中 不能重新分析
            if (Status != DownloadStatus.下載中 && Status != DownloadStatus.任務分析中)
            {
                ThreadID = GetThreadID();
                Thread t = new Thread(() =>
                {
                    this.Status = DownloadStatus.任務分析中;
                    try
                    {
                        ControlBase.Task = this;
                        ControlBase.Analysis();
                        this.Status = DownloadStatus.分析完畢;
                    }
                    catch
                    {
                        this.Status = DownloadStatus.出現錯誤;
                    }
                    ThreadManager.ThreadList.Remove(ThreadID);
                });
                ThreadManager.ThreadList.Add(ThreadID, t);
                ThreadManager.ThreadList[ThreadID].Start();
            }
        }

        private int GetThreadID()
        {
            int threadid = ThreadManager.ThreadList.Keys.Count;
            while (ThreadManager.ThreadList.ContainsKey(ThreadID))
            {
                threadid++;
            }
            return threadid;
        }
    }

    /// <summary>
    /// 資料綁定用，如果不用不會自動更新VIEW
    /// </summary>
    public class CommonDataBind : INotifyPropertyChanged
    {
         public event PropertyChangedEventHandler PropertyChanged;
         protected void OnPropertyChanged(string name)
         {
             PropertyChangedEventHandler handler = PropertyChanged;
             if (handler != null)
             {
                 handler(this, new PropertyChangedEventArgs(name));
             }
         }
    }
}
