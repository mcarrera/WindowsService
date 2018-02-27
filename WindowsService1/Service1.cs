using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices; 

namespace WindowsService1
{
    public partial class TempFilesCounterService : ServiceBase
    {
        private int _eventId = 1;
        private const string EventLogName = "TempFilesCounterService";
        private const string EventLogSource = "TempFilesService";
        private BackgroundWorker _backgroundWorker;
        public TempFilesCounterService()
        {
          
            InitializeComponent();
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists(EventLogSource))
            {
                EventLog.CreateEventSource(EventLogSource, EventLogName);
            }

            eventLog1.Source = EventLogSource;
            eventLog1.Log = EventLogName;
            // Set up a background worker
            _backgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            // Set up a timer to trigger every minute.  
            var timer = new System.Timers.Timer {Interval = 60000}; // 60 seconds  
            timer.Elapsed += OnTimer;
            timer.Start();
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs args)  
        {
            // TODO: Insert monitoring activities here.  
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, _eventId++);  
        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.  
            var serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 100000
            };
            
            SetServiceStatus(ServiceHandle, ref serviceStatus);  
            eventLog1.WriteEntry("In OnStart");
            
            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;  
            SetServiceStatus(ServiceHandle, ref serviceStatus);  
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop.");
        }

        [DllImport("advapi32.dll", SetLastError=true)]  
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);  
    }

    public enum ServiceState  
    {  
        SERVICE_STOPPED = 0x00000001,  
        SERVICE_START_PENDING = 0x00000002,  
        SERVICE_STOP_PENDING = 0x00000003,  
        SERVICE_RUNNING = 0x00000004,  
        SERVICE_CONTINUE_PENDING = 0x00000005,  
        SERVICE_PAUSE_PENDING = 0x00000006,  
        SERVICE_PAUSED = 0x00000007,  
    }  

    [StructLayout(LayoutKind.Sequential)]  
    public struct ServiceStatus  
    {  
        public int dwServiceType;  
        public ServiceState dwCurrentState;  
        public int dwControlsAccepted;  
        public int dwWin32ExitCode;  
        public int dwServiceSpecificExitCode;  
        public int dwCheckPoint;  
        public int dwWaitHint;  
    };  
}
