using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HighConcurrency.Data.Locking
{
//    / <summary>
//    / 阻塞锁
//    / </summary>
//    public class BlockingLock
//    {
//        public static void Show(int i, string key, TimeSpan timeout)
//        {
//            using (var client = new ConnectionHelper().Conn())
//            {
//                using (var dataLock = client.AcquireLock("DataLock:" + key, timeout))
//                {
//                    //库存数量
//                    var inventory = client.Get<int>("inventoryNum");
//                    if (inventory > 0)
//                    {
//                        //库存-1
//                        client.Set<int>("inventoryNum", inventory - 1);
//                        //订单数量+1
//                        var orderNum = client.Incr("orderNum");
//                        Console.WriteLine($"{i}抢购成功***线程id:{Thread.CurrentThread.ManagedThreadId.ToString("00")},库存: {inventory},订单数量: {orderNum}");
//                    }
//                    else
//                    {
//                        Console.WriteLine($"{i}抢购失败");
//                    }
//                    Thread.Sleep(100);
//                }
//            }
//        }
//    }


//    / <summary>
//    / 非阻塞锁
//    / </summary>
//    public class ImmediatelyLock
//    {
//        public static void Show(int i, string key, TimeSpan timeout)
//        {
//            using (var client = new ConnectionHelper().Conn())
//            {
//                bool isLocked = client.Add<string>("DataLock:" + key, key, timeout);
//                if (isLocked)
//                {
//                    try
//                    {
//                        //库存数量
//                        var inventory = client.Get<int>("inventoryNum");
//                        if (inventory > 0)
//                        {
//                            //库存-1
//                            client.Set<int>("inventoryNum", inventory - 1);
//                            //订单+1
//                            var orderNum = client.Incr("orderNum");
//                            client.Set<int>("inventoryNum", inventory - 1);

//                            Console.WriteLine($"{i}抢购成功*****线程id：{ Thread.CurrentThread.ManagedThreadId.ToString("00")},库存：{inventory},订单数量：{orderNum}");

//                        }
//                        else
//                        {
//                            Console.WriteLine($"{i}抢购失败: 库存为零");
//                        }
//                    }
//                    catch
//                    {
//                        throw;
//                    }
//                    finally
//                    {
//                        client.Remove("DataLock:" + key);
//                    }
//                }
//                else
//                {
//                    Console.WriteLine($"{i}抢购失败: 没有拿到锁");
//                }
//            }
//        }
//    }


//    / <summary>
//    / lock锁
//    / </summary>
//    public class NormalSecondsKill
//    {
//        private static readonly object _lock = new object();

//        public static void Show()
//        {

//            lock (_lock)
//            {
//                using (var client = new ConnectionHelper().Conn())
//                {
//                    var inventory = client.Get<int>("inventoryNum");   //获取库存
//                    if (inventory > 0)
//                    {
//                        client.Set<int>("inventoryNum", inventory - 1);    //库存-1
//                        var orderNum = client.Incr("orderNum");             //订单+1
//                        Console.WriteLine($"抢购成功*****线程id：{ Thread.CurrentThread.ManagedThreadId.ToString("00")},库存：{inventory},订单数量：{orderNum}");

//                    }
//                    else
//                    {
//                        Console.WriteLine("抢购失败");
//                    }
//                }
//            }
//        }
//    }
}
