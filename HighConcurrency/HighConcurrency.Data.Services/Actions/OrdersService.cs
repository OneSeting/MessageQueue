using HighConcurrency.Data.Core;
using HighConcurrency.Data.Model;
using HighConcurrency.Data.Model.ModelBox;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HighConcurrency.Data.Services.Actions
{
    public class OrdersService
    {
        #region Fields
        private readonly DapperRepository dapperRepository;
        #endregion

        #region Constructors
        public OrdersService(DapperRepository dapperRepository)
        {
            this.dapperRepository = dapperRepository;
        }
        #endregion

        #region Methods

        public async Task<string> CreateOrders(UserOrder userOrder)
        {
            return "我是内部的方法";
        }

        /// <summary>
        /// 下单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> PlaceAnOrders(object message, bool recursive, string mutexKey)
        {
            try
            {
                if (message == null)
                    throw new AggregateException("parameter is not null");

                using (Mutex mut = new Mutex(initiallyOwned: false, name: mutexKey))
                {
                    try
                    {
                        //上锁，其他线程需等待释放锁之后才能执行处理；若其他线程已经上锁或优先上锁，则先等待其他线程执行完毕
                        mut.WaitOne();
                        //执行处理代码（在调用WaitHandle.WaitOne至WaitHandle.ReleaseMutex的时间段里，只有一个线程处理，其他线程都得等待释放锁后才能执行该代码段）
                        #region
                        //System.Threading.Thread.Sleep(1000);
                        ProductinForMationBox placeAnOrderBox = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductinForMationBox>(message.ToString());
                        ProductinForMation productinForMation = dapperRepository.QueryAsync<ProductinForMation>($"SELECT * FROM productinformation WHERE Sku = '{placeAnOrderBox.Sku}' ").GetAwaiter().GetResult().FirstOrDefault();
                        if (productinForMation.TotalAmount > 0)
                        {
                            productinForMation.TotalAmount = productinForMation.TotalAmount - 1;
                            productinForMation.UpdateDate = DateTime.UtcNow;
                            //int y = await dapperRepository.ExecuteScalarAsync<int>($"update productinformation set TotalAmount={productinForMation.TotalAmount} , UpdateDate='{productinForMation.UpdateDate}' WHERE Sku='{placeAnOrderBox.Sku}' ");                   
                            bool bo = dapperRepository.Update<ProductinForMation>(productinForMation);
                            if (!bo)
                            {
                                Console.WriteLine("---减商品失败了！");
                            }
                        }
                        else
                        {
                            Console.WriteLine("数量小于1了");
                            return false;
                        }
                        return true;
                        #endregion
                    }
                    //当其他进程已上锁且没有正常释放互斥锁时(譬如进程忽然关闭或退出)，则会抛出AbandonedMutexException异常
                    catch (AbandonedMutexException ex)
                    {
                        //避免进入无限递归
                        if (recursive)
                            throw ex;

                        //非递归调用，由其他进程抛出互斥锁解锁异常时，重试执行
                        //MutexExec(mutexKey: mutexKey, action: action, recursive: true);
                    }
                    finally
                    {
                        //释放锁，让其他进程(或线程)得以继续执行
                        mut.ReleaseMutex();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("报错了");
                throw new AggregateException(ex.Message);
            }
        }

        /// <summary>
        /// 获取当前sku的订单总数
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public async Task<int> GetOrderSkuCount(string sku)
        {
            try
            {
                if (string.IsNullOrEmpty(sku))
                    throw new AggregateException("parameter is not null");

                return await dapperRepository.ExecuteScalarAsync<int>($"SELECT TotalAmount FROM productinformation WHERE Sku='{sku}'");
            }
            catch (Exception ex)
            {
                throw new AggregateException(ex.Message);
            }

        }
        #endregion

        #region Utilities
        #endregion
    }
}
