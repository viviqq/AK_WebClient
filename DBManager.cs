using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;

namespace WebClient
{
    class DBManager
    {
        public static int count = 0;
   // SqlConnection sqlConnection = new SqlConnection("server=192.168.1.2;uid=sa;pwd=12345678;database=hxlh;");
     SqlConnection sqlConnection = new SqlConnection("server=.;uid=sa;pwd=mlm@2012SQL1435#*!;database=hxlh;");

        public void ClassToSQL(Dictionary<string,string> dic)
        {       
          // SqlConnection sqlConnection = new SqlConnection("Data Source=.;Initial Catalog=hxlh;uid=sa;pwd=12345678;Integrated Security=True");

            if (IsExist(dic["ordercode"],dic["ln_no"]) == false)
            {

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("Select * from orders", sqlConnection);

                SqlCommandBuilder bldr = new SqlCommandBuilder(sqlDataAdapter);
                sqlDataAdapter.InsertCommand = bldr.GetInsertCommand();
                //创建DataSet

                DataSet dataSet = new DataSet("OrderIn");
                //确定现有 DataSet 架构与传入数据不匹配时需要执行的操作。
                sqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                sqlDataAdapter.Fill(dataSet, "orders");
                //DataRow表示DataTable中的一行数据
                System.Data.DataRow dataRow1;
                dataRow1 = dataSet.Tables["orders"].NewRow();
                dataRow1["ordercode"] = dic["ordercode"];
        
                dataRow1["orderstatus"] = dic["orderstatus"];
                dataRow1["saler"] = dic["sellerSignerFullname"];
                dataRow1["mj_name"] = dic["mj_name"];
                dataRow1["mj_email"] = dic["mj_email"];
                dataRow1["orderdt"] = dic["orderdt"];
                dataRow1["paydt"] = dic["paydt"];
                dataRow1["shr"] = dic["contactPerson"];
                dataRow1["mjxzwl"] = dic["mjxzwl"];
                dataRow1["wlfy"] = dic["wlfy"];
                string nation = getCountryFullName(dic["nation"]);
                dataRow1["nation"] = nation;
                dataRow1["province"] = dic["province"];
                dataRow1["city"] = dic["city"];
                dataRow1["addr"] = dic["addr"];
                dataRow1["code"] = dic["code"];
                if (dic.ContainsKey("phone"))
                {
                    string temp;
                    temp = dic["phone"].ToString().Trim();
                    temp.Replace("-", "");
                    if (temp.Length <6 || temp.Length > 20)
                    {
                        dataRow1["phone"] = "";
                    }
                    else
                    {
                        dataRow1["phone"] = dic["phone"];
                    }


                }
                else
                {
                    dataRow1["phone"] = "";
                }
                if (dic.ContainsKey("mobile"))
                {
                    string temp2 = dic["mobile"].ToString().Trim();
                    temp2.Replace("-", "");
                    if (temp2.Length < 6 || temp2.Length > 20)
                    {
                        dataRow1["mobile"] = "";
                    }
                    else
                    {
                        dataRow1["mobile"] = dic["mobile"];
                    }
                }
                else
                {
                    dataRow1["mobile"] = "";
                }

                if (dataRow1["mobile"].ToString() == "" &&dataRow1["phone"].ToString() == "")
                {
                    dataRow1["isyc"] = 1;
                }

               //除了1其他项总金额栏都填写0

                if (dic["ln_no"] == "1")
                {
                    dataRow1["all_sum"] = dic["all_sum"];
                }
                else
                {
                    dataRow1["all_sum"] = 0;
                }


                dataRow1["projects"] = dic["projects"];
                if (dic["skuCode"] == "")
                {
                    dataRow1["item_bh"] = dic["item_bh"];
                }
                else
                {
                    dataRow1["item_bh"] = dic["skuCode"];
                }

                if (!IsItemBhExist(dataRow1["item_bh"].ToString()))
                {
                    dataRow1["isyc"] = 1;
                }

                string itemName = GetItemName(dataRow1["item_bh"].ToString());
                 if(itemName!="")
                {
                    dataRow1["item_nm"] = itemName;
                }
             


                if (dic.ContainsKey("color"))
                {
                    dataRow1["color"] = dic["color"];
                }

                if (dic.ContainsKey("size"))
                {
                    dataRow1["size"] = dic["size"];
                }

                if (dic.ContainsKey("cup size"))
                {
                    string size = dic["cup size"];
                    if (dic.ContainsKey("bands size"))
                    {
                        size = size +"/"+ dic["bands size"];
                    }
                    dataRow1["size"] = size;
                }




                if (dic.ContainsKey("shoe us size"))
                {
                    string shoetype ;
                    //US码转Eur码
                    switch (dic["deptid"])
                    {
                        case "1012":  shoetype = "men"; break ;//000
                        case "1017": shoetype = "men"; break;//500
                        case "1016": shoetype = "women"; break;//600
                        case "1013": shoetype = "women"; break;//900
                        default: shoetype = ""; break;//不在范围内
                    }

                    string type = dataRow1["item_bh"].ToString().Substring(0, 2);

                    if (type == "XW")
                    {
                        shoetype = "women";
                    }
                    else if (type == "XM")
                    {
                        shoetype = "men";
                    }




                    if (shoetype != "")
                    {
                        string eurSize = ConvertShoeSize(shoetype, dic["shoe us size"]);
                        if (eurSize != "")
                        {
                            dataRow1["size"] = eurSize;
                        }
                        else
                        {
                            dataRow1["size"] = dic["shoe us size"];
                        }
                    }
                    else
                    {
                        dataRow1["size"] = dic["shoe us size"];
                    }
                                       
                    dataRow1["mem"] = "shoe us size：" + dic["shoe us size"];
                }

                if (dic["unit"].Trim().ToLower().Contains("lot")) //单位是lot的订单也为异常
                {
                    dataRow1["isyc"] = 1;
                    dataRow1["mem"] = dic["lotNum"].ToString()+"piece"  + "/ Lot ;  "+ dic["amount"].ToString()+" Lot";                 
                    dataRow1["amount"] = (Int32.Parse(dic["lotNum"]) * Int32.Parse(dic["amount"])).ToString();
                    dataRow1["unit"] = "piece";
                }
                else
                {
                    dataRow1["amount"] = dic["amount"];
                    dataRow1["unit"] = dic["unit"];
                }
                

                
                dataRow1["item_sum"] = dic["item_sum"];
                if (dic["memo"].Trim() != "")
                {
                    dataRow1["isyc"] = 1;
                }

                dataRow1["ordermem"] = dic["memo"];
                dataRow1["import_dt"] = DateTime.Now;
                dataRow1["shaddr"] = dic["shaddr"];
                dataRow1["ln_no"] = dic["ln_no"];
                dataRow1["deptid"] = dic["deptid"];
                dataRow1["operator"] = "自动导入";
                dataRow1["status"] = "0";
               


                dataSet.Tables["orders"].Rows.Add(dataRow1);
                sqlDataAdapter.Update(dataSet, "orders");
                sqlConnection.Close();
                count++;
            }
            else
            {
            }
              

        }

        public DateTime getLastImportDate()
        {
       //     string strsql = "Select top 1 import_dt from orders order by import_dt desc";
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("Select top 1 import_dt from orders order by import_dt desc", sqlConnection);

            SqlCommandBuilder bldr = new SqlCommandBuilder(sqlDataAdapter);
            sqlDataAdapter.SelectCommand.Connection = sqlConnection;

            DataSet dataSet = new DataSet() ;
            //确定现有 DataSet 架构与传入数据不匹配时需要执行的操作。
          //  sqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            string lastdt;
            sqlDataAdapter.Fill(dataSet,"tb_date");
            if (dataSet.Tables["tb_date"].Rows.Count == 0) //没有数据
            {
                lastdt = DateTime.Now.ToShortDateString();
            }
            else
            {
            lastdt = dataSet.Tables["tb_date"].Rows[0]["import_dt"].ToString();

            }
          
            sqlConnection.Close();
            DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy/mm/dd";
            return Convert.ToDateTime(lastdt, dtFormat);
        }


        public Dictionary<string, string> GetShopInfo()
        {
            string strsql = "select shopNum,refreshTokenKey,app_key,sign_key from shopKey";
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(strsql,sqlConnection);
            SqlCommandBuilder bldr = new SqlCommandBuilder(sqlDataAdapter);
            DataSet dataSet = new DataSet();
            sqlDataAdapter.Fill(dataSet, "tb_shop");
            DataTable shopTb = dataSet.Tables["tb_shop"];
            int RowCount = dataSet.Tables["tb_shop"].Rows.Count;
            Dictionary<string, string> dic_shop = new Dictionary<string, string>();
            for ( int k = 0; k < RowCount; k++)
            {
                dic_shop.Add(shopTb.Rows[k]["shopNum"].ToString(), shopTb.Rows[k]["app_key"].ToString() + shopTb.Rows[k]["refreshTokenKey"].ToString()+ shopTb.Rows[k]["sign_key"].ToString() );
            }

            return dic_shop;
        }

        public bool IsExist(string ordercode,string ln_no)
        {
            string sql = "select ordercode from orders where ordercode = '" + ordercode+"' and ln_no = '"+ln_no+"'";
            sqlConnection.Open();
         
            SqlCommand cmd = new SqlCommand(sql,sqlConnection);
            Object result = cmd.ExecuteScalar();
            sqlConnection.Close();
            if (result != null)
            {
               return true;
            }
            else
            {
               return false;
            }
        }


        public string GetItemName(string itemBh)
        {
            string sql = "select item_nm from item_inf where item_bh ='" + itemBh + "'";
            sqlConnection.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlConnection);
            SqlDataReader dr = cmd.ExecuteReader();
            bool result = dr.Read();

            if (result)
            {
                string name = dr["item_nm"].ToString();
                sqlConnection.Close();
                return name;
            }
            else
            {
                sqlConnection.Close();
                return "";
            }
        }


        public bool IsItemBhExist(string bh)
        {
            string sql = "select item_bh from item_inf where item_bh = '" + bh+"'";
            sqlConnection.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlConnection);
            Object result = cmd.ExecuteScalar();
            sqlConnection.Close();
            if (result != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 根据国家简称获取全称
        /// </summary>
        /// <param name="shortname"></param>
        /// <returns></returns>
        public string getCountryFullName(string shortName)
        {
            string sql = "select nm from Nation where bh = '" + shortName +"'";
            sqlConnection.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlConnection);          
            SqlDataReader dr = cmd.ExecuteReader();
            
         
            bool result = dr.Read();
          
            if (result)
            {
                string name = dr["nm"].ToString();
                sqlConnection.Close();
                return name;
            }
            else
            {
                sqlConnection.Close();
                return shortName;
            }
           


        }


        public string ConvertShoeSize(string shoeType,string size)
        {
            string sql = "select EurSize from ShoeSizeMap where shoetype = '"+ shoeType+"' and USize = '"+size+"'";
            sqlConnection.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlConnection);
            SqlDataReader dr = cmd.ExecuteReader();
            bool result = dr.Read();
            string shoesize = "";
            if (result)
            {
                string name = dr["EurSize"].ToString();
                sqlConnection.Close();
                return name;
            }
            else
            {
                sqlConnection.Close();
                return "";
            }
        }


        



        public string getCount()
        {
            return count.ToString();
        }

        public void initCount()
        {
            count = 0;
        }

       
    }
}
