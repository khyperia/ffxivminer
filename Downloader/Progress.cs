using System;
using System.Diagnostics;

namespace Downloader
{
    internal class Progress
    {
        private readonly Stopwatch _stopwatch;

        public Progress()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public TimeSpan Elapsed() => _stopwatch.Elapsed;

        public TimeSpan Time(double value)
        {
            var time = Elapsed();
            return value == 0 ? default : (time / value) - time;
        }

        public string TimeString(double value)
        {
            var left = Time(value);
            var elapsed = Elapsed();
            var total = left + elapsed;
            return $"{100*value:F2}%, {left} left, {elapsed} elapsed, {total} total";
        }
    }
}
