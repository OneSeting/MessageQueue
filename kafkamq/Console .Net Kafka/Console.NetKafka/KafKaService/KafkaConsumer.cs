﻿using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafKaService
{
    public class KafkaConsumer : IDisposable
    {
        ConsumerBuilder<string, object> builder;
        List<IConsumer<string, object>> consumers;
        bool disposed = false;

        /// <summary>
        /// kafka服务节点
        /// </summary>
        public string BootstrapServers { get; private set; }
        /// <summary>
        /// 群组
        /// </summary>
        public string GroupId { get; private set; }
        /// <summary>
        /// 是否允许自动提交（enable.auto.commit）
        /// </summary>
        public bool EnableAutoCommit { get; set; } = false;

        /// <summary>
        /// 异常事件
        /// </summary>
        public event Action<object, Exception> ErrorHandler;
        /// <summary>
        /// 统计事件
        /// </summary>
        public event Action<object, string> StatisticsHandler;
        /// <summary>
        /// 日志事件
        /// </summary>
        public event Action<object, KafkaLogMessage> LogHandler;

        public KafkaConsumer(string groupId, params string[] bootstrapServers)
        {
            if (bootstrapServers == null || bootstrapServers.Length == 0)
            {
                throw new Exception("at least one server must be assigned");
            }

            this.GroupId = groupId;
            this.BootstrapServers = string.Join(",", bootstrapServers);
            this.consumers = new List<IConsumer<string, object>>();
        }

        #region Private
        /// <summary>
        /// 创建消费者生成器
        /// </summary>
        private void CreateConsumerBuilder()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(KafkaConsumer));
            }

            if (builder == null)
            {
                lock (this)
                {
                    if (builder == null)
                    {
                        ConsumerConfig config = new ConsumerConfig();
                        config.BootstrapServers = BootstrapServers;
                        config.GroupId = GroupId;
                        config.AutoOffsetReset = AutoOffsetReset.Earliest;
                        config.EnableAutoCommit = EnableAutoCommit;
                        //config.EnableAutoOffsetStore = true;
                        //config.IsolationLevel = IsolationLevel.ReadCommitted;
                        //config.MaxPollIntervalMs = 10000;


                        //List<KeyValuePair<string, string>> config = new List<KeyValuePair<string, string>>();
                        //config.Add(new KeyValuePair<string, string>("bootstrap.servers", BootstrapServers));
                        //config.Add(new KeyValuePair<string, string>("group.id", GroupId));
                        //config.Add(new KeyValuePair<string, string>("auto.offset.reset", "earliest"));
                        //config.Add(new KeyValuePair<string, string>("enable.auto.commit", EnableAutoCommit.ToString().ToLower()));
                        //config.Add(new KeyValuePair<string, string>("max.poll.interval.ms", "10000"));
                        //config.Add(new KeyValuePair<string, string>("session.timeout.ms", "10000"));
                        //config.Add(new KeyValuePair<string, string>("isolation.level", "read_uncommitted"));

                        builder = new ConsumerBuilder<string, object>(config);

                        Action<Delegate, object> tryCatchWrap = (@delegate, arg) =>
                        {
                            try
                            {
                                @delegate?.DynamicInvoke(arg);
                            }
                            catch { }
                        };
                        builder.SetErrorHandler((p, e) => tryCatchWrap(ErrorHandler, new Exception(e.Reason)));
                        builder.SetStatisticsHandler((p, e) => tryCatchWrap(StatisticsHandler, e));
                        builder.SetLogHandler((p, e) => tryCatchWrap(LogHandler, new KafkaLogMessage(e)));
                        builder.SetValueDeserializer(new KafkaConverter());
                    }
                }
            }
        }
        /// <summary>
        /// 内部处理消息
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="action"></param>
        private void InternalListen(IConsumer<string, object> consumer, CancellationToken cancellationToken, Action<RecieveResult> action)
        {
            try
            {
                var result = consumer.Consume(cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    if (!EnableAutoCommit && result != null)
                    {
                        cancellationTokenSource.Token.Register(() =>
                        {
                            consumer.Commit(result);
                        });
                    }
                    action?.Invoke(result == null ? null : new RecieveResult(result, cancellationTokenSource));
                }
            }
            catch { }
        }
        /// <summary>
        /// 验证消费主题和分区
        /// </summary>
        /// <param name="subscribers"></param>
        private void CheckSubscribers(params KafkaSubscriberModel[] subscribers)
        {
            if (subscribers == null || subscribers.Length == 0)
            {
                throw new InvalidOperationException("subscriber cann't be empty");
            }

            if (subscribers.Any(f => string.IsNullOrEmpty(f.Topic)))
            {
                throw new InvalidOperationException("topic cann't be empty");
            }
        }
        /// <summary>
        /// 设置监听主题
        /// </summary>
        /// <param name="consumer"></param>
        private void SetSubscribers(IConsumer<string, object> consumer, params KafkaSubscriberModel[] subscribers)
        {
            var topics = subscribers.Where(f => f.Partition == null).Select(f => f.Topic).ToArray();
            var topicPartitions = subscribers.Where(f => f.Partition != null).Select(f => new TopicPartition(f.Topic, new Partition(f.Partition.Value))).ToArray();

            if (topics.Length > 0)
            {
                consumer.Subscribe(topics);
            }

            if (topicPartitions.Length > 0)
            {
                consumer.Assign(topicPartitions);
            }
        }
        /// <summary>
        /// 创建一个消费者
        /// </summary>
        /// <param name="listenResult"></param>
        /// <param name="subscribers"></param>
        /// <returns></returns>
        private IConsumer<string, object> CreateConsumer(ListenResult listenResult, params KafkaSubscriberModel[] subscribers)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(KafkaConsumer));
            }

            CheckSubscribers(subscribers);

            CreateConsumerBuilder();

            var consumer = builder.Build();
            listenResult.Token.Register(() =>
            {
                consumer.Dispose();
            });

            SetSubscribers(consumer, subscribers);

            consumers.Add(consumer);

            return consumer;
        }
        #endregion

        #region Listen
        /// <summary>
        /// 监听一次并阻塞当前线程，直至有消息获取或者取消获取
        /// </summary>
        /// <param name="topics"></param>
        public RecieveResult ListenOnce(params string[] topics)
        {
            return ListenOnce(topics.Select(f => new KafkaSubscriberModel() { Partition = null, Topic = f }).ToArray());
        }
        /// <summary>
        /// 监听一次并阻塞当前线程，直至有消息获取或者取消获取
        /// </summary>
        /// <param name="subscribers"></param>
        public RecieveResult ListenOnce(params KafkaSubscriberModel[] subscribers)
        {
            ListenResult listenResult = new ListenResult();
            var consumer = CreateConsumer(listenResult, subscribers);

            RecieveResult result = null;
            InternalListen(consumer, listenResult.Token, rr =>
            {
                result = rr;
            });
            return result;
        }
        /// <summary>
        /// 异步监听一次
        /// </summary>
        /// <param name="topics"></param>
        /// <returns></returns>
        public async Task<RecieveResult> ListenOnceAsync(params string[] topics)
        {
            return await ListenOnceAsync(topics.Select(f => new KafkaSubscriberModel() { Partition = null, Topic = f }).ToArray());
        }
        /// <summary>
        /// 异步监听一次
        /// </summary>
        /// <param name="subscribers"></param>
        /// <returns></returns>
        public async Task<RecieveResult> ListenOnceAsync(params KafkaSubscriberModel[] subscribers)
        {
            return await Task.Run(() =>
            {
                return ListenOnce(subscribers);
            });
        }
        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public void Listen(string[] topics, Action<RecieveResult> action = null, CancellationToken cancellationToken = default)
        {
            Listen(topics.Select(f => new KafkaSubscriberModel() { Partition = null, Topic = f }).ToArray(), action, cancellationToken);
        }
        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="subscribers"></param>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public void Listen(KafkaSubscriberModel[] subscribers, Action<RecieveResult> action = null, CancellationToken cancellationToken = default)
        {
            ListenResult result = new ListenResult();
            var consumer = CreateConsumer(result, subscribers);
            cancellationToken.Register(() =>
            {
                result.Stop();
            });
            while (!result.Stoped)
            {
                InternalListen(consumer, result.Token, action);
            }
        }
        /// <summary>
        /// 异步监听
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<ListenResult> ListenAsync(string[] topics, Action<RecieveResult> action = null)
        {
            return await ListenAsync(topics.Select(f => new KafkaSubscriberModel() { Partition = null, Topic = f }).ToArray(), action);
        }
        /// <summary>
        /// 异步监听
        /// </summary>
        /// <param name="subscribers"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<ListenResult> ListenAsync(KafkaSubscriberModel[] subscribers, Action<RecieveResult> action = null)
        {
            ListenResult result = new ListenResult();
            new Task(() =>
            {
                var consumer = CreateConsumer(result, subscribers);
                while (!result.Stoped)
                {
                    InternalListen(consumer, result.Token, action);
                }
            }).Start();
            return await Task.FromResult(result);
        }
        #endregion
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            disposed = true;
            builder = null;

            foreach (var consumer in consumers)
            {
                consumer?.Close();
                consumer?.Dispose();
            }

            GC.Collect();
        }

        public static KafkaConsumer Create(string groupId, KafkaBaseOptionsModel kafkaBaseOptions)
        {
            return new KafkaConsumer(groupId, kafkaBaseOptions.BootstrapServers);
        }
        public override string ToString()
        {
            return BootstrapServers;
        }
    }

    public class RecieveResult
    {
        CancellationTokenSource cancellationTokenSource;

        internal RecieveResult(ConsumeResult<string, object> consumeResult, CancellationTokenSource cancellationTokenSource)
        {
            this.Topic = consumeResult.Topic;
            this.Message = consumeResult.Message?.Value?.ToString();
            this.Offset = consumeResult.Offset.Value;
            this.Partition = consumeResult.Partition.Value;
            this.Key = consumeResult?.Message?.Key;

            this.cancellationTokenSource = cancellationTokenSource;
        }

        /// <summary>
        /// Kafka消息所属的主题
        /// </summary>
        public string Topic { get; private set; }
        /// <summary>
        /// 键值
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// 我们需要处理的消息具体的内容
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Kafka数据读取的当前位置
        /// </summary>
        public long Offset { get; private set; }
        /// <summary>
        /// 消息所在的物理分区
        /// </summary>
        public int Partition { get; private set; }

        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            if (cancellationTokenSource == null || cancellationTokenSource.IsCancellationRequested) return;

            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }
    public class ListenResult : IDisposable
    {
        CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// CancellationToken
        /// </summary>
        public CancellationToken Token { get { return cancellationTokenSource.Token; } }
        /// <summary>
        /// 是否已停止
        /// </summary>
        public bool Stoped { get { return cancellationTokenSource.IsCancellationRequested; } }

        public ListenResult()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
