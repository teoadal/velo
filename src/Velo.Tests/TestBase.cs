using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace Velo
{
    public abstract class TestBase : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private Stopwatch _stopwatch;

        protected TestBase(ITestOutputHelper output)
        {
            _output = output;
            _stopwatch = Stopwatch.StartNew();
        }

        protected StopwatchScope StartStopwatch()
        {
            _stopwatch = Stopwatch.StartNew();
            return new StopwatchScope(_stopwatch);
        }

        protected void WriteLine(string text, [CallerMemberName] string from = "")
        {
            _output.WriteLine(string.IsNullOrWhiteSpace(from)
                ? text
                : $"{from}: {text}");
        }

        public virtual void Dispose()
        {
            _stopwatch.Stop();
            if (_stopwatch.ElapsedMilliseconds > 0)
            {
                _output.WriteLine($"Elapsed {_stopwatch.ElapsedMilliseconds} ms");
            }
            else
            {
                var nanoseconds = 1000000000.0 * _stopwatch.ElapsedTicks / Stopwatch.Frequency;
                _output.WriteLine($"Elapsed < 1 ms ({nanoseconds} ns)");
            }
        }

        protected readonly struct StopwatchScope : IDisposable
        {
            private readonly Stopwatch _stopwatch;

            public StopwatchScope(Stopwatch stopwatch)
            {
                _stopwatch = stopwatch;
            }

            public void Dispose()
            {
                _stopwatch.Stop();
            }
        }
    }
}