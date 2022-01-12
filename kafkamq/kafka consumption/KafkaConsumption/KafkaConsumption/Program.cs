// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using KafkaConsumption;

Console.WriteLine("Hello, World!");

string[] kalist = new string[] { "120.79.77.91:9092", "sun" };

//调用封装的接口
//KafkaService kafkaService = new KafkaService();
//await kafkaService.SubscribeAsync<string>(new string[] { "sun" }, like, new CancellationToken());
//static void like(string like)
//{
//    Console.WriteLine($"这个是接受到的结果:{like}");
//}




//调用下面的方法
KafKa.Main(kalist);

public class KafKa
{
    public static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: .. brokerList topicName");
            // 127.0.0.1:9092 helloTopic
            return;
        }

        var brokerList = args.First();
        var topicName = args.Last();

        Console.WriteLine($"Started consumer, Ctrl-C to stop consuming");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        var config = new ConsumerConfig
        {
            BootstrapServers = brokerList,
            GroupId = "consumer",
            EnableAutoCommit = false,
            StatisticsIntervalMs = 5000,
            SessionTimeoutMs = 6000,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true
        };

        const int commitPeriod = 5;

        using var consumer = new ConsumerBuilder<Ignore, string>(config)
        .SetErrorHandler((_, e) =>
        {
            Console.WriteLine($"Error: {e.Reason}");
        })
        .SetStatisticsHandler((_, json) =>
        {
            Console.WriteLine($" - {DateTime.Now:yyyy-MM-dd HH:mm:ss} > monitoring..");
            //Console.WriteLine($"Statistics: {json}");
        })
        .SetPartitionsAssignedHandler((c, partitions) =>
        {
            Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
        })
        .SetPartitionsRevokedHandler((c, partitions) =>
        {
            Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
        })
        .Build();
        consumer.Subscribe(topicName);

        try
        {
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(cts.Token);

                    if (consumeResult.IsPartitionEOF)
                    {
                        Console.WriteLine($"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                        continue;
                    }

                    Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");

                    if (consumeResult.Offset % commitPeriod == 0)
                    {
                        try
                        {
                            consumer.Commit(consumeResult);
                        }
                        catch (KafkaException e)
                        {
                            Console.WriteLine($"Commit error: {e.Error.Reason}");
                        }
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Closing consumer.");
            consumer.Close();
        }
    }


}