using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Net.Security;
using System.Security.Authentication;
namespace WebClient
{
    class AliAPI
    {
        protected static  string app_key = "";
        protected static  string sign_key = "";
        protected static string access_token_main = "";
        public static  string refresh_token_main = "";
        protected static string redirect_uri_cs = "urn:ietf:wg:oauth:2.0:oob";
        protected static string redirect_uri_bs = "";//待定

        protected static string initUrl = "http://gw.api.alibaba.com:80/openapi/param2/1/aliexpress.open/api.findOrderById/";
     //   protected static string initAuthUrl = "http://gw.api.alibaba.com/auth/authorize.htm?client_id=8529544&site=aliexpress&redirect_uri=" + redirect_uri_cs;

        DBManager db = new DBManager();

        public AliAPI(string shopName)
        {
            Dictionary<string, string> dic_shopInfo = db.GetShopInfo();
             app_key = dic_shopInfo[shopName].ToString().Substring(0,7);
             refresh_token_main = dic_shopInfo[shopName].ToString().Substring(7,36);
             sign_key = dic_shopInfo[shopName].ToString().Substring(43);
        }

        public AliAPI()
        {
        }
        
           
             
       
        public string  getOrderList(DateTime startdate,DateTime enddate,int page)
        {
            System.Globalization.DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy/MM/dd";//把格式中默认的-转换为/

            string start = startdate.ToString("MM/dd/yyyy 00:00:00",dtFormat);//MM HH大写
            string end = enddate.ToString("MM/dd/yyyy 00:00:00",dtFormat);

            try
            {
                string reqUrl = "http://gw.api.alibaba.com:80/openapi/param2/1/aliexpress.open/api.findOrderListQuery/"+app_key+"?page="+page+"&pageSize=300&createDateStart="+start+"&createDateEnd="+end+"&orderStatus=WAIT_SELLER_SEND_GOODS&access_token=" + access_token_main;
                return GetUrltoHtml(reqUrl, "utf-8");
            }
            catch (Exception ex)
            {
                     
                throw ex;
 
            }
        }

        /// <summary>
        /// 取得订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public string getOrderDetail(string orderId)
        {
            try
            {
                string reqUrl = initUrl+app_key+"?" + "orderId=" + orderId + "&access_token=" + access_token_main;
                return GetUrltoHtml(reqUrl, "utf-8");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据Url获取返回json值
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetUrltoHtml(string url, string type)
        {
            try
            {
                System.Net.WebRequest wReq = WebRequest.Create(url);
                wReq.ContentType = "application/json; charset=utf-8";
                WebResponse wResp = wReq.GetResponse();
                Stream respStream = wResp.GetResponseStream();
                StreamReader reader = new StreamReader(respStream, Encoding.GetEncoding(type));
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "";
        }

        public string timeConvert(string initime)
        {

            return initime.Substring(0, 4) + "." + initime.Substring(4, 2) +"."+ initime.Substring(6, 2)+" "+initime.Substring(8,2)+":"+initime.Substring(10,2);
        }


        public string arrayJson(object json, string objName,int index)
        {
            Newtonsoft.Json.Linq.JArray _arry = (Newtonsoft.Json.Linq.JArray)json;
            string rtstr ="";

                Dictionary<string, Object> _dictemp = JsonConvert.DeserializeObject<Dictionary<string, object>>(_arry[index].ToString());
                rtstr = _dictemp[objName].ToString();            
                return rtstr;
          
        }

        protected void getAuthorize()
        {
            string aimUrl = "http://gw.api.alibaba.com/auth/authorize.htm?client_id=xxx&site=aliexpress&redirect_uri=YOUR_REDIRECT_URL&state=YOUR_PARM&_aop_signature=SIGENATURE";

        }

        ///获取签名
        //urlPath: 基础url部分，格式为protocol/apiVersion/namespace/apiName/appKey，如 json/1/system/currentTime/1；如果为客户端授权时此参数置为空串""
        //paramDic: 请求参数，即queryString + request body 中的所有参数
        public string sign(string urlPath, Dictionary<string, string> paramDic)
        {
            byte[] signatureKey = Encoding.UTF8.GetBytes("1tROOV4EnfcD");//此处用自己的签名密钥
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> kv in paramDic)
            {
                list.Add(kv.Key + kv.Value);
            }
            list.Sort();
            string tmp = urlPath;
            foreach (string kvstr in list)
            {
                tmp = tmp + kvstr;
            }

            //HMAC-SHA1
            HMACSHA1 hmacsha1 = new HMACSHA1(signatureKey);
            hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(tmp));
            /*
            hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(urlPath));
            foreach (string kvstr in list)
            {
                hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(kvstr));
            }
             */
            byte[] hash = hmacsha1.Hash;
            //TO HEX
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();
        }




        /// <summary>
        /// 通过零时码获取accessToken码
        /// </summary>
        /// <param name="tempCode"></param>
        /// <returns></returns>
        public string getAccessTokenByTempCode(string tempCode)
        {
         
            string url = "http://gw.api.alibaba.com/auth/authorize.htm?client_id="+app_key+"&site=aliexpress&redirect_uri=urn:ietf:wg:oauth:2.0:oob&_aop_signature=" + tempCode;
            WebRequest wbreq = WebRequest.Create(url);
            wbreq.Method = "Post";
            WebResponse rep = wbreq.GetResponse();
            return "";
        }

       /// <summary>
       /// 获取零时码
       /// </summary>
       /// <returns></returns>
        public string GetTempCode()
        {
             Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("client_id", app_key);
            dic.Add("redirect_uri", "urn:ietf:wg:oauth:2.0:oob");
            dic.Add("site", "aliexpress");
            string aop_signature = sign("", dic);
            string url = "http://gw.api.alibaba.com/auth/authorize.htm?client_id="+ app_key +"&site=aliexpress&redirect_uri=urn:ietf:wg:oauth:2.0:oob&_aop_signature="+aop_signature;
            System.Diagnostics.Process.Start("Chrome.exe", url);
            return "";
        }

       /// <summary>
       /// 根据refresh_Token获取access_Token
       /// </summary>
       /// <param name="refreshToken"></param>
       /// <returns></returns>
        public void refreshTokenGetAccessToken()
        {
            string baseUrl = "https://gw.api.alibaba.com:443/openapi/http/1/system.oauth2/getToken/"+ app_key +"?grant_type=refresh_token&client_id="+ app_key+"&client_secret="+sign_key+"&refresh_token=" + refresh_token_main;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(baseUrl);
            req.Method = "Post";
            req.ContentType = "application/x-www-form-urlencoded";//关键
            Stream myRequestStream2 = req.GetRequestStream();
            StreamWriter myStreamWriter2 = new StreamWriter(myRequestStream2);
            myStreamWriter2.Close();
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();    
            Stream myResponseStream2 = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream2);
            string retString = myStreamReader.ReadToEnd();
            Dictionary<string, Object> _dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(retString);
            access_token_main = _dic["access_token"].ToString();
           // return access_token_main;
        }
   



    }
}
