using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CopyFilesWinService
{
    partial class FileService : ServiceBase
    {
        bool _disposed = false;
        public FileService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            stLapse.Start();
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            stLapse.Stop();
        }

        private void stLapse_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_disposed) return;

            try
            {
                _disposed = true;
                EventLog.WriteEntry("The files copy process started", EventLogEntryType.Information);

                string pathOriginFiles = ConfigurationManager.AppSettings["pathOriginFiles"];
                string pathDestinyFiles = ConfigurationManager.AppSettings["pathDestinyFiles"];

                if (!Directory.Exists(pathOriginFiles))
                    throw new Exception("message");
                if (!Directory.Exists(pathDestinyFiles))
                    throw new Exception("error");

                DirectoryInfo filesOrigin = new DirectoryInfo(pathOriginFiles);
                foreach (var file in filesOrigin.GetFiles("*", SearchOption.AllDirectories))
                {
                    file.Attributes = FileAttributes.Normal;
                    file.CopyTo(pathDestinyFiles + file.Name, true);
                    //File.Copy(pathDestinyFiles, file.Name, true);
                    //File.SetAttributes(pathDestinyFiles + file.Name, FileAttributes.Normal);

                    if (File.Exists(pathDestinyFiles + file.Name))
                        EventLog.WriteEntry("The files copy was succesfull.", EventLogEntryType.Information);
                    else
                        EventLog.WriteEntry("The files copy was succesfull.", EventLogEntryType.Warning);
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex);
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                //throw;
            }

        }
    }
}

