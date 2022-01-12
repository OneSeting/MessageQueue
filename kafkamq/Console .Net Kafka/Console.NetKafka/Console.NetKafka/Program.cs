// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using KafKaService;

Console.WriteLine("Hello, World!");



Programs.Main();

//Main01();
//Main03();




/// <summary>
/// 生产
/// </summary>
static void Main01()
{
    var config = new ProducerConfig { BootstrapServers = "120.79.77.91:9092" };

    Action<DeliveryReport<Null, string>> handler = r =>
        Console.WriteLine(!r.Error.IsError
            ? $"Delivered message to {r.TopicPartitionOffset}"
            : $"Delivery Error: {r.Error.Reason}");


    using (var p = new ProducerBuilder<Null, string>(config).Build())
    {
        try
        {
            for (var i = 1; i <= 10; i++)
            {
                p.Produce("test", new Message<Null, string> { Value = $"my message: {i}" }, handler);
            }

            p.Flush(TimeSpan.FromSeconds(10));

        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
        }
    }
    Console.WriteLine("Done!");
    Console.ReadKey();
}


/// <summary>
/// 消费
/// </summary>
static void Main03()
{
    var conf = new ConsumerConfig
    {
        GroupId = "test-consumer-group",
        BootstrapServers = "120.79.77.91:9092",
        AutoOffsetReset = AutoOffsetReset.Earliest,

        EnableAutoOffsetStore = false//<----this
    };
    using (var consumer = new ConsumerBuilder<Ignore, string>(conf)
        .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
        .Build())
    {

        consumer.Subscribe("test");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
        };

        try
        {
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(cts.Token);
                    Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: ${consumeResult.Message.Value}");
                    consumer.StoreOffset(consumeResult);//<----this
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ensure the consumer leaves the group cleanly and final offsets are committed.
            consumer.Close();
        }
    }
}


class Programs
{
    public static void Main()
    {
        var bootstrapServers = new string[] { "192.168.209.133:9092", "192.168.209.134:9092", "192.168.209.135:9092" };
        var bootstrapServer = new string[] { "120.79.77.91:9092" };
        var group1 = "group.1";
        var group2 = "group.2";
        var topic = "test";

        {
            KafkaConsumer consumer = new KafkaConsumer(group1, bootstrapServer);
            consumer.EnableAutoCommit = false;
            consumer.ListenAsync(new KafkaSubscriberModel[] { new KafkaSubscriberModel() { Topic = topic, Partition = 0 } }, result =>
            {
                Console.WriteLine($"{group1} recieve message:{result.Message}");
                result.Commit();//手动提交，如果上面的EnableAutoCommit=true表示自动提交，则无需调用Commit方法
            }).Wait();
        }

        {
            KafkaConsumer consumer = new KafkaConsumer(group2, bootstrapServer);
            consumer.EnableAutoCommit = false;
            consumer.ListenAsync(new KafkaSubscriberModel[] { new KafkaSubscriberModel() { Topic = topic, Partition = 1 } }, result =>
            {
                Console.WriteLine($"{group2} recieve message:{result.Message}");
                result.Commit();//手动提交，如果上面的EnableAutoCommit=true表示自动提交，则无需调用Commit方法
            }).Wait();
        }

        KafkaProducer producer = new KafkaProducer(bootstrapServer);

        int index = 0;
        while (true)
        {
            Console.Write("请输入消息:");
            var line = Console.ReadLine();

            int partition = index % 3;
            producer.Publish(topic, partition, "Test", line);
            index++;
        }
    }
}
