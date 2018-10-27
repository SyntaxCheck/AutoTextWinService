using SynUtil.FileSystem;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTextWindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Using the ManagedInstallerClass we can register the Service, and remove the service
        /// Parameter 0 will be the install/uninstall parm
        /// (Optional) Parameter 1 will be the file to log exceptions or any issues
        /// </summary>
        static void Main(string[] args)
        {
            if (System.Environment.UserInteractive)
            {
                string parameter = String.Empty, pathToLog = String.Empty;
                parameter = args[0];
                if (args.Length > 1)
                {
                    pathToLog = args[1];
                }

                try
                {
                    //Create the log file before we attempt install/uninstall
                    if (!String.IsNullOrEmpty(pathToLog))
                    {
                        if (!File.Exists(pathToLog))
                            File.Create(pathToLog).Dispose();
                    }

                    switch (parameter)
                    {
                        case "--install":
                            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });

                            if (!String.IsNullOrEmpty(pathToLog))
                            {
                                Logger.Write(pathToLog, "Successfully installed service!");
                            }
                            break;
                        case "--uninstall":
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });

                            if (!String.IsNullOrEmpty(pathToLog))
                            {
                                Logger.Write(pathToLog, "Successfully installed service!");
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (!String.IsNullOrEmpty(pathToLog))
                    {
                        Logger.Write(pathToLog, "Exception: " + ex.Message + Environment.NewLine + "Inner Exception: " + ex.InnerException);
                    }
                    else
                    {
                        //Log the error in the EventLog
                        System.Diagnostics.EventLog.WriteEntry("AutoText", "Unknown Exception on install. Exception: " + ex.Message + ". Inner Exception: " + ex.InnerException, System.Diagnostics.EventLogEntryType.Error);
                        //Since we are in UserInteractive mode we can show a MessageBox
                        MessageBox.Show("Install/Uninstall process failed. Error: " + ex.Message + Environment.NewLine + Environment.NewLine + "See Event Log for more info.", "Failed to install");
                    }
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new AutoText()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
