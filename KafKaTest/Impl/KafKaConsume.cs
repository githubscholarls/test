using Confluent.Kafka;
using KafKaTest.Interfaces;
using System.Text.Json;

namespace KafKaTest.Impl
{
    public class KafKaConsume: LConsume
    {
        public override void ComsumeMessage(string topic, string groupId,  Action<string> dealMessage, Action<Exception> saveLog = null)
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
                            var value = JsonSerializer.Deserialize(cr.Message.Value, typeof(string));
                            dealMessage(value.ToString());

                            consumer.Commit(cr);//手动提交成功后再提交
                        }
                        catch (ConsumeException e)
                        {
                            if (saveLog != null)
                            {
                                saveLog(e);
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

        public override void ConsumeByCommit(string topic, string groupId, Action<string> dealMessage)
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
                        try
                        {
                            var cr = consumer.Consume(cts.Token);
                            var value = JsonSerializer.Deserialize(cr.Message.Value, typeof(string));
                            dealMessage(value.ToString());
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

        public override void ConsumeNTaskByCommit(string topic, string groupId,  Action<string> dealMessage, int taskCount = 1)
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

        public override void ConsumePartitionMessage(TopicPartition topicPartition, string groupId,Action<string> dealMessage)
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
                            dealMessage(cr.Message.Value);
                            consumer.Commit(cr);//手动提交成功后再提交
                        }
                        catch (ConsumeException e)
                        {
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
