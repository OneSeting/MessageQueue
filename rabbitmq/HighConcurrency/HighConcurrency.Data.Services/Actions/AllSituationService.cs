using HighConcurrency.Data.Model;
using HighConcurrency.Data.ServiceLocators;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HighConcurrency.Data.Services.Actions
{
    public class AllSituationService: BaseSvrAction
    {
        #region Fileds
        private readonly OrdersService ordersService = ServiceLocator.Current.GetInstance<OrdersService>();
        #endregion

        #region Mothon
        /// <summary>
        /// 创建订单队列
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ReturnBox> GetOrderPage(object data)
        {
            if (data == null)
                throw new AggregateException("parameter is not null");

            //UserOrder userOrder = Newtonsoft.Json.JsonConvert.DeserializeObject<UserOrder>(data.ToString()); 
            JObject jObject = JObject.Parse(data.ToString());
            string values = jObject.GetValue("Sku").ToString();
            return UniData(await ordersService.GetOrderSkuCount(values));
        }      
        #endregion
    }
}
