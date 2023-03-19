using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KafKaTest.Impl.produceClient
{
    internal class ProducerByteClient
    {
        private IProducer<Null, byte[]> _producerByte;
        public readonly static ProducerByteClient Instance = new();
        //private ProduceByteClient()
        //{

        //}
        public IProducer<Null, byte[]> Producer
        {
            get 
            {
                //可以试下 option引入配置
                var config = new ProducerConfig { BootstrapServers = consts.BootStrapServers };
                _producerByte = new ProducerBuilder<Null, byte[]>(config).Build();
                return _producerByte;
            }
        }
    }
}
