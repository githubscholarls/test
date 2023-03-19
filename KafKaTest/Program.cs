using Confluent.Kafka;
using KafKaTest.Impl;
using KafKaTest.Interfaces;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

{

    var group1 = "group1";

    LConsume consume = new KafKaConsume();
    consume.ConsumeByCommit("par3rep3", group1, msg => Console.WriteLine(msg));

}



//学习测试
{
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
}
