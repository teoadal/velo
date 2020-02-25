using System;
using System.Collections;
using System.Globalization;
using Velo.Collections;
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
        private readonly Hashtable _renderers;
        private readonly object _lock;

        public RendererCollection(IConvertersCollection converters = null)
        {
            _converters = converters ?? new ConvertersCollection(CultureInfo.InvariantCulture);
            _renderers = new Hashtable();
            _lock = new object();
        }

        public ArrayRenderer GetArrayRenderer(string template, object[] args)
        {
            var exists = _renderers[template];
            if (exists != null) return (ArrayRenderer) exists;

            var arguments = BuildArguments(template);

            var argumentTypes = new LocalList<Type>(args.Length);
            foreach (var arg in args)
            {
                argumentTypes.Add(arg.GetType());
            }

            var arrayRenderer = new ArrayRenderer(arguments, argumentTypes, _converters);

            using (Lock.Enter(_lock))
            {
                _renderers[template] = arrayRenderer;
            }

            return arrayRenderer;
        }

        public TRenderer GetRenderer<TRenderer>(string template) where TRenderer : Renderer
        {
            var exists = _renderers[template];
            if (exists != null)
            {
                try
                {
                    return (TRenderer) exists;
                }
                catch (InvalidCastException e)
                {
                    throw Error.Cast($"Template '{template}' is used elsewhere with other types of arguments or with other arguments", e);
                }
            }

            var arguments = BuildArguments(template);
            var constructorParams = new object[] {arguments, _converters};
            var renderer = (TRenderer) Activator.CreateInstance(typeof(TRenderer), constructorParams);

            using (Lock.Enter(_lock))
            {
                _renderers[template] = renderer;
            }

            return renderer;
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