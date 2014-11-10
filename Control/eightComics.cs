using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComicsDownload.Control;
using System.Text.RegularExpressions;
using ComicsDownload.Class;
using System.Net;

namespace ComicsDownload.Control
{
    public class eightComics : abstractDownloadControl
    {
        /// <summary>
        /// 圖片網址
        /// </summary>
        Dictionary<int, List<string>> ImgRUL = new Dictionary<int, List<string>>();
        
        /// <summary>
        /// 正規表示法在用的
        /// </summary>
        Regex regex;

        public eightComics()
        {
            CurrentParameter = new DownloadParameter();
        }
        
        /// <summary>
        /// 網址確認實作
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public override bool CheckDownloadControl(string url)
        {
            string[] regexString = new string[]
{
"^http:\\/\\/new\\.comicvip\\.com\\/show\\/(?<TID>\\D.*)\\.\\S*\\?ch=(?<ch>\\d*)",
"^http:\\/\\/\\w*\\.8comic\\.com\\/html\\/(?<TID>\\d*)\\.html"
};
            foreach (string i in regexString)
            {                
                Regex regex = new Regex(i);
                if (regex.Match(url).Success)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 網址分析實作
        /// </summary>
        public override void Analysis()
        {            
            ConvertURL();   //因為8平台的列表不會有漫畫實際網址，所以需要先找到裡面的網址
            GetImgURLandComicsURL();    //分析抓到的亂碼轉成實際圖片網址
            Task.SectionStart = 1;
            Task.LastSection = Task.SectionTotal;
            Task.PageCount = ImgRUL[Task.SectionStart].Count();
            Task.PageStart = 1;
            Task.LastPage = Task.PageCount;
            Task.Platform = "8Comics";
        }

        /// <summary>
        /// 分析圖片網址亂碼並找出實際位置
        /// </summary>
        private void GetImgURLandComicsURL()
        {
            CurrentParameter.Url = Task.Url;            
            string sHTML_CODE = Network.GetHtmlSource(CurrentParameter, Encoding.GetEncoding("BIG5"));
            Regex r = new Regex("<title>(?<title>\\S*).*<\\/title>");
            Match m = r.Match(sHTML_CODE);
            if (m.Success)
            {
                Task.Name = m.Groups["title"].Value;
            }
            string itemid = string.Empty;
            string chs = string.Empty;
            string allcodes = string.Empty;
            regex = new Regex("var\\schs=(?<chs>\\d*);var\\sti=(?<itemid>\\d*);var\\scs=\'(?<allcodes>.*)\';");
            m = regex.Match(sHTML_CODE);
            if (m.Success)
            {
                itemid = m.Groups["itemid"].Value;
                chs = m.Groups["chs"].Value;
                allcodes = m.Groups["allcodes"].Value;
                int f = 50;
                int keyLength = allcodes.Length;
                for (int i = 0; i < keyLength / f; i++)
                {
                    int change_tmp = 0;
                    string ch = strSplit(allcodes, i * f, 4);
                    if (int.TryParse(ch, out change_tmp))
                    {
                        string Ccode = allcodes.Substring(i * f, f);
                        var imgurl_list = new List<string>();
                        int page = int.Parse(strSplit(Ccode, 7, 3));
                        for (int j = 1; j <= page; j++)
                        {
                            string code1 = strSplit(Ccode, 4, 2);
                            string code2 = strSplit(Ccode, 6, 1);
                            string code3 = itemid;
                            string code4 = strSplit(Ccode, 0, 4);
                            string code5 = j.ToString("000");
                            string code6 = Ccode.Substring((((j - 1) / 10) % 10) + (((j - 1) % 10) * 3) + 10, 3);
                            imgurl_list.Add("http://img" + code1 + ".8comic.com/" + code2 + "/" + code3 + "/" + code4 + "/" + code5 + "_" + code6 + ".jpg");
                        }
                        ImgRUL.Add(int.Parse(ch), imgurl_list);
                    }
                    Task.SectionTotal = int.Parse(ch);
                }                
            }
        }

        /// <summary>
        /// 分析亂碼用
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public string strSplit(string str, int start, int count)
        {
            string temp1;
            string temp2 = string.Empty;

            temp1 = str.Substring(start, count);

            for (int i = 0; i < temp1.Length; i++)
            {
                int tmp = 0;
                if (int.TryParse(temp1.Substring(i, 1), out tmp))
                    temp2 += temp1[i];
            }
            return temp2;
        }
        
        /// <summary>
        /// 下載實作
        /// </summary>
        /// <returns></returns>
        public override bool StartDownload()
        {
            IDownLoadType downloadProvider = null;
            if (Task.IsSingle)
            { downloadProvider = new SingleSctionDownLoad(Task, ImgRUL[Task.LastSection]); }
            else
            { downloadProvider = new SelectSctionDownLoad(Task, ImgRUL); }
            try
            {
                downloadProvider.StartDownLoad();
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 解析HtmlCode
        /// </summary>
        /// <param name="convertString">HtmlCoe</param>
        /// <param name="dataName">Html節點名稱</param>
        /// <returns></returns>
        private string GetComicsInfo(string convertString, string dataName)
        {
            string[] regexString = new string[]{
                "^http:\\/\\/new\\.comicvip\\.com\\/show\\/(?<TID>\\D.*)\\.\\S*\\?ch=(?<ch>\\d*)",
                "^http:\\/\\/\\w*\\.8comic\\.com\\/html\\/(?<TID>\\d*)\\.html",
                "onclick=\"cview\\(\\'(?<URL>\\d*)\\-\\S*\\'\\,(?<NUM>\\d*)\\)"
            };
            foreach (var i in regexString)
            {
                regex = new Regex(i);
                Match m = regex.Match(convertString);
                if (m.Success)
                { return m.Groups[dataName].Value; }
            }
            return "";
        }

        //因為8平台的列表不會有漫畫實際網址，所以需要先找到裡面的網址
        private void ConvertURL()
        {
            if (string.IsNullOrEmpty(GetComicsInfo(Task.Url, "ch")))
            {
                CurrentParameter.Url = Task.Url;
                string sHTML_CODE = Network.GetHtmlSource(CurrentParameter, Encoding.GetEncoding("BIG5"));
                string BOOKNUM = GetComicsInfo(sHTML_CODE, "URL");
                try
                {
                    int NUM = int.Parse(GetComicsInfo(sHTML_CODE, "NUM"));
                    string NewURL = "";
                    if (NUM == 4 || NUM == 6 || NUM == 12 || NUM == 22) NewURL = "http://new.comicvip.com/show/cool-";
                    if (NUM == 1 || NUM == 17 || NUM == 19 || NUM == 21) NewURL = "http://new.comicvip.com/show/cool-";
                    if (NUM == 2 || NUM == 5 || NUM == 7 || NUM == 9) NewURL = "http://new.comicvip.com/show/cool-";
                    if (NUM == 10 || NUM == 11 || NUM == 13 || NUM == 14) NewURL = "http://new.comicvip.com/show/best-manga-";
                    if (NUM == 3 || NUM == 8 || NUM == 15 || NUM == 16 || NUM == 18 || NUM == 20) NewURL = "http://new.comicvip.com/show/best-manga-";
                    Task.Url = NewURL + BOOKNUM + ".html?ch=1";                    
                }
                catch
                {
                    throw;
                }
            }
        }
                
        interface IDownLoadType
        {
            void StartDownLoad();
        }

        class SingleSctionDownLoad : IDownLoadType
        {
            private TaskInfo task;
            private List<string> pageImgURL;

            public SingleSctionDownLoad(TaskInfo _task,object _pageURL)
            {
                this.task = _task;
                this.pageImgURL = (List<string>)_pageURL;                
            }

            public void StartDownLoad()
            {
                for (; task.PageStart <= task.LastPage && task.IsExecuting; task.PageStart++)
                {
                    string imgurl = Network.GetHtmlSource((HttpWebRequest)WebRequest.Create(pageImgURL[task.PageStart + 1]), Encoding.GetEncoding(1251));
                    FileWrite.TxtWrire(imgurl, task.Name + task.LastSection.ToString(), Encoding.GetEncoding(1251));
                }
            }
            
        }

        class SelectSctionDownLoad : IDownLoadType
        {
            private TaskInfo task;

            private Dictionary<int, List<string>> imgUrlList;

            public SelectSctionDownLoad(TaskInfo _task,object _imgURL)
            {
                this.task = _task;
                this.imgUrlList = (Dictionary<int, List<string>>)_imgURL;
            }

            public void StartDownLoad()
            {
                for (; task.SectionStart <= task.LastSection && task.IsExecuting; task.SectionStart++)
                {
                    if (imgUrlList.ContainsKey(task.SectionStart))
                    {
                        task.PageStart = 1;
                        task.PageCount = imgUrlList[task.SectionStart].Count;
                        task.LastPage = task.PageCount;
                        for (; task.PageStart <= task.LastPage && task.IsExecuting; task.PageStart++)
                        {
                            int pageindex = task.PageStart - 1;
                            string imgurl = Network.GetHtmlSource((HttpWebRequest)WebRequest.Create(imgUrlList[task.SectionStart][pageindex]), Encoding.GetEncoding(1251));
                            string filepath = task.FilePath + "\\" + task.Name + (task.SectionStart).ToString("000") + "\\" + (task.PageStart).ToString("000") + ".jpg";
                            FileWrite.TxtWrire(imgurl, filepath, Encoding.GetEncoding(1251));
                        }                            
                    }
                }
            }
        }        
    }
}
