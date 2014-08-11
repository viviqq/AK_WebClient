using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace WebClient
{
    public partial class mainForm : Form
    {
       
        DBManager db = new DBManager();
        Order order = new Order();
        Dictionary<string, string> _dicAtt = new Dictionary<string, string>();
        AliAPI api = new AliAPI();
        DateTime lastIptDt;
        string shopName;
        public mainForm()
        {
            InitializeComponent();
            lastIptDt = db.getLastImportDate();
            lbLastDt.Text = "上次导入日期:"+lastIptDt.ToShortDateString();
            lbNowDt.Text ="当 前 日 期 :"+ DateTime.Now.ToShortDateString();
            dtStart.Value = lastIptDt;
          
        }

        public string GetUrltoHtml(string url, string type)
        {
            try
            {
                WebRequest wReq = WebRequest.Create(url);
                wReq.ContentType = "application/json; charset=utf-8";
                WebResponse wResp = wReq.GetResponse();
                Stream respStream = wResp.GetResponseStream();
                StreamReader reader = new StreamReader(respStream, Encoding.GetEncoding(type));
                txtjson.Text = reader.ReadToEnd();///1`23
                return reader.ReadToEnd();
               
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "";
             
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            BtnTest.Enabled = false;
            Dictionary<string, string> dic_shopInfo = db.GetShopInfo();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = dic_shopInfo.Count;
            progressBar1.Step = 1;
            lbNote.Text = "数据开始导入....";
            foreach (KeyValuePair<string, string> kvp in dic_shopInfo)
            {
                shopName = kvp.Key;
                string app_key = kvp.Value.Substring(0, 7);
                string refreshTokenKey = kvp.Value.Substring(7);
      
                AliAPI api = new AliAPI(kvp.Key);
                api.refreshTokenGetAccessToken();

         
              //  txResult.Text = api.getOrderList(dtStart.Value, dtEnd.Value);
                List<string> orderList = new List<string>();
                orderList.Clear();

                Dictionary<string, Object> _dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(api.getOrderList(dtStart.Value, dtEnd.Value,1));

                int listSum = Int32.Parse(_dic["totalItem"].ToString());

                int pages;
                int imod = listSum % 50;
                if (imod == 0)
                {
                    pages = (int)(listSum / 50);
                }
                else
                {
                    pages = (int)(listSum / 50) + 1; 
                }
                            

                if (listSum != 0)
                {
                    for (int i = 0; i < pages; i++)
                    {
                        if (i > 0)
                        {
                            _dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(api.getOrderList(dtStart.Value, dtEnd.Value, i + 1));
                        }
                  
               
                            Newtonsoft.Json.Linq.JArray temp = (Newtonsoft.Json.Linq.JArray)_dic["orderList"];

                            foreach (object temp2 in temp)
                            {
                                Dictionary<string, Object> _dic2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(temp2.ToString());

                                Newtonsoft.Json.Linq.JArray productList = (Newtonsoft.Json.Linq.JArray)_dic2["productList"];
                                foreach (object temp3 in productList)
                                {
                                    Dictionary<string, Object> _dic3 = JsonConvert.DeserializeObject<Dictionary<string, object>>(temp3.ToString());
                                    string tempint = _dic2["orderId"].ToString();
                                    string memo = "";
                                    if(_dic3.ContainsKey("memo"))
                                    {
                                        memo = _dic3["memo"].ToString();
                                    }
                                    string attribute = _dic3["logisticsServiceName"].ToString() + "|" + memo;
                                    if (!_dicAtt.ContainsKey(tempint))
                                    {
                                        _dicAtt.Add(tempint, attribute);
                                    }
                                    orderList.Add(tempint);
                                    //得到导出的订单号  

                                    txtjson.Text = txtjson.Text + tempint.ToString() + "\n";
                                }
                            }

                    }
                orderList = orderList.Distinct().ToList();
                WriteToSql(orderList);
                }
                #region
                /*
            try
            {
                CookieContainer CookieArray = new CookieContainer();
                //创建http请求
                HttpWebRequest LoginHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.baidu.com");
                //登录数据
                string LoginData = "";////////////**************
                //数据被传输类型
                LoginHttpWebRequest.ContentType = "text/html";
                //数据长度
                LoginHttpWebRequest.ContentLength = 1024;
                //数据传输方法 get或post
                LoginHttpWebRequest.Method = "POST";
                //设置HttpWebRequest的 CookieContainer 为刚才建立的那个 CookieArray
                LoginHttpWebRequest.CookieContainer = CookieArray;
                //获取登录数据流
                Stream myRequestStream = LoginHttpWebRequest.GetRequestStream();
                //StreamWriter
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.Default);
                //把数据写入HttpWebRequest的Request流
                myStreamWriter.Write(LoginData);///**************
                //关闭打开对象
                myStreamWriter.Close();
                myRequestStream.Close();

                //新建一个HttpWebResponse
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)LoginHttpWebRequest.GetResponse();
                //获取一个包含url的cookie集合 CookieCollection
                myHttpWebResponse.Cookies = CookieArray.GetCookies(LoginHttpWebRequest.RequestUri);

                WebHeaderCollection a = myHttpWebResponse.Headers;

                Stream myResponseStream = myHttpWebResponse.GetResponseStream();

                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.Default);

                Txt = myStreamReader.ReadToEnd();
                txResult.Text = Txt;

                //把数据从HttpWebResponse的 Response流中读出
                myStreamReader.Close();
                myResponseStream.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
             **/
                #endregion

               
                lbNote.Text = kvp.Key + "店铺已导入完毕";
                progressBar1.PerformStep();
                
                
            }

            lbNote.Text = "数据全部导入完毕";
            lbCount.Text = "此次已成功导入"+ db.getCount()+"条订单数据";
            db.initCount();
            BtnTest.Enabled = true;
        }

        

        /// <summary>
        /// 把订单信息写入数据库表
        /// </summary>
        /// <param name="list"></param>
        protected void WriteToSql(List<string> list)
        {
            foreach (string orderId in list)
            {
                WriteToSqlById(orderId);
            }
        }

        protected void WriteToSqlById(string ordercode)
        {


         //   Dictionary<string, string> dic_send = new Dictionary<string, string>();
            string Ord_Detail = api.getOrderDetail(ordercode);
         //   dic_send.Add("ordercode", ordercode);
            Order order = new Order();
            order = JsonConvert.DeserializeObject<Order>(Ord_Detail);
            Dictionary<string, Object> _dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(Ord_Detail);

            Newtonsoft.Json.Linq.JArray _arry = (Newtonsoft.Json.Linq.JArray)_dic["childOrderList"];
            int count = _arry.Count;

            for (int i = 0; i < count; i++)
            {
                string temp123321 = _arry[i].ToString();
                Dictionary<string, string> dic_send = new Dictionary<string, string>();
                dic_send.Add("ordercode", ordercode);
                //基础信息
                #region
                string sellname = _dic["sellerSignerFullname"].ToString();
                dic_send.Add("sellerSignerFullname", sellname);
                dic_send.Add("orderstatus", "等待您发货");
                //付款时间
              
                string gmtPaySuccess = _dic["gmtPaySuccess"].ToString();           
                gmtPaySuccess = api.timeConvert(gmtPaySuccess);
                dic_send.Add("paydt", gmtPaySuccess);
                //下单时间
                string gmtCreate = _dic["gmtCreate"].ToString();
                gmtCreate = api.timeConvert(gmtCreate);
                dic_send.Add("orderdt", gmtCreate);
                //买家名称
                string buyername = _dic["buyerSignerFullname"].ToString();
                dic_send.Add("mj_name", buyername);//有疑问
                //收货地址
                Dictionary<string, Object> _receiptAddress = JsonConvert.DeserializeObject<Dictionary<string, object>>(_dic["receiptAddress"].ToString());
                if (_receiptAddress.ContainsKey("mobileNo"))
                {
                    string mobileNo = _receiptAddress["mobileNo"].ToString();//手机
                    dic_send.Add("mobile", mobileNo);
                }
                string contractPerson = _receiptAddress["contactPerson"].ToString();
                dic_send.Add("contactPerson", contractPerson);
                string country = _receiptAddress["country"].ToString();
                dic_send.Add("nation", country);
                string city = _receiptAddress["city"].ToString();
                dic_send.Add("city", city);
                string province = _receiptAddress["province"].ToString();
                dic_send.Add("province", province);
                if (_receiptAddress.ContainsKey("phoneNumber"))
                {
                    string phoneNumber = _receiptAddress["phoneNumber"].ToString(); //联系电话
                    string phoneCountry = _receiptAddress["phoneCountry"].ToString();
                    string phoneArea = "";
                    if (_receiptAddress.ContainsKey("phoneArea"))
                    {
                         phoneArea = _receiptAddress["phoneArea"].ToString();
                    }

                  
                    dic_send.Add("phone", phoneCountry + "-" + phoneArea + phoneNumber);
                }

                string zip = _receiptAddress["zip"].ToString();//邮编
                dic_send.Add("code", zip);//邮政编码
                string contactPerson = _receiptAddress["contactPerson"].ToString();//联系人
                dic_send.Add("shr", contactPerson);
                string detailAddress;
                if (_receiptAddress.ContainsKey("address2"))
                {
                     detailAddress = _receiptAddress["detailAddress"].ToString() + " " + _receiptAddress["address2"].ToString();//详细地址
                }
                else
                {
                     detailAddress = _receiptAddress["detailAddress"].ToString();//详细地址
                }
               
                dic_send.Add("addr", detailAddress);
                dic_send.Add("shaddr", detailAddress + "、" + city + "、" + province + "、" + country);
                //买家信息
                Dictionary<string, object> _buyerInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(_dic["buyerInfo"].ToString());

                string email = _buyerInfo["email"].ToString();
                dic_send.Add("mj_email", email);
                //物流费用
                Dictionary<string, object> _logisticsAmount = JsonConvert.DeserializeObject<Dictionary<string, object>>(_dic["logisticsAmount"].ToString());
                string logistic_amount = _logisticsAmount["amount"].ToString();
                dic_send.Add("wlfy", logistic_amount);
                #endregion


                //子订单信息组合
                #region 
                string productName = api.arrayJson(_dic["childOrderList"], "productName", i);
                //projects 拼装
                string productBh = productName.Substring(0, productName.IndexOf(" "));
                dic_send.Add("item_bh", productBh);
                string probh = "(商家编码:" + productBh + ")";

                //订单价格
                Dictionary<string, object> _orderAmount = JsonConvert.DeserializeObject<Dictionary<string, object>>(_dic["orderAmount"].ToString());
                string order_amount = _orderAmount["amount"].ToString();//订单金额
                dic_send.Add("all_sum", order_amount);

                //产品总价格
                Dictionary<string, object> _initOderAmount = JsonConvert.DeserializeObject<Dictionary<string, object>>(_dic["initOderAmount"].ToString());
       //         string item_amount = _initOderAmount["amount"].ToString();//订单金额

                dic_send.Add("ln_no",(i+1).ToString());//序号

                string lotNum = api.arrayJson(_dic["childOrderList"], "lotNum", i);
                string proCount = api.arrayJson(_dic["childOrderList"], "productCount", i);
                string UnitName = api.arrayJson(_dic["childOrderList"], "productUnit", i);
                string initOrderAmt = api.arrayJson(_dic["childOrderList"], "initOrderAmt", i);
                string skuCode = api.arrayJson(_dic["childOrderList"], "skuCode", i);
                Dictionary<string, object> _childList = JsonConvert.DeserializeObject<Dictionary<string, object>>(initOrderAmt);
                string item_amount = _childList["amount"].ToString();

                string proNum = "(产品数量:" + lotNum + " " + UnitName + ")";
                dic_send.Add("unit", UnitName);
                dic_send.Add("amount", proCount);
                dic_send.Add("item_sum", item_amount);
                dic_send.Add("skuCode", skuCode);
                dic_send.Add("lotNum", lotNum);

                string productAttributes = api.arrayJson(_dic["childOrderList"], "productAttributes", i);
                Dictionary<string, object> _productAttributes = JsonConvert.DeserializeObject<Dictionary<string, object>>(productAttributes);
                Newtonsoft.Json.Linq.JContainer jc = (Newtonsoft.Json.Linq.JContainer)_productAttributes["sku"];

                //    string ordertemp = jc.First.Value<string>("order").ToString();
                string temp = "";
                string ProSX = "";
                if (jc.Count > 0)
                {

                    string Name1 = jc.First.Value<string>("pName").ToString();
                    string value1 = "";

                    value1 = jc.First.Value<string>("selfDefineValue").ToString();
                    if (value1 == "")
                    {
                        value1 = jc.First.Value<string>("pValue").ToString();
                    }
                    dic_send.Add(Name1.ToLower(), value1);


                    string Name2 = "";
                    string value2 = "";

                    string Name3 = "";
                    string value3 = "";

                    string Name4 = "";
                    string value4 = "";


                    if (jc.Count == 2)
                    {
                        Name2 = jc.Last.Value<string>("pName").ToString().ToLower();
                        value2 = jc.Last.Value<string>("pValue").ToString();
                        string selfvalue = jc.Last.Value<string>("selfDefineValue").ToString();
                        if (selfvalue != "")
                        {
                            value2 = selfvalue;
                        }
                        dic_send.Add(Name2, value2);
                    }
                    else if (jc.Count > 2)
                    {
                         if(jc.First.Next.HasValues)  //if (jc.Next.HasValues)
                        {
                            Name2 = jc.First.Next.Value<string>("pName").ToString().ToLower();
                            value2 = jc.First.Next.Value<string>("pValue").ToString();
                            string selfvalue = jc.First.Next.Value<string>("selfDefineValue").ToString();
                            if (selfvalue != "")
                            {
                                value2 = selfvalue;
                            }
                            dic_send.Add(Name2, value2);
                        }
                         if (jc.First.Next.Next.HasValues)
                         {
                             Name3 = jc.First.Next.Next.Value<string>("pName").ToString().ToLower();
                             value3 = jc.First.Next.Next.Value<string>("pValue").ToString();
                             string selfvalue = jc.First.Next.Next.Value<string>("selfDefineValue").ToString();
                             if (selfvalue != "")
                             {
                                 value3 = selfvalue;
                             }
                             dic_send.Add(Name3, value3);
                             
                         }
                         




                    }

                    if (value3 != "")
                    {
                        ProSX = "(产品属性" + Name1 + "：" + value1 + "、" + Name2 + ":" + value2 + "、"+Name3+": "+value3+")";
                    }
                    else
                    {
                        ProSX = "(产品属性" + Name1 + "：" + value1 + "、" + Name2 + ":" + value2 + ")";
                    }

                   


                }
                temp ="【"+ (i+1).ToString()+"】"+ productName + ProSX + proNum;

                string Unit = api.arrayJson(_dic["childOrderList"], "productUnit",i);//单位
                string productCount = api.arrayJson(_dic["childOrderList"], "productCount",i);//单位
                dic_send.Add("projects", temp);              
                dic_send.Add("deptid",shopName);
                //产品属性 
                string jihe = _dicAtt[ordercode];
                int flag = jihe.IndexOf("|");
                string memo;
                string wlname = jihe.Substring(0, flag);
                if (flag == jihe.Length)
                {
                    memo = "";
                }
                else
                {
                    memo = jihe.Substring(flag + 1);
                }
                dic_send.Add("mjxzwl", wlname);
                dic_send.Add("memo", memo);
                #endregion


                db.ClassToSQL(dic_send);
                dic_send.Clear();

            }

           
        }

        private void button1_Click(object sender, EventArgs e)
        {
         DateTime dt = db.getLastImportDate();
         DateTime startdate = DateTime.Now;
          
        }


     /*   public List<string> GetOrderList(DateTime stdate,DateTime endate)
        {          
            List<string> orderList = new List<string>();

            Dictionary<string, Object> _dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(api.getOrderList(stdate,endate));

            Newtonsoft.Json.Linq.JArray temp = (Newtonsoft.Json.Linq.JArray)_dic["orderList"];

            foreach (object temp2 in temp)
            {
                Dictionary<string, Object> _dic2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(temp2.ToString());

                Newtonsoft.Json.Linq.JArray productList = (Newtonsoft.Json.Linq.JArray)_dic2["productList"];
                foreach (object temp3 in productList)
                {
                    Dictionary<string, Object> _dic3 = JsonConvert.DeserializeObject<Dictionary<string, object>>(temp3.ToString());
                    string orderid = _dic2["orderId"].ToString();
                    string attribute = _dic3["logisticsServiceName"].ToString() + "|" + _dic3["memo"].ToString();
                    _dicAtt.Add(orderid, attribute);
                    orderList.Add(orderid);
                    //得到导出的订单号
                    //   orderList.Distinct().ToString();        
                }
            }
            return orderList;
        }*/


        public void getShopInfo()
        {
            Dictionary<string, string> dic_shopInfo = db.GetShopInfo();
            foreach (KeyValuePair<string, string> kvp in dic_shopInfo)
            {
                string shopNm = kvp.Key;
                string app_key = kvp.Value.Substring(0,7);
                string refreshTokenKey = kvp.Value.Substring(7,43);
            }

        }

        public void GetInfoFromJson()
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            lbCount.Text =  db.getCount();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }
}

