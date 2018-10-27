using SynUtil.FileSystem;
using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace AutoTextWindowsService
{
    public partial class AutoText : ServiceBase
    {
        private Timer timer;
        private LogInfo logInfo;
        private bool LoggerSuccess;
        private DateTime currentDateTime;
        private bool debugActive;

        public AutoText()
        {
            InitializeComponent();
            this.ServiceName = "AutoText";
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                logInfo = new LogInfo();
                logInfo.RootFolder = System.Reflection.Assembly.GetEntryAssembly().Location.ToString();
                logInfo.LogFolder = "LogsFolder";
                logInfo.LogFileName = "AutoText.txt";
                logInfo.AppendDateTime = true;
                logInfo.AppendDateTimeFormat = "yyyyMMdd";
                logInfo.IsDebug = true;

                try
                {
                    Logger.ValidateLogPath(ref logInfo);
                }
                catch (Exception ex)
                {
                    logInfo.PathValidated = false;
                    System.Diagnostics.EventLog.WriteEntry("AutoText", ex.Message, System.Diagnostics.EventLogEntryType.Error);
                }

                timer = new System.Timers.Timer(60000);

                // Hook up the Elapsed event for the timer.
                timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                timer.Enabled = true;

                Logger.Write(logInfo, "AutoText service started");
                LoggerSuccess = true;
            }
            catch (Exception ex)
            {
                string logMessage = "Unknown Exception on service startup. Exception: " + ex.Message + ". Inner Exception: " + ex.InnerException;

                if (logInfo.PathValidated)
                {
                    Logger.Write(logInfo, logMessage);
                }
                else
                {
                    //If we failed to initialize the Logger then write to the Event Log
                    System.Diagnostics.EventLog.WriteEntry("AutoText", logMessage, System.Diagnostics.EventLogEntryType.Error);
                }
                this.Stop(); //End the service
            }
        }

        protected override void OnStop()
        {
            if (LoggerSuccess)
            {
                Logger.Write(logInfo, "AutoText service stopped");
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            timer.Enabled = false; //Pause the timer while we work

            try
            {
                
            }
            catch (Exception ex)
            {
                Logger.Write(logInfo, "General timer exception. Message: " + ex.Message + Environment.NewLine + "Stack Trace: " + ex.StackTrace + Environment.NewLine + "Inner Exception: " + ex.InnerException);
            }

            timer.Enabled = true; //Enable the timer after work is complete
        }

        private string GetLogFileName()
        {
            string logName = Path.Combine(System.Reflection.Assembly.GetEntryAssembly().Location.ToString(), "LogsFolder");
            logName = Path.Combine(logName, "log_" + currentDateTime.Date.ToString("yyyy MM dd") + ".txt");

            return logName;
        }
    }
}
