using Confluent.Kafka;
using KafKaTest.Impl.produceClient;
using KafKaTest.Interfaces;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KafKaTest.Impl
{
    internal class KafKaProduce:LProduce
    {
        private static IProducer<Null, string> _strProducer = ProducerStrClient.Instance.Producer;
        private static IProducer<Null, byte[]> _byteProducer= ProducerByteClient.Instance.Producer;
        public override void ProduceByBytes<TValue>(string topic, TValue msg)
        {

            _byteProducer.Produce(topic, new Message<Null, byte[]> { Value = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(msg) });
        }
        public override void ProduceByStr<TValue>(string topic, TValue msg)
        {
            _strProducer.Produce(topic, new Message<Null, string> { Value = JsonConvert.SerializeObject(msg) });
        }

        public override void ProduceByBytes<TValue>(TopicPartition topicPartition, TValue msg)
        {
            _byteProducer.Produce(topicPartition, new Message<Null, byte[]> { Value = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(msg) });
        }
        public override void ProduceByStr<TValue>(TopicPartition topicPartition, TValue msg)
        {
            _strProducer.Produce(topicPartition, new Message<Null, string> { Value = JsonConvert.SerializeObject(msg) });
        }
    }
}
