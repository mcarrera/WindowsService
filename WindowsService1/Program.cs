using System.ServiceProcess;

namespace WindowsService1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new TempFilesCounterService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
