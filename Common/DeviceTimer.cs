using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Common
{
    /// <summary>
    /// .Net Stopwatch对高精度定时器作了很好的包装
    /// DeviceTimer内部采用Stopwatch类实现高精度定时操作
    /// </summary>
    public sealed class DeviceTimer
    {
#if USE_CPU_COUNTING
        //引入高性能计数器API，通过对CPU计数完成计时
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
 
        //获取当前CPU的工作频率
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
#else
        /// <summary>
        /// 获取TickCount64计数
        /// </summary>
        /// <returns></returns>
        //[DllImport("kernel32.dll")]
        //public static extern long GetTickCount64();
#endif
        private enum DeviceTimerState
        {
            TM_ST_IDLE = 0,
            TM_ST_BUSY = 1,
            TM_ST_TIMEOUT = 2,
        }

        /// <summary>
        /// Stopwatch object
        /// </summary>
        Stopwatch _stopWatch = new Stopwatch();

        /// <summary>
        /// 定时器内部状态
        /// </summary>
        DeviceTimerState _state;

        /// <summary>
        /// 定时器开始计时时刻的相对时间点
        /// </summary>
        long _startTime;

        /// <summary>
        /// 定时器超时时刻的相对时间点
        /// </summary>
        long _timeOut;

#if USE_CPU_COUNTING
 
        /// <summary>
        /// CPU运行的时钟频率
        /// </summary>
        double _freq;
#endif

        /// <summary>
        /// 定时时间（单位：ms）
        /// </summary>
        double _duration;

        /// <summary>
        /// class constructure
        /// </summary>
        public DeviceTimer()
        {
#if USE_CPU_COUNTING
            long freq;
            if (QueryPerformanceFrequency(out freq) == false)
                throw new Exception("本计算机不支持高性能计数器");
            //得到每1ms的CPU计时TickCount数目
            _freq = (double)freq / 1000.0;
            QueryPerformanceCounter(out _startTime);
#else
            _stopWatch.Start();
            _startTime = 0;
#endif
            SetState(DeviceTimerState.TM_ST_IDLE);
            _timeOut = _startTime;
            _duration = 0;
        }

        /// <summary>
        /// 内部调用：设置定时器当前状态
        /// </summary>
        /// <param name="state"></param>
        private void SetState(DeviceTimerState state)
        {
            _state = state;
        }

        /// <summary>
        /// 内部调用：返回定时器当前状态
        /// </summary>
        /// <returns></returns>
        private DeviceTimerState GetState()
        {
            return _state;
        }

        /// <summary>
        /// 定时器开始计时到现在已流逝的时间（单位：毫秒）
        /// </summary>
        /// <returns></returns>
        public double GetElapseTime()
        {
            long curCount;
#if USE_CPU_COUNTING
            QueryPerformanceCounter(out curCount);
            return (double)(curCount - _startTime) / (double)_freq;
#else
            curCount = _stopWatch.ElapsedMilliseconds;
            return curCount - _startTime;
#endif
        }

        /// <summary>
        /// 获取定时总时间
        /// </summary>
        /// <returns></returns>
        public double GetTotalTime()
        {
            return _duration;
        }

        /// <summary>
        /// 停止计时器计时
        /// </summary>
        public void Stop()
        {
            SetState(DeviceTimerState.TM_ST_IDLE);
#if USE_CPU_COUNTING
            QueryPerformanceCounter(out _startTime);
#else
            _startTime = _stopWatch.ElapsedMilliseconds;
#endif
            _timeOut = _startTime;
            _duration = 0;
        }

        /// <summary>
        /// 启动定时器
        /// </summary>
        /// <param name="delay_ms">定时时间（单位：毫秒）</param>
        public void Start(double delay_ms)
        {
#if USE_CPU_COUNTING
            QueryPerformanceCounter(out _startTime);
            _timeOut = Convert.ToInt64(_startTime + delay_ms * _freq);
#else
            _startTime = _stopWatch.ElapsedMilliseconds;
            _timeOut = Convert.ToInt64(_startTime + delay_ms);
#endif
            SetState(DeviceTimerState.TM_ST_BUSY);
            _duration = delay_ms;
        }

        /// <summary>
        /// 重新开始定时器
        /// 开始的计时时间以上一次Start的时间为准
        /// </summary>
        /// <param name="delay_ms">定时时间（单位：毫秒）</param>
        public void Restart(double delay_ms)
        {
#if USE_CPU_COUNTING
            _timeOut = Convert.ToInt64(_startTime + delay_ms * _freq);
#else
            _timeOut = Convert.ToInt64(_startTime + delay_ms);
#endif
            SetState(DeviceTimerState.TM_ST_BUSY);
            _duration = delay_ms;
        }

        /// <summary>
        /// 返回定时器是否超时
        /// </summary>
        /// <returns></returns>
        public bool IsTimeout()
        {
            if (_state == DeviceTimerState.TM_ST_IDLE)
            {
                //System.Diagnostics.Debug.WriteLine("Warning: Misuage of the device timer. You must start it first before you can use it.");
                //System.Diagnostics.Debug.Assert(false, "Warning: Misuage of the device timer. You must start it first before you can use it.");
            }
            long curCount;
#if USE_CPU_COUNTING
            QueryPerformanceCounter(out curCount);
#else
            curCount = _stopWatch.ElapsedMilliseconds;
#endif
            if (_state == DeviceTimerState.TM_ST_BUSY && (curCount >= _timeOut))
            {
                SetState(DeviceTimerState.TM_ST_TIMEOUT);
                return true;
            }
            else if (_state == DeviceTimerState.TM_ST_TIMEOUT)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 定时器是否在工作中
        /// </summary>
        /// <returns></returns>
        public bool IsIdle()
        {
            return (_state == DeviceTimerState.TM_ST_IDLE);
        }
    }
}
