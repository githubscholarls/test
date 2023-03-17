using Confluent.Kafka;
using KafKaTest.KHelper;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


//实践
{
    string weekLogDetail = ":";

    {
        //测试kafka

        var topic = "testSendKafKa";
        var groupId1 = "group1";
        var groupId21 = "group21";
        //var groupId22 = "group22";


        //线上未知分区 topic
        {

            //Task.Factory.StartNew(() =>
            //{
            //    Task.Delay(2000);
            //    Console.WriteLine("生产了");
            //    KafKaProduce.Producer.Produce("TestsetIsPriority03171125", "我是测试03171125");
            //});



            KafKaProduce.Producer.Produce("TestsetIsPriority03171125", "我是测试03171125");
            Task.Delay(2000);

            KafKaConsume.ComsumeMessage("TestsetIsPriority03171125", "wt.test.trigger", (msg) => { Console.WriteLine(msg); });

        }




        var t0 = Task.Factory.StartNew(async () =>
        {
            for (int i = 0; i < 100; i++)
            {
                KafKaProduce.Producer.Produce(topic, $"msg{i}");
                Console.WriteLine($"produce sendWlineSetPriority msg{i}");
                await Task.Delay(1000 + i);
            }
        });

        //分区
        //var part1 = new TopicPartition(topic, 1);
        //var part2 = new TopicPartition(topic, 2);
        //var t1 = Task.Factory.StartNew(() =>
        //{
        //    for (int i = 0; i < 100; i++)
        //    {

        //        KafKaProduce.Producer.Produce(part1, new Message<Null, string>() { Value = $"part1-msg{i}" }) ;
        //        Console.WriteLine($"t1 {i} end");
        //        Task.Delay(1000 + i*10);
        //    }
        //});
        //var t2 = Task.Factory.StartNew(() =>
        //{
        //    for (int i = 0; i < 100; i++)
        //    {
        //        KafKaProduce.Producer.Produce(part2, new Message<Null, string>() { Value = $"part2-msg{i}" });
        //        Console.WriteLine($"t2 {i} end");
        //        Task.Delay(1000 + i * 10);
        //    }
        //});

        //Thread thread = new Thread(() =>
        //{
        //    string msg = "consume21";
        //    KafKaConsume.ComsumeMessage(topic, groupId21, (msg => { Console.WriteLine(msg); }));
        //});
        //thread.Start();
        //thread.Join();
        //Console.ReadKey();



        //consume21
        //Task.Factory.StartNew(() =>
        //{
        //    KafKaConsume.ComsumeMessage(topic, groupId21, (msg => { Console.WriteLine("cousume21" + msg); }));

        //});
        ////consume22
        //Task.Factory.StartNew(() =>
        //{
        //    KafKaConsume.ComsumeMessage(topic, groupId21, (msg => { Console.WriteLine("cousume22" + msg); }));
        //});

        KafKaConsume.ComsumeMessage(topic, groupId1, (msg => { Console.WriteLine("consume1" + msg); }));

        Console.ReadKey();
        Console.WriteLine("done");
    }
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
