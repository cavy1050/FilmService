using System.Configuration;
using System.Linq;


namespace FilmUtility
{
    public class Common
    {
        private static Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        /// <summary>
        /// 获取指定节点的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                string value = config.AppSettings.Settings[key].Value;
                return value;
            }
            else
            {
                return null;
            }
        }
    }
}
