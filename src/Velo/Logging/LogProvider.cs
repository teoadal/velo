using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Velo.Collections;
using Velo.Logging.Enrichers;
using Velo.Logging.Renderers;
using Velo.Serialization;
using Velo.Utils;

namespace Velo.Logging
{
    internal sealed class LogProvider
    {
        [ThreadStatic] 
        private static StringBuilder _buffer;

        private readonly IConvertersCollection _converters;
        private readonly ILogEnricher[] _enrichers;
        private readonly Dictionary<string, LogRenderer> _renderers;
        private readonly ILogWriter[] _writers;

        public LogProvider(ILogWriter[] writers, ILogEnricher[] enrichers, IConvertersCollection converters = null)
        {
            _converters = converters ?? new ConvertersCollection(CultureInfo.InvariantCulture);
            _enrichers = enrichers;
            _renderers = new Dictionary<string, LogRenderer>();
            _writers = writers;
        }

        public void Write(LogLevel level, Type sender, string template)
        {
            var stringWriter = new StringWriter(GetBuffer());

            Enrich(stringWriter, level, sender);

            stringWriter.Write(template);

            Sink(level, StringUtils.Release(stringWriter));
        }

        public void Write<T1>(LogLevel level, Type sender, string template, T1 arg1)
        {
            var stringWriter = new StringWriter(GetBuffer());

            Enrich(stringWriter, level, sender);

            var renderer = GetRenderer<LogRenderer<T1>>(template);
            renderer.Render(stringWriter, arg1);

            Sink(level, StringUtils.Release(stringWriter));
        }

        public void Write<T1, T2>(LogLevel level, Type sender, string template, T1 arg1, T2 arg2)
        {
            var stringWriter = new StringWriter(GetBuffer());

            Enrich(stringWriter, level, sender);

            var renderer = GetRenderer<LogRenderer<T1, T2>>(template);
            renderer.Render(stringWriter, arg1, arg2);

            Sink(level, StringUtils.Release(stringWriter));
        }

        public void Write<T1, T2, T3>(LogLevel level, Type sender, string template, T1 arg1, T2 arg2, T3 arg3)
        {
            var stringWriter = new StringWriter(GetBuffer());

            Enrich(stringWriter, level, sender);

            var renderer = GetRenderer<LogRenderer<T1, T2, T3>>(template);
            renderer.Render(stringWriter, arg1, arg2, arg3);

            Sink(level, StringUtils.Release(stringWriter));
        }

        public void Write(LogLevel level, Type sender, string template, params object[] args)
        {
            LogRenderer renderer;

            using (Lock.Enter(_renderers))
            {
                if (!_renderers.TryGetValue(template, out renderer))
                {
                    var argTypes = new LocalList<Type>(args.Length);
                    foreach (var arg in args)
                    {
                        argTypes.Add(arg.GetType());
                    }

                    renderer = new LogRendererArray(template, GetBuffer(), _converters, argTypes);

                    _renderers.Add(template, renderer);
                }
            }

            var stringWriter = new StringWriter(GetBuffer());
            Enrich(stringWriter, level, sender);
            ((LogRendererArray) renderer).Render(stringWriter, args);

            Sink(level, template);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static StringBuilder GetBuffer()
        {
            return _buffer ??= new StringBuilder(500);
        }

        private TRenderer GetRenderer<TRenderer>(string template)
            where TRenderer : LogRenderer
        {
            using (Lock.Enter(_renderers))
            {
                if (_renderers.TryGetValue(template, out var exists))
                {
                    return (TRenderer) exists;
                }

                var rendererType = typeof(TRenderer);
                var rendererConstructorParams = new object[] {template, GetBuffer(), _converters};
                var instance = Activator.CreateInstance(rendererType, rendererConstructorParams);

                var renderer = (LogRenderer) instance;

                _renderers.Add(template, renderer);

                return (TRenderer) renderer;
            }
        }

        private void Enrich(TextWriter writer, LogLevel level, Type sender)
        {
            foreach (var enricher in _enrichers)
            {
                enricher.Enrich(writer, level, sender);
                writer.Write(' ');
            }
        }

        private void Sink(LogLevel level, string message)
        {
            foreach (var sink in _writers)
            {
                if (sink.Level > level) continue;
                sink.Write(level, message);
            }
        }
    }
}