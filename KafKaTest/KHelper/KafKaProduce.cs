using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KafKaTest.KHelper
{
    internal static class KafKaProduce
    {
        private static IProducer<Null, byte[]> _producer;

        public static IProducer<Null, byte[]> Producer
        {
            get
            {
                if(_producer == null)
                {
                    var config = new ProducerConfig { BootstrapServers = "192.168.0.220:9092" };
                    _producer = new ProducerBuilder<Null, byte[]>(config).Build();
                }
                return _producer;
            }
        }
        public static void Produce(this IProducer<Null, byte[]> Producer, string topic, string msg)
        {

            Producer.Produce(topic, new Message<Null, byte[]> { Value = JsonSerializer.SerializeToUtf8Bytes(msg) });

            //生产到指定分区
            //Producer.Produce(new TopicPartition("adsf",0), new Message<Null, string> { Value = msg });
        }
    }
}
