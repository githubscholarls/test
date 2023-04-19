using BenchmarkDotNet.Running;

namespace GCTest
{
    public class program
    {
        static void Main(string[] args)
        {
            //性能测试
            BenchmarkRunner.Run<DictoryEqualNull>();

            //gc次数测试
            //var gc = new GCTimes();
            //gc.Start();

            //值任务valueTask
            //BenchmarkRunner.Run<ValueTaskTest>();
        }


        
    }
}
