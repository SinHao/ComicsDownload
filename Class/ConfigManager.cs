using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System;

namespace ComicsDownload.Class
{
    public class ConfigManager
    {
        private static string _ConfigFolderPath { get { return System.Environment.CurrentDirectory; } }
        private static string _ConfigFileName { get { return "config.json"; } }
        public static string ConfigFullFileName { get { return Path.Combine(_ConfigFolderPath, _ConfigFileName); } }


        public static void LoadSettings(ComicsConfig settings)
        {            
            try
            {                
                if (File.Exists(ConfigFullFileName))
                {
                    using (StreamReader sr = new StreamReader(ConfigFullFileName, Encoding.UTF8))
                    {
                        var oJsonSerializer = new DataContractJsonSerializer(typeof(ComicsConfig));
                        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(sr.ReadToEnd())))
                        {
                            ComicsConfig tmp = (ComicsConfig)oJsonSerializer.ReadObject(stream);
                            settings.AutoAnalysisTask = tmp.AutoAnalysisTask;
                            settings.DefaultFilePath = tmp.DefaultFilePath;
                        }
                    }
                }                
                else
                {                    
                    SaveSettings(settings);
                }

            }
            catch (Exception ex)
            {

            }            
        }

        public static void SaveSettings(ComicsConfig settings)
        {            
            using (var oFileStream = new FileStream(ConfigFullFileName, FileMode.Create))
            {
                var JsonSerializer = new DataContractJsonSerializer(typeof(ComicsConfig));
                JsonSerializer.WriteObject(oFileStream, settings);
            }
        }
    }

    public class ComicsConfig : CommonDataBind
    {
        private string _DefaultFilePath;
        /// <summary>
        /// 預設下載路徑
        /// </summary>
        public string DefaultFilePath
        {
            get { return this._DefaultFilePath; }
            set
            {
                this._DefaultFilePath = value;
                base.OnPropertyChanged("DefaultFilePath");
            }
        }

        private bool _AutoAnalysisTask;
        /// <summary>
        /// 是否自動重新分析任務
        /// </summary>
        public bool AutoAnalysisTask
        {
            get { return this._AutoAnalysisTask; }
            set
            {
                this._AutoAnalysisTask = value;
                base.OnPropertyChanged("AutoAnalysisTask");
            }
        }

        public ComicsConfig()
        {
            this.DefaultFilePath = Environment.CurrentDirectory;
            this.AutoAnalysisTask = false;
        }
    }
}
