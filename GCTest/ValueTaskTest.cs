using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTest
{
    [MemoryDiagnoser]
    public class ValueTaskTest
    {

        public async Task<int> TaskFunc(int i)
        {
            return i;
        }

        public async ValueTask<int> ValueFunc(int i)
        {
            return i;
        }



        [Benchmark]
        public void TaskTest()
        {
            //gen0  200
            for (int i = 0; i < 1000; i++)
            {
                TaskFunc(i);
            }
        }
        [Benchmark]
        public void TaskValueTest()
        {
            //gen0   ---
            for (int i = 0; i < 1000; i++)
            {
                ValueFunc(i);
            }
        }


        public void GCTimeTaskTest()
        {

        }

    }
}
