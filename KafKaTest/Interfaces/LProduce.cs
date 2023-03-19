using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafKaTest.Interfaces
{
    public abstract class LProduce
    {
        public abstract void ProduceByBytes<TValue>(string topic, TValue msg);
        public abstract void ProduceByStr<TValue>(string topic, TValue msg);
        public abstract void ProduceByBytes<TValue>(TopicPartition topicPartition, TValue msg);
        public abstract void ProduceByStr<TValue>(TopicPartition topicPartition, TValue msg);
    }
}
