using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Velo
{
    public abstract class TestClass : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private Stopwatch _stopwatch;

        protected TestClass(ITestOutputHelper output)
        {
            _output = output;
            _stopwatch = Stopwatch.StartNew();
        }
        
        protected static int FixCount(int count)
        {
            count = Math.Abs(count);
            if (count > 10000) count = 10000;

            return count;
        }
        
        protected StopwatchScope Measure()
        {
            _stopwatch = Stopwatch.StartNew();
            return new StopwatchScope(_stopwatch);
        }
        
        protected async Task Measure(ValueTask task)
        {
            _stopwatch = Stopwatch.StartNew();
            await task;
            _stopwatch.Stop();
        }
        
        protected async Task Measure(Func<Task> action)
        {
            _stopwatch = Stopwatch.StartNew();
            await action();
            _stopwatch.Stop();
        }
        
        protected TResult Measure<TResult>(Func<TResult> action)
        {
            _stopwatch = Stopwatch.StartNew();
            var result = action();
            _stopwatch.Stop();

            return result;
        }
        
        protected void Measure(Action action)
        {
            _stopwatch = Stopwatch.StartNew();
            action();
            _stopwatch.Stop();
        }

        protected static Task RunTasks(int count, Func<Task> action)
        {
            var tasks = new Task[count];
            for (var i = 0; i < count; i++)
            {
                tasks[i] = action();
            }

            return Task.WhenAll(tasks);
        }
        
        protected static Task RunTasks(int count, Action action)
        {
            var tasks = new Task[count];
            for (var i = 0; i < count; i++)
            {
                tasks[i] = Task.Run(action);
            }

            return Task.WhenAll(tasks);
        }
        
        protected static Task RunTasks<T>(T[] array, Func<T, Task> action)
        {
            var tasks = new Task[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                tasks[i] = action(array[i]);
            }

            return Task.WhenAll(tasks);
        }

        protected static Task RunTasks<T>(T[] array, Func<T, ValueTask> action)
        {
            var tasks = new ValueTask[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                tasks[i] = action(array[i]);
            }

            return Task.WhenAll(tasks.Select(t => t.AsTask()));
        }
        
        protected static Task<TResult[]> RunTasks<T, TResult>(T[] array, Func<T, ValueTask<TResult>> action)
        {
            var tasks = new ValueTask<TResult>[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                tasks[i] = action(array[i]);
            }

            return Task.WhenAll(tasks.Select(t => t.AsTask()));
        }
        
        protected void WriteLine(string text, [CallerMemberName] string from = null)
        {
            _output.WriteLine(string.IsNullOrWhiteSpace(from)
                ? text
                : $"{from}: {text}");
        }

        public virtual void Dispose()
        {
            _stopwatch.Stop();
            _output.WriteLine(_stopwatch.ElapsedMilliseconds > 0
                ? $"Elapsed {_stopwatch.ElapsedMilliseconds} ms"
                : "Elapsed < 1 ms");
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