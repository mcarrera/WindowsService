using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WindowsService1
{
    public partial class TempFilesCounterService : ServiceBase
    {
        private int _eventId = 1;
        private const string EventLogName = "TempFilesCounter";
        private const string EventLogSource = "TempFilesCounterService";
        private BackgroundWorker Baa;

        public TempFilesCounterService()
        {
            InitializeComponent();
            // set up a new event log
            eventLog = new EventLog();
            if (!EventLog.SourceExists("MySource"))
            {
                EventLog.CreateEventSource(EventLogSource, EventLogName);
            }
            eventLog.Source = EventLogName;
            eventLog.Log = EventLogSource;

            // set up a background worker
            backGroundWorker = new BackgroundWorker();
            backGroundWorker.WorkerReportsProgress = true;
            backGroundWorker.WorkerSupportsCancellation = true;

            // set up a timer to trigger every minute.  
            var timer = new System.Timers.Timer { Interval = 60000 };    // 60 seconds  
            timer.Elapsed += OnTimer;
            timer.Start();
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            var date = DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
            eventLog.WriteEntry(date, EventLogEntryType.Information, _eventId++);
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
            eventLog.WriteEntry("In OnStart");

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("In onStop.");
        }

        [DllImport("advapi32.dll", SetLastError = true)]
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
