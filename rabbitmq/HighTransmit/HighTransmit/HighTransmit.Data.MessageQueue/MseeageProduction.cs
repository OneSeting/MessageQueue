using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HighTransmit.Data.MessageQueue
{
    public class MseeageProduction
    {

        public async Task<bool> MessageSend(object data)
        {
            try
            {
                #region
                //1.1.实例化连接工厂
                var factory = new ConnectionFactory() { HostName = "localhost" };
                //2. 建立连接
                using (var connection = factory.CreateConnection())
                {
                    //3. 创建信道
                    using (var channel = connection.CreateModel())
                    {
                        #region 通过指定durable:true，并指定Persistent=true，来告知RabbitMQ将消息持久化。 生产端设置后 消费端也要设置  
                        ///************注意如果 要改成消息持久化 那么queue 的名字不能是以前的 因为以前已经可能有服务占用了 ************///

                        //4. 申明队列(指定durable:true,告知rabbitmq对消息进行持久化)
                        channel.QueueDeclare(queue: "XP", durable: true, exclusive: false, autoDelete: false, arguments: null);

                        //将消息标记为持久性 - 将IBasicProperties.SetPersistent设置为true
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        //5. 构建byte消息数据包
                        string message = data.ToString();
                        var body = Encoding.UTF8.GetBytes(message);

                        //6. 发送数据包(指定basicProperties)
                        channel.BasicPublish(exchange: "", routingKey: "XP", basicProperties: properties, body: body);
                        ReferenceCounting.Count = ReferenceCounting.Count + 1;
                        Console.WriteLine($" {ReferenceCounting.Count} [x] Sent {message}");
                        #endregion
                    }
                }
                return true;
                #endregion

            }
            catch (Exception ex)
            {
                throw new AggregateException(ex.Message);
            }
        }
    }
}
