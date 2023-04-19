using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTest
{

    internal struct BinaryTree
    {
        public int Left { get; set; }
        public int Right { get; set; }
    }

    internal class BinaryTreeReference
    {
        public int Left { get; set; }
        public int Right { get; set; }
    }

    [MemoryDiagnoser]
    public class AllocationStructAndClass
    {
        [Benchmark]
        public void AllocationStruct()
        {
            List<BinaryTree> binaryTrees = new List<BinaryTree>(10000);
            for (int i = 0; i < 10000; i++)
            {
                binaryTrees.Add(new BinaryTree
                {
                    Left = i,
                    Right = i
                });
            }
        }

        [Benchmark]
        public void AllocationClass()
        {
            List<BinaryTreeReference> binaryTrees = new List<BinaryTreeReference>(10000);
            for (int i = 0; i < 10000; i++)
            {
                binaryTrees.Add(new BinaryTreeReference
                {
                    Left = i,
                    Right = i
                });
            }
        }
    }
}
