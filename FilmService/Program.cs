using FilmUtility;
using System.ServiceProcess;

namespace FilmService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new FilmService()
            };
            LoggerWriter.LogerInit();
            ServiceBase.Run(ServicesToRun);
        }
    }
}
