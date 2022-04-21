using KayiCloud.Guomi.Security;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Text;

namespace FilmUtility
{
    public class GetFilmUrl
    {
        private static bool _SetFilmUrlState = true;
        const string pubkey = "04AC9F7358A9A6749E6FBF02D6938587B1F08D534D8508421D6064633C9E01DBC528B6DF59A123A0A2E74E525CE33ABA1DC01FDD4B820FCFEDBF41C37B81EC1745";

        public static bool SetFilmUrl_msg(string LogKey)
        {
            try
            {
                if (_SetFilmUrlState)
                {
                    _SetFilmUrlState = false;
                    DataTable _RIS_GetFilmUrl;
                    string sql = "select top 50 * from VW_X_RIS_GetFilmUrl where State='0'";
                    _RIS_GetFilmUrl = SqlHelper.ExecuteDt(sql);
                    bool flag = _RIS_GetFilmUrl.Rows.Count > 0;
                    if (flag)
                    {
                        LoggerWriter.Info(LogKey, " 获取需执行接口数据", _RIS_GetFilmUrl.Rows.Count + "条");
                        int num = 0;
                        foreach (object obj in _RIS_GetFilmUrl.Rows)
                        {
                            DataRow dataRow = (DataRow)obj;
                            string Id = dataRow["Id"].ToString();
                            string ApplyNo = dataRow["ApplyNo"].ToString();
                            string ExecTime = dataRow["ExecTime"].ToString();
                            string GetPosFilmStr = PosFilmNo(LogKey, ApplyNo, ExecTime);

                            string[] requestArray = GetPosFilmStr.Split('|');

                            string RequestStr = requestArray[1].ToString();

                            JObject RequestStrJson = JObject.Parse(RequestStr);

                            string code = RequestStrJson["code"].ToString();
                            string message = RequestStrJson["message"].ToString();
                            string content=RequestStrJson["content"].ToString();
                            string RequestUrl = "";
                            string State = "";
                            if (code== "1")
                            {
                                State = "T";
                                RequestUrl = RequestStrJson["content"].ToString();
                            }
                            string sqlstr = "USP_X_UpDateGetFilmUrl_State '{0}','{1}','{2}','{3}','{4}'";
                            sqlstr = string.Format(sqlstr, Id, ApplyNo, State,RequestUrl,GetPosFilmStr);
                            SqlHelper.ExecuteSql(sqlstr);
                            LoggerWriter.Info("****** " + LogKey + " " + sqlstr + "******");
                            if (requestArray[0] == "True") {
                                num++;
                            }
                        }
                        LoggerWriter.Info("****** " + LogKey + "  获取需执行接口数据共" + _RIS_GetFilmUrl.Rows.Count + "条-成功" + num.ToString() + "条-失败" + (_RIS_GetFilmUrl.Rows.Count - num).ToString() + "条******");
                    }
                    _SetFilmUrlState = true;
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerWriter.Error(LogKey, ex);
                _SetFilmUrlState = true;
                return false;
            }
        }
        private static string PosFilmNo(string LogKey, string Applyno ,string DateTime="")
        {
            string JKLX = "FT";
       
            /*卡易接口*/
            string ResultStr = "";
            if (Applyno == "")
            {
                ResultStr = "False|获取编号不能为空";
            }
            switch (JKLX)
            {
                case "FT":
                    string accessionNumber = FilmUtility.AESEncrypt(Applyno, "26ku1blods0pymxj", "3nsvumxb6f51a4y8");
                    string hsCode = "1250000067866859XM";
                    string GetUrlStr = "http://cq.szyxcloud.cn/dimage/index.html?accessionNumber={0}&hsCode={1}&date={2}&noPreview=1";
                    GetUrlStr = string.Format(GetUrlStr, accessionNumber, hsCode, DateTime);
                    string result = "{\"code\":\"1\",\"message\":\"操作成功\",\"content\":\""+ GetUrlStr + "\"}";
                    LoggerWriter.Info(LogKey, "By飞图-生成电子胶片接口数据", result);
                    ResultStr = "True|" + result;
                    break;
                case "KY":
                    string InHospitalCode = "1250000067866859XM";
                    string InTimestamp = FilmUtility.GetNowTimeSpanSec(Convert.ToDateTime("1970-01-01 00:00:00"));
                    string InData = InDataStr("{recordNum:'" + Applyno + "',hospitalCode:'1250000067866859XM' }");
                    string InSignature = InSignatureStr(InHospitalCode, InTimestamp, InData);
                    string InJosn = "{ \"hospitalCode\": \"" + InHospitalCode + "\"," +
                        "\"timestamp\": \"" + InTimestamp + "\", \"signature\": \"" + InSignature.ToUpper() + "\", \"data\":\"" + InData + "\"}";
                    LoggerWriter.Info(LogKey, "执行接口获取传参", InJosn);
                    ResultStr = "True|" + FilmUtility.GetPost("http://10.228.81.142:10008/GetMobileReportURL", InJosn);
                    break;
                default: break;
            }
           
            LoggerWriter.Info(LogKey, "执行接口返回结果", ResultStr);

            return ResultStr;
        }
        private static string InSignatureStr(string InHospitalCode, string InTimestamp, string InData)
        {
            StringBuilder content = new StringBuilder();
            content.Append(InData).Append(InHospitalCode).Append(InTimestamp);
            return FilmUtility.Sha1(content.ToString());
        }
        private static string InDataStr(string Data)
        {
            StringBuilder content = new StringBuilder();
            return SM2Utils.Encrypt(pubkey, Data, System.Text.Encoding.UTF8);
        }
    }
}
