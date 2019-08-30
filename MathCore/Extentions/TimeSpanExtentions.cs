using System.Diagnostics.Contracts;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace System
{
    public static class TimeSpanExtentions
    {
        [DST, Pure]
        public static string ToShortString(this TimeSpan time)
        {
            var result = "";
            var empty = true;
            var days = time.Days;
            if(days != 0)
            {
                result = $"{days}d";
                empty = false;
            }

            var hours = time.Hours;
            if(!empty)
                result += $"{hours:00}";
            else if(hours != 0)
            {
                result = hours.ToString();
                empty = false;
            }

            var minutes = time.Minutes;
            if(!empty)
                result += $":{minutes:00}";
            else if(minutes != 0)
            {
                result = minutes.ToString();
                empty = false;
            }

            var seconds = time.Seconds;
            var miliseconds = time.Milliseconds;

            return empty
                    ? (seconds == 0 && miliseconds == 0
                        ? "0"
                        : $"{seconds + (double)miliseconds / 1000}")
                    : seconds >= 10 
                        ? $"{result}:{seconds + (double)miliseconds/1000}" : $"{result}:0{seconds + (double)miliseconds/1000}";
        }
    }
}
