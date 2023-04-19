using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace GCTest
{
    [MemoryDiagnoser]
    public class DictoryEqualNull
    {
        //                  Mean    Error   stdDev  Gen0    Allocated
        //DictorySetNull   117us    2.9us   8.1us   77      237k
        //DIctoryClear      122us   2.4us   4.38us  48      147.k
        /*
        Mean: Arithmetic mean of al1 measurements
        Error: Half of 99.9% confidence interva1
        StdDev: Standard deviation of al1 measurements
        Gen0: cC Generation 0 collects per 1000 operations
        A11ocated : Al1ocated memory per single operation(managed only，inc1usive,1KB = 1024B)
        1 us: 1 Microsecond(o.o00001 sec)*/



        [Benchmark]
        public void DictorySetNull()
        {
            Dictionary<string, List<BinaryTreeReference>> dic = new();
            int k = 0;
            for (int i = 0; i < 10; i++)
            {
                dic ??= new();
                for (int j = 0; j < 100; j++)
                {
                    dic.Add(j.ToString(), new List<BinaryTreeReference>(){ new BinaryTreeReference
                {
                    Left =j,
                    Right = j
                } });
                }
                if (dic.Count == 100) k++;
                dic = null;
            }
            Console.WriteLine(nameof(DictorySetNull) + $"ok:{k}");
        }
        [Benchmark]
        public void DictoryClear()
        {
            Dictionary<string, List<BinaryTreeReference>> dic = new();
            int k = 0;
            for (int i = 0; i < 10; i++)
            {
                dic ??= new();
                for (int j = 0; j < 100; j++)
                {
                    dic.Add(j.ToString(), new List<BinaryTreeReference>(){ new BinaryTreeReference
                {
                    Left =j,
                    Right = j
                } });
                }
                if (dic.Count == 100) k++;
                dic.Clear();
            }
            Console.WriteLine(nameof(DictoryClear) + $"ok:{k}");
        }
    }
}
