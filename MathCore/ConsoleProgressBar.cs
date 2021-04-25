using System;
using System.Text;
using System.Threading;

namespace MathCore
{
    /// <summary>
    /// Индикатор прогресса для консоли
    ///     <code>
    ///     Console.Write("Performing some task... ");
    ///     const int max = 2000;
    ///     using (var progress = new ProgressBar(50) { PercentSymbol = null, PercentFormat = "p1" })
    ///         for (var i = 0; i &lt;= max; i++)
    ///         {
    ///             progress.Report((double) i / max);
    ///             Thread.Sleep(20);
    ///             Console.Title = i.ToString();
    ///         }
    ///     Console.WriteLine("Done.");
    ///     </code>
    /// </summary>
    public class ConsoleProgressBar : IDisposable, IProgress<double>
    {
        public const string GradientSharp = ".:+=#";
        public const string GradientBlocks = "+│░▒▓";

        private readonly int _BlockCount;
        private const string __Animation = @"|/-\";

        private readonly Timer _Timer;

        private double _CurrentProgress;
        private string _CurrentText = string.Empty;
        private bool _Disposed;
        private int _AnimationIndex;

        private readonly int _CursorTop;
        private readonly int _CursorLeft;

        public char ProgressChar { get; init; } = '█'; //'#';

        public string PercentFormat { get; init; } = "f0";

        public char? PercentSymbol { get; init; } = '%';

        public int PercentStrLength { get; init; } = 3;

        public string GradientBlockSet { get; init; } = GradientBlocks;

        public TimeSpan AnimationTimeout { get; init; } = TimeSpan.FromMilliseconds(100);

        public ConsoleProgressBar(int BlockCount = 20)
        {
            _CursorLeft = Console.CursorLeft;
            _CursorTop = Console.CursorTop;

            _BlockCount = BlockCount;
            _Timer = new Timer(TimerHandler);

            // A progress bar is only for temporary display in a console window.
            // If the console output is redirected to a file, draw nothing.
            // Otherwise, we'll end up with a lot of garbage in the target file.
            if (!Console.IsOutputRedirected) ResetTimer();
        }

        public void Report(double value) => Interlocked.Exchange(ref _CurrentProgress, Math.Max(0, Math.Min(1, value)));

        private void TimerHandler(object state)
        {
            lock (_Timer)
            {
                if (_Disposed) return;

                var gradient_set = GradientBlockSet;

                var progress_block_count = (int)(_CurrentProgress * _BlockCount);
                //var percent = (int)(_CurrentProgress * 100);

                var filled_part = new string(ProgressChar, progress_block_count);
                var empty_blocks_count = _BlockCount - progress_block_count - (gradient_set is { Length: > 0 } ? 2 : 1);
                var empty_part = new string('-', Math.Max(0, empty_blocks_count));
                var animation = __Animation[_AnimationIndex++ % __Animation.Length];

                var gradient_index = (int)(_CurrentProgress * _BlockCount * gradient_set.Length) % gradient_set.Length;
                var gradient = gradient_set is { Length: > 0 }
                    ? gradient_set[gradient_index]
                    : (char?)null;

                var percent_str = PercentFormat is { Length: > 0 } percent_format
                    ? (percent_format[0] is 'p' or 'P' ? _CurrentProgress : _CurrentProgress * 100).ToString(percent_format).PadLeft(Math.Max(0, PercentStrLength))
                    : $"{(int)(_CurrentProgress * 100),3}";

                var dec_part = empty_blocks_count >= 0 ? (int)(_CurrentProgress * _BlockCount * 10) % 10 : (int?)null;
                var text = $"[{filled_part}{gradient}{dec_part}{empty_part}] {percent_str}{PercentSymbol} {animation}";
                UpdateText(text);

                ResetTimer();
            }
        }

        private void UpdateText(string text)
        {
            // Get length of common portion
            var prefix_length = 0;
            var length = Math.Min(_CurrentText.Length, text.Length);
            while (prefix_length < length && text[prefix_length] == _CurrentText[prefix_length])
                prefix_length++;

            // Backtrack to the first differing character
            var output_builder = new StringBuilder();
            output_builder.Append('\b', _CurrentText.Length - prefix_length);

            // Output new suffix
            output_builder.Append(text[prefix_length..]);

            // If the new text is shorter than the old one: delete overlapping characters
            var overlap_count = _CurrentText.Length - text.Length;
            if (overlap_count > 0)
            {
                output_builder.Append(' ', overlap_count);
                output_builder.Append('\b', overlap_count);
            }

            var current_cursor_left = Console.CursorLeft;
            var current_cursor_top = Console.CursorTop;

            lock (Console.Out)
            {
                Console.CursorTop = _CursorTop;
                Console.CursorLeft = _CursorLeft + _CurrentText.Length;

                Console.Write(output_builder);

                if (_CursorTop != current_cursor_top)
                    Console.CursorTop = current_cursor_top;
                if (_CursorLeft + _CurrentText.Length != current_cursor_left)
                    Console.CursorLeft = current_cursor_left;
            }

            _CurrentText = text;
        }

        private void ResetTimer() => _Timer.Change(AnimationTimeout, TimeSpan.FromMilliseconds(-1));

        public void Dispose()
        {
            lock (_Timer)
            {
                _Disposed = true;
                UpdateText(string.Empty);
            }
        }

    }
}
