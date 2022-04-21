using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FilmUtility
{
    public class FilmUtility
    {
        public static string Sha1(string list1)
        {
            var enstr = list1;
            var strRes = Encoding.Default.GetBytes(enstr);

            HashAlgorithm iSha = new SHA1CryptoServiceProvider();
            strRes = iSha.ComputeHash(strRes);
            var enText = new StringBuilder();
            foreach (byte iByte in strRes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }


            return enText.ToString();
        }
        public static string GetNowTimeSpanSec(DateTime _time)
        {
            DateTime now = DateTime.UtcNow;
            now = now.AddHours(9);
            TimeSpan ts = now.Subtract(_time);
            int sec = (int)ts.TotalSeconds;
            return sec.ToString();
        }
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="Data">被加密的明文</param>
        /// <param name="Key">密钥</param>
        /// <param name="Vector">向量</param>
        /// <returns>密文</returns>

        public static string AESEncrypt(string Data, string Key, string Vector)
        {
            if (string.IsNullOrEmpty(Data)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(Data);
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(Key),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                BlockSize = 128,
                IV = Encoding.UTF8.GetBytes(Vector)

            };
            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return ByteArrayToHexString(resultArray);
        }
        /// <summary>
        /// 将一个byte数组转换成一个格式化的16进制字符串
        /// </summary>
        /// <param name="data">byte数组</param>
        /// <returns>格式化的16进制字符串</returns>
        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                //16进制数字
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                //16进制数字之间以空格隔开
                //sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            return sb.ToString().ToUpper();
        }
        public static string GetPost(string Url, string jsonParas)
        {
            string strURL = Url;
            //创建一个HTTP请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
            //Post请求方式
            request.Method = "POST";
            //内容类型
            request.ContentType = "application/json";
            //设置参数，并进行URL编码 
            string paraUrlCoded = jsonParas;
            byte[] payload;
            //将Json字符串转化为字节
            payload = Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的ContentLength
            request.ContentLength = payload.Length;
            //发送请求，获得请求流
            Stream writer;
            try
            {
                writer = request.GetRequestStream();//获取用于写入请求数据的Stream对象
            }
            catch (Exception)
            {
                writer = null;
                Console.Write("连接服务器失败!");
            }
            //将请求参数写入流
            writer.Write(payload, 0, payload.Length);
            writer.Close();//关闭请求流
                           // String strValue = "";//strValue为http响应所返回的字符流
            HttpWebResponse response;
            try
            {
                //获得响应流
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }
            Stream s = response.GetResponseStream();
            //  Stream postData = Request.InputStream;
            StreamReader sRead = new StreamReader(s);
            string postContent = sRead.ReadToEnd();
            sRead.Close();
            return postContent;//返回Json数据
        }
        #region Post请求
        /// <summary>
        /// http Post请求
        /// </summary>
        /// <param name="parameterData">参数</param>
        /// <param name="serviceUrl">访问地址</param>
        /// <param name="ContentType">默认 application/json , application/x-www-form-urlencoded,multipart/form-data,raw,binary </param>
        /// <param name="Accept">默认application/json</param>
        /// <returns></returns>
        public static string GetPost(string parameterData, string serviceUrl, string ContentType = "application/json", string Accept = "application/json")
        {
            //先根据用户请求的uri构造请求地址
            //string serviceUrl = string.Format("{0}/{1}", this.BaseUri, uri);
            //创建Web访问对象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            //把用户传过来的数据转成“UTF-8”的字节流
            byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(parameterData);
            myRequest.Method = "POST";
            myRequest.AutomaticDecompression = DecompressionMethods.GZip;
            myRequest.Accept = Accept;
            myRequest.ContentType = "application/json; charset=UTF-8";
            myRequest.ContentLength = buf.Length;
            myRequest.MaximumAutomaticRedirections = 1;
            myRequest.AllowAutoRedirect = true;
            //发送请求
            Stream stream = myRequest.GetRequestStream();
            stream.Write(buf, 0, buf.Length);
            stream.Close();
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
            string returnData = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
            reader.Close();
            myResponse.Close();
            return returnData;
        }
        #endregion
    }
}
