using System.Diagnostics;

namespace EFAttribute.Helper
{
    // 用于测试性能的计时类
    internal sealed class OperationTimer : IDisposable
    {
        private readonly string mText;
        private readonly int mCollectionCount;
        private readonly Stopwatch mStopwatch;

        public OperationTimer(string text)
        {
            PrepareForOperation();
            mText = text;
            // 返回自启动进程以来已经对指定代进行的GC次数，参数是对象的代
            mCollectionCount = GC.CollectionCount(0);

            // Stopwatch类：提供一组方法和属性，可用于准确地测量运行时间
            // StartNew()：初始化新的Diagnostics.Stopwatch实例，将运行时间置零，然后开始测量运行时间
            mStopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            // Elapsed用于获取当前实例测量得出的总运行时间
            Console.WriteLine("Time={0}s;  GC times={1};  Tag:{2}", mStopwatch.Elapsed,
                GC.CollectionCount(0) - mCollectionCount, mText);
        }

        private void PrepareForOperation()
        {
            GC.Collect(); // 强制对所有代进行即时GC。
            GC.WaitForPendingFinalizers(); // 挂起当前线程，直到处理终结器队列的线程清空该队列为止。
            GC.Collect();
        }
    }
}
