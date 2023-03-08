using Confluent.Kafka;
using System.Diagnostics;
using System.Net;
using static Confluent.Kafka.ConfigPropertyNames;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


var config = new ProducerConfig
{
    BootstrapServers = "host1:9092",
};
//低吞吐量
//using (var producer = new ProducerBuilder<Null, string>(config).Build())
//{
//    var result = await producer.ProduceAsync("weblog", new Message<Null, string> { Value = "a log message" });
//}

//高吞吐量
void handler(DeliveryReport<Null, string> ex)
{
}
void process()
{
    using (var producer = new ProducerBuilder<Null, string>(config).Build())
    {
        producer.Produce(
        "my-topic", new Message<Null, string> { Value = "hello world" }, handler);
    }
    
}