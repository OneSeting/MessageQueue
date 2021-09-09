using HighConcurrency.Data.Model;
using HighConcurrency.Data.ServiceLocators;
using HighConcurrency.Data.Services.Actions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HighConcurrency.Data.MessageQueue
{
    public static class ConsumerMessage
    {
        #region 
        private static OrdersService ordersService = ServiceLocator.Current.GetInstance<OrdersService>();
        #endregion

        /// <summary>
        /// 创建订单的消息队列
        /// </summary>
        public static void CreateOrderMessage()
        {
            try
            {
                //1.实例化连接工厂
                var factory = new ConnectionFactory() { HostName = "localhost" };
                //2. 建立连接
                using (var connection = factory.CreateConnection())
                {
                    //3. 创建信道
                    using (var channel = connection.CreateModel())
                    {
                        #region
                        //channel.ExchangeDeclare(exchange: "XX", type: ExchangeType.Fanout);

                        //var queueName = channel.QueueDeclare().QueueName;
                        //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                        //channel.QueueBind(queue: queueName,exchange: "XX",routingKey: "");

                        //var consumer = new EventingBasicConsumer(channel);
                        //consumer.Received += (model, ea) =>
                        //{
                        //    ReferenceCounting.COUNT = ReferenceCounting.COUNT + 1;
                        //    Console.WriteLine(ReferenceCounting.COUNT + ".已进入信号源，开始方法");
                        //    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        //    ordersService.PlaceAnOrders(message);                        
                        //    Console.WriteLine(ReferenceCounting.COUNT + "方法已完成，消息已确认");
                        //};
                        //channel.BasicConsume(queue: queueName,autoAck: true,consumer: consumer);
                        //Console.ReadLine();
                        #endregion

                        #region 添加的有消息确认的代码
                        //4. 申明队列
                        channel.QueueDeclare(queue: "XP", durable: true, exclusive: false, autoDelete: false, arguments: null);
                        //设置prefetchCount : 1来告知RabbitMQ，在未收到消费端的消息确认时，不再分发消息，也就确保了当消费端处于忙碌状态时
                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                        //5. 构造消费者实例
                        var consumer = new EventingBasicConsumer(channel);
                        //6. 绑定消息接收后的事件委托
                        consumer.Received += (model, ea) =>
                        {
                            ReferenceCounting.COUNT = ReferenceCounting.COUNT + 1;
                            Console.WriteLine(ReferenceCounting.COUNT + ".已进入信号源，开始方法");
                            var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                            //Task.Run(async delegate { await ordersService.PlaceAnOrders(message); }).GetAwaiter().GetResult(); //Thread.Sleep(500);//模拟耗时
                            Task.Run(async delegate { await ordersService.PlaceAnOrders(message, true,"XP"); }) ;
                            // 7. 发送消息确认信号（手动消息确认）
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                            Console.WriteLine(ReferenceCounting.COUNT + "方法已完成，消息已确认");
                        };
                        //8. 启动消费者
                        //autoAck:true；自动进行消息确认，当消费端接收到消息后，就自动发送ack信号，不管消息是否正确处理完毕
                        //autoAck:false；关闭自动消息确认，通过调用BasicAck方法手动进行消息确认
                        channel.BasicConsume(queue: "XP", autoAck: false, consumer: consumer);
                        Console.ReadLine(); //卡住线程 相当于一个长链接
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AggregateException(ex.Message);
            }
        }
    }
}
