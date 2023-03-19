using Confluent.Kafka;
using KafKaTest.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Security.AccessControl;
using System.Text.Json;

namespace KafKaTest.Impl
{
    /// <summary>
    /// 如何解决多种消费方式下 通用的处理消息流程
    /// </summary>
    /// <typeparam name="THandleMessage"></typeparam>
    public class KafKaConsume<THandleMessage>: LConsume<THandleMessage>
    {
        private readonly ILogger<KafKaConsume<THandleMessage>> logger;

        public KafKaConsume(ILogger<KafKaConsume<THandleMessage>> logger)
        {
            this.logger = logger;
        }
        public override void ComsumeMessage(string topic, string groupId,  Action<THandleMessage> dealMessage, Action<Exception> saveLog = null)
        {
            var config = autoConfig;
            config.GroupId = groupId;
            try
            {
                using var consumer = new ConsumerBuilder<Ignore, byte[]>(config).Build();
                consumer.Subscribe(topic);
                CancellationTokenSource cts = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = consumer.Consume(cts.Token);
                            var value = JsonSerializer.Deserialize(cr.Message.Value, typeof(THandleMessage));
                            if(value != null)
                                dealMessage((THandleMessage)value);
                        }
                        catch (ConsumeException e)
                        {
                            if (saveLog != null)
                            {
                                saveLog(e);
                            }
                        }
                        catch(Exception e)
                        {
                            continue;
                        }
                    }
                }
                catch (OperationCanceledException ex)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    consumer.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void ConsumeByCommit(string topic, string groupId, Action<THandleMessage> dealMessage)
        {
            var config = handleConfig;
            config.GroupId= groupId;
            try
            {
                using var consumer = new ConsumerBuilder<Ignore, byte[]>(config).Build();

                consumer.Subscribe(topic);
                CancellationTokenSource cts = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        ConsumeResult<Ignore, Byte[]> cr = default;
                        try
                        {
                            cr = consumer.Consume(cts.Token);
                            var value = JsonSerializer.Deserialize(cr.Message.Value, typeof(THandleMessage));
                            if(value is not null)
                                dealMessage((THandleMessage)value);
                            consumer.Commit(cr);//手动提交成功后再提交
                        }
                        catch (ConsumeException e)
                        {

                        }
                        catch(Exception e)
                        {
                            if (e.Message.StartsWith("The JSON value could not be converted to "))
                            {
                                consumer.Commit(cr);
                                logger.LogError($"unhandle message topic:{cr?.Topic} offset:{cr?.TopicPartitionOffset.Offset.Value}");
                            }
                        }
                    }
                }
                catch (OperationCanceledException ex)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    consumer.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void ConsumeNTaskByCommit(string topic, string groupId,  Action<THandleMessage> dealMessage, int taskCount = 1)
        {
            try
            {
                for (int i = 0; i < taskCount; i++)
                {
                    _ = Task.Factory.StartNew(() =>
                    {
                        ConsumeByCommit(topic, groupId,  dealMessage);
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
           
        }
        public override void ConsumePartitionMessage(TopicPartition topicPartition, string groupId,Action<THandleMessage> dealMessage)
        {
            var config = handleConfig;
            config.GroupId = groupId;
            config.PartitionAssignmentStrategy = PartitionAssignmentStrategy.RoundRobin;
            try
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Assign(topicPartition);
                CancellationTokenSource cts = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = consumer.Consume(cts.Token);
                            //dealMessage(cr.Message.Value);
                            consumer.Commit(cr);//手动提交成功后再提交
                        }
                        catch (ConsumeException e)
                        {

                        }
                    }
                }
                catch (OperationCanceledException ex)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    consumer.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ConsumeHandle()
        {

        }
    }
}
