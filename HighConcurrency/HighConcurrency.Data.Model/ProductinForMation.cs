using HighConcurrency.Data.Model.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static HighConcurrency.Data.Model.BaseClass.ConcreteClass;

namespace HighConcurrency.Data.Model
{
    [Table("productinformation")]
    public class ProductinForMation: Identity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string Introduction { set; get; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { set; get; }
        /// <summary>
        /// sku
        /// </summary>
        public string Sku { set; get; }
        /// <summary>
        /// 仓库位置
        /// </summary>
        public string WarehouseLocation { set; get; }
        /// <summary>
        /// 供货商
        /// </summary>
        public string Supplier { set; get; }
        /// <summary>
        /// 供货商电话
        /// </summary>
        public string Phone { set; get; }
        /// <summary>
        /// 供货商姓名
        /// </summary>
        public string SupplierName { set; get; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { set; get; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateDate { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { set; get; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalAmount { set; get; }
    }
}
