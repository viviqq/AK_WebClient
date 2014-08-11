using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace WebClient
{
   
    //在类的上面增加了属性:Serializable.(如果不加这个属性,将抛出SerializationException异常)。
    //在不继承自接口ISerializable的情况下，通过增加[Serializable]属性可以允许该类可以被序列化。
    [Serializable]
    class Order
    {
        public string OrderId { get; set;}
        public string OrderStatus{ get; set;}
        public object receiptAddress;
        public object buyerInfo;
        public string buyerloginid;//买家账号
       // public object logisticsAmount;
        public object childOrderExtInfoList;
        public string id;//订单号
        public string logisticsStatus;//物流状态
        public object orderAmount;
        public string sellerSignerFullname;
        //public object initOderAmount;
        public object childOrderList;
        public string gmtCreate;
        public string sellerOperatorLoginId;//卖家登录账号
        public string gmtPaySuccess;
        public string loanStatus;
        public string fundStatus;


        public string Seller;
        public string BuyerName;
        public string BuyerEmail;
        public string CreateDate;
        public string PayDate;
        public string ProductAmount;
        //满立减
        public string Num;//序号
        public string ProductId;
        public string ProductName;
        //规格
        public string Color;
        public string Size;
        public string Sum;//数量
        public string Unit;//单位
        public string OrderMemo;//订单备注
        public string Memo;//备注
        //导入产品信息
        public string RecAddress;//收件人地址
        public string Receiver;
        public string Country;
        public string Province;
        public string City;
        public string Address;//地址
        public string Zip;//邮编
        public string Phone;//电话
        public string MobilePhone;//手机
        public string logisticsServiceName;//买家选择物流
        public string logisticsNo;//物流单号
        public string GmtSend;//发件日期
        public string gmtReceived;//收件日期


    }
}
