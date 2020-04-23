using System;
using System.Globalization;
using Velo.Collections;
using Velo.Logging.Formatters;
using Velo.Serialization;
using Velo.Utils;

namespace Velo.Logging.Renderers
{
    internal interface IRenderersCollection
    {
        ArrayRenderer GetArrayRenderer(string template, object[] args);

        TRenderer GetRenderer<TRenderer>(string template) where TRenderer : Renderer;
    }

    internal sealed class RenderersCollection : DangerousVector<string, Renderer>, IRenderersCollection
    {
        private readonly Func<string, object[], Renderer> _arrayRendererBuilder;
        private readonly IConvertersCollection _converters;
        private readonly Func<string, Type, Renderer> _rendererBuilder;

        public RenderersCollection(IConvertersCollection? converters = null)
        {
            _arrayRendererBuilder = BuildArrayRenderer;
            _converters = converters ?? new ConvertersCollection(CultureInfo.InvariantCulture);
            _rendererBuilder = BuildRenderer;
        }

        public ArrayRenderer GetArrayRenderer(string template, object[] args)
        {
            return (ArrayRenderer) GetOrAdd(template, _arrayRendererBuilder, args);
        }

        public TRenderer GetRenderer<TRenderer>(string template) where TRenderer : Renderer
        {
            return (TRenderer) GetOrAdd(template, _rendererBuilder, Typeof<TRenderer>.Raw);
        }

        private Renderer BuildArrayRenderer(string template, object[] args)
        {
            var arguments = BuildArguments(template);
            var argumentTypes = new LocalList<Type>(args.Length);

            foreach (var arg in args)
            {
                argumentTypes.Add(arg.GetType());
            }

            var messageRenderer = new DefaultStringFormatter(template);
            return new ArrayRenderer(arguments, messageRenderer, argumentTypes, _converters);
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

        private Renderer BuildRenderer(string template, Type rendererType)
        {
            var arguments = BuildArguments(template);
            var messageRenderer = new DefaultStringFormatter(template);
            var constructorParams = new object[] {arguments, messageRenderer, _converters};

            return (Renderer) Activator.CreateInstance(rendererType, constructorParams);
        }
    }
}