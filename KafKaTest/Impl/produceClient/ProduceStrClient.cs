using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafKaTest.Impl.produceClient
{
    public class ProducerStrClient
    {
        private IProducer<Null, string> _producerStr;
        public readonly static ProducerStrClient Instance = new();
        //private ProduceStrClient()
        //{

        //}
        public IProducer<Null, string> Producer
        {
            get 
            {
                //可以试下 option引入配置
                var config = new ProducerConfig { BootstrapServers = consts.BootStrapServers };
                _producerStr = new ProducerBuilder<Null, string>(config).Build();
                return _producerStr;
            }
        }
    }
}
