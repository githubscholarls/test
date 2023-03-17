using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KafKaTest.KHelper
{
    public static class KafKaConsume
    {
        public static void ComsumeMessage(string topic,string groupId, Action<string> dealMessage, Action<Exception> saveLog = null)
        {
            var config = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = "192.168.0.220:9092",
                EnableAutoCommit = false,  // true:默认值   允许自动提交    会造成数据没处理 丢失  情况I
                //AutoCommitIntervalMs = 20000,//自动提交间隔毫秒数
                //StatisticsIntervalMs = 60000,
                //session.timeout.ms是个"逻辑"指标，它指定了一个阈值---10秒，在这个阈值内如果coordinator未收到consumer的任何消息，那coordinator就认为consumer挂了     HeartbeatIntervalMs默认
                //SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,

                //EnableAutoOffsetStore = false,//情况 II
                //PartitionAssignmentStrategy = PartitionAssignmentStrategy.RoundRobin


                //处理未知的partition or topic    必须先投递（自动创建）   或者消费设置自动创建
                AllowAutoCreateTopics = true

            };
            try
            {
                using (var consumer = new ConsumerBuilder<Ignore, byte[]>(config).Build())
                {
                    consumer.Subscribe(topic);
                    CancellationTokenSource cts = new CancellationTokenSource();
                    try
                    {
                        while (true)
                        {
                            try
                            {

                                //指定读取分区
                                //consumer.Assign(new TopicPartition("topic", 1));
                                var cr = consumer.Consume(cts.Token);
                                var value = JsonSerializer.Deserialize(cr.Message.Value, typeof(string));
                                dealMessage(value.ToString());
                                //Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");

                                consumer.Commit(cr);//手动提交成功后再提交
                            }
                            catch (ConsumeException e)
                            {
                                if (saveLog != null)
                                {
                                    saveLog(e);
                                }
                                Console.WriteLine($"Error occured: {e.Error.Reason}");
                                Console.WriteLine("内部1错误：" + e.Error.Reason, "待清理kafka队列清理缓存");

                            }
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        // Ensure the consumer leaves the group cleanly and final offsets are committed.
                        consumer.Close();

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
