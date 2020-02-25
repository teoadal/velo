using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Serilog;
using Velo.Logging;

namespace Velo.Benchmark.Logging
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class LoggerBenchmark
    {
        [Params(1000)] 
        public int Count;

        private LogMessage[] _messages;
        private string _template;

        private NLog.Logger _nlog;
        private NullLogTarget _nlogNullTarget;
        private StringLogTarget _nlogStringTarget;
        
        private ILogger _serilogString;
        private StringSink _serilogStringSink;
        private ILogger<LoggerBenchmark> _veloString;
        private StringLogWriter _veloStringWriter;
        
        private ILogger _serilogNull;
        private NullSink _serilogNullSink;
        private ILogger<LoggerBenchmark> _veloNull;
        private NullLogWriter _veloNullWriter;

        [GlobalSetup]
        public void Init()
        {
            _messages = new LogMessage[Count];
            for (var i = 0; i < Count; i++)
            {
                _messages[i] = new LogMessage(Guid.NewGuid(), Guid.NewGuid().ToString("N"), i);
            }

            _template = "Test '{value}' template for {id} ({name})";

            // I have failed to create two NLog configurations in a reasonable amount of time
            _nlogNullTarget = new NullLogTarget();
            _nlogStringTarget = new StringLogTarget();
            _nlog = LoggerBuilder.BuildNLog(_nlogNullTarget, _nlogStringTarget);

            _serilogNullSink = new NullSink();
            _serilogNull = LoggerBuilder.BuildSerilog(_serilogNullSink);

            _veloNullWriter = new NullLogWriter();
            _veloNull = LoggerBuilder.BuildVelo(_veloNullWriter);
            
            _serilogStringSink = new StringSink();
            _serilogString = LoggerBuilder.BuildSerilog(_serilogStringSink);

            _veloStringWriter = new StringLogWriter();
            _veloString = LoggerBuilder.BuildVelo(_veloStringWriter);
        }

        #region Null

        [BenchmarkCategory("Null")]
        [Benchmark(Baseline = true)]
        public int Serilog_Null()
        {
            foreach (var message in _messages)
            {
                _serilogNull.Debug(_template, message.Value, message.Id, message.Name);
            }
        
            if (_serilogNullSink.Counter == 0) throw new Exception();
            return _serilogNullSink.Counter;
        }

        [BenchmarkCategory("Null")]
        [Benchmark]
        public int Nlog_Null()
        {
            foreach (var message in _messages)
            {
                _nlog.Debug(_template, message.Value, message.Id, message.Name);
            }

            if (_nlogNullTarget.Counter == 0) throw new Exception();
            return _nlogNullTarget.Counter;
        }

        [BenchmarkCategory("Null")]
        [Benchmark]
        public int Velo_Null()
        {
            foreach (var message in _messages)
            {
                _veloNull.Debug(_template, message.Value, message.Id, message.Name);
            }

            if (_veloNullWriter.Counter == 0) throw new Exception();
            return _veloNullWriter.Counter;
        }

        #endregion

        [BenchmarkCategory("String")]
        [Benchmark(Baseline = true)]
        public int Serilog_String()
        {
            foreach (var message in _messages)
            {
                _serilogString.Debug(_template, message.Value, message.Id, message.Name);
            }

            var release = _serilogStringSink.Release();
            if (release == 0) throw new Exception();
            return release;
        }

        [BenchmarkCategory("String")]
        [Benchmark]
        public int Nlog_String()
        {
            foreach (var message in _messages)
            {
                _nlog.Debug(_template, message.Value, message.Id, message.Name);
            }

            var release = _nlogStringTarget.Release();
            if (release == 0) throw new Exception();
            return release;
        }

        [BenchmarkCategory("String")]
        [Benchmark]
        public int Velo_String()
        {
            foreach (var message in _messages)
            {
                _veloString.Debug(_template, message.Value, message.Id, message.Name);
            }

            var release = _veloStringWriter.Release();
            if (release == 0) throw new Exception();
            return release;
        }
    }
}