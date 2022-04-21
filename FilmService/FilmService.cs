using FilmUtility;
using System;
using System.ServiceProcess;
using System.Timers;

namespace FilmService
{
    public partial class FilmService : ServiceBase
    {
        public FilmService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string LogKey = "LogKey:" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + DateTime.Now.ToString("ffffff");
            Timer timer = new Timer();
            timer.Interval = 20000;
            timer.Elapsed += new ElapsedEventHandler(SetFilmUrl_msg);
            timer.Start();
            LoggerWriter.Info("****** " + LogKey + "  电子胶片网址获取服务 V 1.1 by X（FilmService） 启动服务******");
            
        }

        protected override void OnStop()
        {
            string LogKey = "LogKey:" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + DateTime.Now.ToString("ffffff");
            LoggerWriter.Info("****** " + LogKey + " 电子胶片网址获取服务 V 1.1 by X（FilmService） 关闭服务******");
        }
        public void SetFilmUrl_msg(object sender, ElapsedEventArgs args)
        {
            string LogKey = "LogKey:" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + DateTime.Now.ToString("ffffff");
            GetFilmUrl.SetFilmUrl_msg(LogKey);
        }
    }
}
