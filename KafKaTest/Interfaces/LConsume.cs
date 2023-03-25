using Confluent.Kafka;
using KafKaTest.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KafKaTest.Interfaces
{
    public abstract class LConsume<THandleMessage> where THandleMessage : class
    {

        //若设置静态   多线程中多消费组创建多消费者  线程安全问题

        /// <summary>
        /// 自动提交配置
        /// </summary>
        public ConsumerConfig autoConfig = new ConsumerConfig()
        {
            BootstrapServers = consts.BootStrapServers,
            EnableAutoCommit = false,
            AutoCommitIntervalMs = 20000,//自动提交间隔毫秒数
            StatisticsIntervalMs = 60000,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };
        /// <summary>
        /// 手动提交配置
        /// </summary>
        public ConsumerConfig handleConfig = new ConsumerConfig()
        {
            BootstrapServers = consts.BootStrapServers,
            EnableAutoCommit = false,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        /// <summary>
        /// 自动提交消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="groupId"></param>
        /// <param name="dealMessage"></param>
        /// <param name="saveLog"></param>
        public abstract void ComsumeMessage(string topic, string groupId, Action<THandleMessage> dealMessage, Action<Exception> saveLog = null);
        /// <summary>
        /// 手动提交消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="groupId"></param>
        /// <param name="dealMessage"></param>
        public abstract void ConsumeByCommit(string topic, string groupId, Action<THandleMessage> dealMessage);
        /// <summary>
        /// 开启多个线程消费手动提交
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="groupId"></param>
        /// <param name="dealMessage"></param>
        /// <param name="taskCount"></param>
        public abstract void ConsumeNTaskByCommit(string topic,string groupId, Action<THandleMessage> dealMessage,int taskCount);

        public abstract void ConsumePartitionMessage(TopicPartition topicPartition, string groupId, Action<THandleMessage> dealMessage);
    }
}
