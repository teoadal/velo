using System;
using System.Collections.Generic;
using System.Globalization;
using Velo.Collections;
using Velo.Logging.Formatters;
using Velo.Serialization;
using Velo.Utils;

namespace Velo.Logging.Renderers
{
    internal interface IRendererCollection
    {
        ArrayRenderer GetArrayRenderer(string template, object[] args);

        TRenderer GetRenderer<TRenderer>(string template) where TRenderer : Renderer;
    }

    internal sealed class RendererCollection : IRendererCollection
    {
        private readonly IConvertersCollection _converters;
        private readonly Dictionary<string, Renderer> _renderers;
        private readonly object _lock;

        public RendererCollection(IConvertersCollection converters = null)
        {
            _converters = converters ?? new ConvertersCollection(CultureInfo.InvariantCulture);
            _renderers = new Dictionary<string, Renderer>();
            _lock = new object();
        }

        public ArrayRenderer GetArrayRenderer(string template, object[] args)
        {
            if (_renderers.TryGetValue(template, out var renderer)) return (ArrayRenderer) renderer;
            
            var arguments = BuildArguments(template);
            var argumentTypes = new LocalList<Type>(args.Length);
            
            foreach (var arg in args)
            {
                argumentTypes.Add(arg.GetType());
            }

            var messageRenderer = new DefaultStringFormatter(template);
            renderer = new ArrayRenderer(arguments, messageRenderer, argumentTypes, _converters);

            using (Lock.Enter(_lock))
            {
                _renderers[template] = renderer;
            }

            return (ArrayRenderer) renderer;
        }

        public TRenderer GetRenderer<TRenderer>(string template) where TRenderer : Renderer
        {
            if (!_renderers.TryGetValue(template, out var renderer))
            {
                var arguments = BuildArguments(template);
                var messageRenderer = new DefaultStringFormatter(template);
                var constructorParams = new object[] {arguments, messageRenderer, _converters};
                
                renderer = (TRenderer) Activator.CreateInstance(typeof(TRenderer), constructorParams);

                using (Lock.Enter(_lock))
                {
                    _renderers[template] = renderer;
                }
            }

            try
            {
                return (TRenderer) renderer;
            }
            catch (InvalidCastException e)
            {
                throw Error.Cast($"Template '{template}' is used elsewhere with other types of arguments or with other arguments", e);
            }
        }

        private static string[] BuildArguments(string template)
        {
            var argumentStarted = false;
            var arguments = new LocalList<string>();

            var builder = new LocalList<char>();

            foreach (var ch in template)
            {
                switch (ch)
                {
                    case '{':
                        argumentStarted = true;
                        break;
                    case '}':
                        arguments.Add(new string(builder.ToArray()));
                        builder.Clear();
                        argumentStarted = false;
                        break;
                    default:
                        if (argumentStarted)
                        {
                            builder.Add(ch);
                        }

                        break;
                }
            }

            return arguments.ToArray();
        }
    }
}