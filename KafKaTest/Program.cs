using Confluent.Kafka;
using KafKaTest.Domain.DTOS;
using KafKaTest.Impl;
using KafKaTest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var serviceProvider = new ServiceCollection()
.AddSingleton<LProduce, KafKaProduce>()
.AddTransient(typeof(LConsume<>),typeof(KafKaConsume<>))
.AddTransient(typeof(ITestInterfaceDI<>), typeof(TestInterfaceDI<>))
.AddLogging()
.BuildServiceProvider();



//测试分区消息处理
{
    LProduce produce = new KafKaProduce();
    _ = Task.Factory.StartNew(async () =>
    {
        var topicConfig = new TopicPartition("TestPartition", 0);
        var k = 1;
        while (true && k <5000)
        {
            await Task.Delay(2000);

            produce.ProduceByBytes(topicConfig, new RequestMsg() { id = k, name ="partion:0", contacts = new List<string>() { "sadfaf", "as" } });
            Console.WriteLine("0--->"+k++);
        }
    });
    _ = Task.Factory.StartNew(async () =>
    {
        var topicConfig = new TopicPartition("TestPartition", 1);
        var k = 1;
        while (true && k <5000)
        {
            await Task.Delay(3500);

            produce.ProduceByBytes(topicConfig, new RequestMsg() { id = k, name = "partion:1", contacts = new List<string>() { "sadfaf", "as" } });
            Console.WriteLine("1--->" + k++);
        }
    });
    _ = Task.Factory.StartNew(async () =>
    {
        var topicConfig = new TopicPartition("TestPartition", 2);
        var k = 1;
        while (true && k < 5000)
        {
            await Task.Delay(5000);

            produce.ProduceByBytes(topicConfig, new RequestMsg() { id = k, name = "partion:2", contacts = new List<string>() { "sadfaf", "as" } });
            Console.WriteLine("2--->" + k++);
        }
    });
    _ = Task.Factory.StartNew(async () =>
    {
        var topicConfig = new TopicPartition("TestPartition", 3);
        var k = 1;
        while (true && k < 5000)
        {
            await Task.Delay(6500);

            produce.ProduceByBytes(topicConfig, new RequestMsg() { id = k, name = "partion:3", contacts = new List<string>() { "sadfaf", "as" } });
            Console.WriteLine("3--->" + k++);
        }
    });

    //读取   (五个消费者读取一个拥有四个分区的topic)   一个curI一直空闲
    var tasks = new List<Task>();
    for (int i = 0; i < 5; i++)
    {
        int j = i;
        var consume = serviceProvider.GetService<LConsume<RequestMsg>>();
        tasks.Add(Task.Factory.StartNew(() =>
        {
            var curI = j;
            while (true)
            {
                 consume.ConsumeByCommit("TestPartition", "group1", (msg) => { Console.WriteLine("  group1:" + curI+" hash:"+consume.GetHashCode() + "----->" + msg.ToString()); });
            }
        }));
    }

    //读取  两个消费者读取分区topic
    for (int i = 5; i < 7; i++)
    {
        int j = i;
        var consume = serviceProvider.GetService<LConsume<RequestMsg>>();
        tasks.Add(Task.Factory.StartNew(() =>
        {
            var curI = j;
            while (true)
            {
                //一直没有消息可读  可用来做备用
                //consume.ConsumeByCommit("TestPartition", "group1", (msg) => { Console.WriteLine("  " + curI + " hash:" + consume.GetHashCode() + "----->" + msg.ToString()); });

                consume.ConsumeByCommit("TestPartition", "group2", (msg) => { Console.WriteLine("                         group2:" + curI + "   hash:" + consume.GetHashCode() + "----->" + msg.ToString()); });
            }
        }));
    }


    Task.WaitAll(tasks.ToArray());
    Console.ReadKey();
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
