﻿using BlazeGate.Common.Autofac;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement
{
    public class SnowFlakeService : ISnowFlakeService, ISingletonDenpendency
    {
        public static Random random = new Random();

        public SnowFlakeService()
        {
            //随机生成一个数据中心ID和工作ID
            DatacenterId = GetRandomDatacenterId();
            WorkerId = GetRandomWorkerId();

            Guid = System.Guid.NewGuid().ToString();
        }

        public static long GetRandomWorkerId()
        {
            return random.Next(0, (int)MaxWorkerId + 1);
        }

        public static long GetRandomDatacenterId()
        {
            return random.Next(0, (int)MaxDatacenterId + 1);
        }

        //基准时间
        public const long Twepoch = 1288834974657L;

        //机器标识位数
        private const int WorkerIdBits = 5;

        //数据标志位数
        private const int DatacenterIdBits = 5;

        //序列号识位数
        private const int SequenceBits = 12;

        //机器ID最大值 31
        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);

        //数据标志ID最大值 31
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

        //序列号ID最大值 4095
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        //机器ID偏左移12位
        private const int WorkerIdShift = SequenceBits;

        //数据ID偏左移17位
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;

        //时间毫秒左移22位
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

        private long _sequence = 0L;
        private long _lastTimestamp = -1L;

        //机器ID
        public long WorkerId { get; protected set; }

        //机房ID
        public long DatacenterId { get; protected set; }

        public string Guid { get; protected set; }

        public long Sequence
        {
            get { return _sequence; }
            internal set { _sequence = value; }
        }

        private readonly object _lock = new Object();

        public async Task<List<long>> NextIds(int count)
        {
            if (count < 1)
            {
                count = 1;
            }
            if (count > 4095)
            {
                count = 4095;
            }

            List<long> ids = new List<long>();
            for (int i = 0; i < count; i++)
            {
                ids.Add(await NextId());
            }
            return ids;
        }

        public virtual Task<long> NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();
                if (timestamp < _lastTimestamp)
                {
                    throw new Exception(string.Format("时间戳必须大于上一次生成ID的时间戳.  拒绝为{0}毫秒生成id", _lastTimestamp - timestamp));
                }

                //如果上次生成时间和当前时间相同,在同一毫秒内
                if (_lastTimestamp == timestamp)
                {
                    //sequence自增，和sequenceMask相与一下，去掉高位
                    _sequence = (_sequence + 1) & SequenceMask;
                    //判断是否溢出,也就是每毫秒内超过1024，当为1024时，与sequenceMask相与，sequence就等于0
                    if (_sequence == 0)
                    {
                        //等待到下一毫秒
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    //如果和上次生成时间不同,重置sequence，就是下一毫秒开始，sequence计数重新从0开始累加,
                    //为了保证尾数随机性更大一些,最后一位可以设置一个随机数
                    _sequence = 0;//new Random().Next(10);
                }

                _lastTimestamp = timestamp;
                return Task.FromResult(((timestamp - Twepoch) << TimestampLeftShift) | (DatacenterId << DatacenterIdShift) | (WorkerId << WorkerIdShift) | _sequence);
            }
        }

        public async Task<SnowFlakeInfo> GetSnowFlakeInfo()
        {
            return new SnowFlakeInfo
            {
                Guid = Guid,
                WorkerId = WorkerId,
                DatacenterId = DatacenterId
            };
        }

        public async Task SetId(long datacenterId, long workerId)
        {
            // 如果超出范围就抛出异常
            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentException(string.Format("worker Id 必须大于0，且不能大于MaxWorkerId： {0}", MaxWorkerId));
            }

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException(string.Format("region Id 必须大于0，且不能大于MaxDatacenterId： {0}", MaxDatacenterId));
            }

            this.DatacenterId = datacenterId;
            this.WorkerId = workerId;
        }

        // 防止产生的时间比之前的时间还要小（由于NTP回拨等问题）,保持增量的趋势.
        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        private readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // 获取当前的时间戳
        protected virtual long TimeGen()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }
}