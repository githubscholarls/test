﻿using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionlessTest.Jobs
{
    [DisallowConcurrentExecution]
    public class Job11 : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Job:" + nameof(Job11) + $"Now Time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}"+ $"UTC Time:{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}");

            return Task.CompletedTask;
        }
    }
}
