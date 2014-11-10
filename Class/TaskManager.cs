using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace ComicsDownload.Class
{
    public class TaskManager
    {
        private static string _TaskFolderPath { get { return System.Environment.CurrentDirectory; } }
        private static string _TaskFileName { get { return "Task.xml"; } }
        private static string TaskFullFileName { get { return Path.Combine(_TaskFolderPath, _TaskFileName); } }

        /// <summary>
        /// 儲存任務列表
        /// </summary>
        /// <param name="taskList"></param>
        public static void SaveTaskList(BindingList<TaskInfo> taskList)
        {
            using (var oFileStream = new FileStream(TaskFullFileName, FileMode.Create))
            {
                var oXmlSerializer = new XmlSerializer(typeof(BindingList<TaskInfo>));
                oXmlSerializer.Serialize(oFileStream, taskList);
            }
        }

        /// <summary>
        /// 讀取任務列表
        /// </summary>
        /// <returns></returns>
        public static BindingList<TaskInfo> LoadTaskList()
        {
            
            BindingList<TaskInfo> tasklist = new BindingList<TaskInfo>();
            try
            {
                //如果檔案存在
                if (File.Exists(TaskFullFileName))
                {
                    using (var oFileStream = new FileStream(TaskFullFileName, FileMode.Open))
                    {
                        var oXmlSerializer = new XmlSerializer(typeof(BindingList<TaskInfo>));
                        tasklist = (BindingList<TaskInfo>)oXmlSerializer.Deserialize(oFileStream);
                    }
                }

                foreach (TaskInfo taskInfo in tasklist)
                {
                    //尋找對應插件
                    if (taskInfo.ControlBase == null)
                    {
                        taskInfo.SetControlBase(DownloadControlManager.GetDownloadControl(taskInfo.Url));
                    }
                }
            }
            catch { MessageBox.Show("讀取任務失敗"); }
            return tasklist;
        }
    }
}
