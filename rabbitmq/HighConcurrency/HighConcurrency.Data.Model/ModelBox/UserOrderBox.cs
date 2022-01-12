using System;
using System.Collections.Generic;
using System.Text;
using static HighConcurrency.Data.Model.BaseClass.ConcreteClass;

namespace HighConcurrency.Data.Model.ModelBox
{

    public class UserOrderBox : QueryBox
    {
        /// <summary>
        /// 订购人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Gender { set; get; }
        /// <summary>
        /// 年纪
        /// </summary>
        public int? Age { set; get; }
        /// <summary>
        /// 住址
        /// </summary>
        public string Residential { set; get; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { set; get; }
        /// <summary>
        /// 购买产品的sku
        /// </summary>
        public string ProductsPurchasedSku { set; get; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { set; get; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { set; get; }
        /// <summary>
        /// 快递方式 10 陆运  20水运  30空运
        /// </summary>
        public int Courier { set; get; }
        /// <summary>
        /// '交易方式 10 支付宝 20 微信  30信用卡'
        /// </summary>
        public int MeansOfTransaction { set; get; }
        /// <summary>
        /// 加急 1是 0不是
        /// </summary>
        public int Expedited { set; get; }
        /// <summary>
        /// '所有有优惠卷 1是 0不是'
        /// </summary>
        public int Coupon { set; get; }
        /// <summary>
        /// 反馈
        /// </summary>
        public string Feedback { set; get; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateDate { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { set; get; }

    }
}
