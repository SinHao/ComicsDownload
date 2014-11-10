using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;
using ComicsDownload.Control;
using ComicsDownload.Class;

namespace ComicsDownload
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            
        }

        /// <summary>
        /// 整體繫結的資料
        /// </summary>
        BindingList<TaskInfo> TaskList = new BindingList<TaskInfo>();

        /// <summary>
        /// 下載器的設定
        /// </summary>
        ComicsConfig Settings;
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.TaskList = TaskManager.LoadTaskList();
            this.lvTaskList.ItemsSource = TaskList;
            Settings = (ComicsConfig)this.Resources["ComicsDownloadConfig"];
            ConfigManager.LoadSettings(Settings);
            
        }

        private void btStartClick(object sender, RoutedEventArgs e)
        {            
            foreach (TaskInfo task in this.lvTaskList.SelectedItems)
            {
                task.Start();
            }
        }        

        private void btStopClick(object sender, RoutedEventArgs e)
        {
            foreach (TaskInfo task in this.lvTaskList.SelectedItems)
            {
                task.Stop();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            panelAddNewTask.Visibility = Visibility.Visible;
        }

        private void btnAddNewTask_Click(object sender, RoutedEventArgs e)
        {
            string url = txtNewTaskURL.Text;
            abstractDownloadControl downloadControl = Class.DownloadControlManager.GetDownloadControl(url);
            if (downloadControl != null)
            {
                TaskInfo task = new TaskInfo()
                {
                    Url = url,
                    Name = "Unknow",
                    Platform = "Unknow",
                    FilePath = Settings.DefaultFilePath,
                    TaskID = Guid.NewGuid()
                };
                task.SetControlBase(downloadControl);
                TaskList.Add(task);
                task.Analysis();
            }
            txtNewTaskURL.Text = string.Empty;
            panelAddNewTask.Visibility = Visibility.Collapsed;
        }

        private void CloseAddNewPanel_Click(object sender, RoutedEventArgs e)
        {
            panelAddNewTask.Visibility = Visibility.Collapsed;
        }

        private void btnSelectFilePath_Click(object sender, RoutedEventArgs e)
        {
            this.txtFilePath.Text = GetPath();
        }

        private void tcDownType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvTaskList.SelectedItem != null)
            {
                if (tcDownLoadType.SelectedIndex != 1)
                {
                    ((TaskInfo)this.lvTaskList.SelectedItem).IsSingle = false;
                }
                else
                {
                    ((TaskInfo)this.lvTaskList.SelectedItem).IsSingle = true;
                }
            }

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (TaskInfo task in lvTaskList.SelectedItems)
            {
                task.Analysis();
            }
        }

        private void lvTaskList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvTaskList.SelectedItem != null)
            {
                TaskInfo task = (TaskInfo)lvTaskList.SelectedItem;
                switch (task.Status)
                {
                    case DownloadStatus.下載中:
                    case DownloadStatus.任務分析中:                        
                        DisableExtraOptions();
                        break;
                    case DownloadStatus.下載中斷:
                    case DownloadStatus.下載完成:
                    case DownloadStatus.下載停止:
                    case DownloadStatus.出現錯誤:
                    case DownloadStatus.分析完畢:
                        EnabledExtraOptions();
                        break;
                    default:
                        DisableExtraOptions();
                        break;
                }
            }
        }

        private void DisableExtraOptions()
        {
            this.txtComicsName.IsEnabled = false;
            this.txtFilePath.IsEnabled = false;
            this.btnSelectFilePath.IsEnabled = false;
            this.txtLastPage.IsEnabled = false;
            this.txtLastSection.IsEnabled = false;
            this.txtSelectSection.IsEnabled = false;
            this.txtStartPage.IsEnabled = false;
            this.txtStartSection.IsEnabled = false;
        }

        private void EnabledExtraOptions()
        {
            this.txtComicsName.IsEnabled = true;
            this.txtFilePath.IsEnabled = true;
            this.btnSelectFilePath.IsEnabled = true;
            this.txtLastPage.IsEnabled = true;
            this.txtLastSection.IsEnabled = true;
            this.txtSelectSection.IsEnabled = true;
            this.txtStartPage.IsEnabled = true;
            this.txtStartSection.IsEnabled = true;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("是否刪除所選取的任務", "刪除任務", MessageBoxButton.YesNo, MessageBoxImage.Question) == System.Windows.MessageBoxResult.No) 
            {
                return;
            }

            List<Guid> taskGuidList = new List<Guid>();
            foreach (TaskInfo task in lvTaskList.SelectedItems)
            {                
                taskGuidList.Add(task.TaskID);                
            }

            lvTaskList.SelectedItems.Clear();

            foreach (Guid taskGuid in taskGuidList)
            {
                TaskInfo task = TaskList.First(_task => _task.TaskID == taskGuid);
                task.Stop();  
                TaskList.Remove(task);
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            this.panelSetComicsDownload.Visibility = Visibility.Visible;
        }

        private void SetConfig_Click(object sender, RoutedEventArgs e)
        {            
            ConfigManager.SaveSettings(Settings);
            panelSetComicsDownload.Visibility = Visibility.Collapsed;
        }

        private void btnShowFilePathWindow_Click(object sender, RoutedEventArgs e)
        {
            Settings.DefaultFilePath = GetPath();
        }

        private void btnClosePanelSetComicsDownload_Click(object sender, RoutedEventArgs e)
        {
            this.panelSetComicsDownload.Visibility = Visibility.Collapsed;
        }

        private string GetPath()
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                fbd.ShowDialog();
                if (!string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    return fbd.SelectedPath;
                }
            }            
            return "";
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Cursor = Cursors.Wait;
            foreach (TaskInfo task in TaskList)
            {
                //先強制終止執行續
                if (ThreadManager.ThreadList.ContainsKey(task.ThreadID))
                {
                    ThreadManager.ThreadList[task.ThreadID].Abort();
                }
            }
            //後儲存任務
            TaskManager.SaveTaskList(this.TaskList);
        }
    }
}
