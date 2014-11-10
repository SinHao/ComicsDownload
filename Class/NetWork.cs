﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Threading;


namespace ComicsDownload.Class
{
    public class Network
    {

        //public static HtmlDocument GetHtmlDocument(string htmlSource)
        //{

        //    var htmlRoot = new HtmlDocument();
        //    htmlRoot.LoadHtml(htmlSource);

        //    return htmlRoot;
        //}

        //public static void RemoveSubHtmlNode(HtmlNode curHtmlNode, string subNodeToRemove)
        //{

        //    try
        //    {
        //        var foundAllSub = curHtmlNode.SelectNodes(subNodeToRemove);
        //        if (foundAllSub != null)
        //        {
        //            foreach (HtmlNode subNode in foundAllSub)
        //            {
        //                curHtmlNode.RemoveChild(subNode);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //    //return curHtmlNode;
        //}

        //public static void RemoveSubHtmlNode(HtmlNode curHtmlNode, string subNodeToRemove, string subNodeToRemove2)
        //{

        //    try
        //    {
        //        var foundAllSub = curHtmlNode.SelectNodes(subNodeToRemove);
        //        if (foundAllSub != null)
        //        {
        //            foreach (HtmlNode subNode in foundAllSub)
        //            {
        //                RemoveSubHtmlNode(subNode, subNodeToRemove2);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //    //return curHtmlNode;
        //}
        /// <summary>
        /// 取得網頁網始碼
        /// </summary>
        /// <param name="para"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string GetHtmlSource(DownloadParameter para, System.Text.Encoding encode)
        {
            return GetHtmlSource(para, encode, new WebProxy());
        }

        /// <summary>
        /// 取得網頁網始碼
        /// </summary>
        /// <param name="request"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string GetHtmlSource(HttpWebRequest request, System.Text.Encoding encode)
        {
            string sline = "";
            bool needRedownload = false;
            int remainTimes = 3;

            do
            {
                try
                {
                    //接收 HTTP 回應
                    var res = (HttpWebResponse)request.GetResponse();
                    var responseStream = res.GetResponseStream();
                    if (responseStream == null) throw new Exception();
                    StreamReader reader;
                    switch (res.ContentEncoding)
                    {
                        case "gzip":
                            //Gzip解壓縮
                            using (var gzip = new GZipStream(responseStream, CompressionMode.Decompress))
                            {
                                reader = new StreamReader(gzip, encode);

                                sline = reader.ReadToEnd();

                            }
                            break;
                        case "deflate":
                            //deflate解壓縮
                            using (var deflate = new DeflateStream(responseStream, CompressionMode.Decompress))
                            {
                                reader = new StreamReader(deflate, encode);

                                sline = reader.ReadToEnd();

                            }
                            break;
                        default:
                            reader = new StreamReader(responseStream, encode);

                            sline = reader.ReadToEnd();
                            break;
                    }

                }
                catch (Exception ex)
                {
                    //重試等待時間
                    Thread.Sleep(3000);
                    needRedownload = true;

                    //重試次數-1
                    remainTimes--;
                    //如果重試次數小於0，拋出錯誤
                    if (remainTimes < 0)
                    {
                        needRedownload = false;
                        //CoreManager.LogManager.Debug(ex.ToString());

                    }


                }
            } while (needRedownload);
            return sline;
        }

        /// <summary>
        /// 取得網頁網始碼
        /// </summary>
        /// <param name="para"></param>
        /// <param name="encode"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static string GetHtmlSource(DownloadParameter para, System.Text.Encoding encode, WebProxy proxy)
        {

            //再來建立你要取得的Request
            var webReq = (HttpWebRequest)WebRequest.Create(para.Url);
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            webReq.Headers.Set("Accept-Language", "zh-TW");
            webReq.UserAgent = para.UserAgent;
            webReq.Headers.Set("Accept-Encoding", "gzip, deflate");
            //webReq.Host = "www09.eyny.com";
            webReq.KeepAlive = true;
            //將剛剛取得的cookie加上去
            webReq.CookieContainer = para.Cookies;
            webReq.Timeout = 30000;
            if (para.Timeout != 0)
            {
                webReq.Timeout = para.Timeout;
            }

            //webReq.Proxy = proxy;
            return GetHtmlSource(webReq, encode);
        }



    }
}
