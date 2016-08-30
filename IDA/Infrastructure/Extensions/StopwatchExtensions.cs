using System;
using System.Diagnostics;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class StopwatchExtensions
    {
        public static string ToFormattedString(this Stopwatch stopwatch)
        {
            TimeSpan timeSpan = stopwatch.Elapsed;
            string result = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            return result;

        }
    }
}
