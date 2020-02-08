using System.IO;
using System.Text;
using Velo.Collections;
using Velo.Serialization;
using Velo.Serialization.Converters;

namespace Velo.Logging.Renderers
{
    internal abstract class LogRenderer
    {
        protected readonly TemplatePart[] Parts;

        protected LogRenderer(string template, StringBuilder stringBuilder)
        {
            var parts = new LocalList<TemplatePart>();
            foreach (var ch in template)
            {
                switch (ch)
                {
                    case '{':
                        parts.Add(BuildPart(stringBuilder, false));
                        stringBuilder.Clear();
                        break;
                    case '}':
                        parts.Add(BuildPart(stringBuilder, true));
                        break;
                    default:
                        stringBuilder.Append(ch);
                        break;
                }
            }

            parts.Add(BuildPart(stringBuilder, false));

            Parts = parts.ToArray();
        }

        private static TemplatePart BuildPart(StringBuilder stringBuilder, bool isArgument)
        {
            var value = stringBuilder.ToString();
            stringBuilder.Clear();
            return new TemplatePart(value, isArgument);
        }

        protected readonly struct TemplatePart
        {
            public readonly string Argument;
            public readonly bool IsArgument;
            public readonly string Text;

            public TemplatePart(string value, bool isArgument = false)
            {
                if (isArgument)
                {
                    Argument = value;
                    Text = null;
                }
                else
                {
                    Argument = null;
                    Text = value;
                }

                IsArgument = isArgument;
            }
        }
    }

    internal sealed class LogRenderer<T1>: LogRenderer
    {
        private readonly IJsonConverter<T1> _converter;
        
        public LogRenderer(string template, StringBuilder stringBuilder, IConvertersCollection converters) 
            : base(template, stringBuilder)
        {
            _converter = converters.Get<T1>();
        }

        public void Render(TextWriter writer, T1 arg)
        {
            foreach (var part in Parts)
            {
                if (part.IsArgument) _converter.Serialize(arg, writer);
                else writer.Write(part.Text);
            }
        }
    }
    
    internal sealed class LogRenderer<T1, T2>: LogRenderer
    {
        private readonly IJsonConverter<T1> _converter1;
        private readonly IJsonConverter<T2> _converter2;
        
        public LogRenderer(string template, StringBuilder stringBuilder, IConvertersCollection converters) 
            : base(template, stringBuilder)
        {
            _converter1 = converters.Get<T1>();
            _converter2 = converters.Get<T2>();
        }

        public void Render(TextWriter writer, T1 arg1, T2 arg2)
        {
            var argumentCounter = 0;
            foreach (var part in Parts)
            {
                if (part.IsArgument)
                {
                    switch (argumentCounter++)
                    {
                        case 0:
                            _converter1.Serialize(arg1, writer);
                            break;
                        case 1:
                            _converter2.Serialize(arg2, writer);
                            break;
                    }
                }
                else writer.Write(part.Text);
            }
        }
    }
    
    internal sealed class LogRenderer<T1, T2, T3>: LogRenderer
    {
        private readonly IJsonConverter<T1> _converter1;
        private readonly IJsonConverter<T2> _converter2;
        private readonly IJsonConverter<T3> _converter3;
        
        public LogRenderer(string template, StringBuilder stringBuilder, IConvertersCollection converters) 
            : base(template, stringBuilder)
        {
            _converter1 = converters.Get<T1>();
            _converter2 = converters.Get<T2>();
            _converter3 = converters.Get<T3>();
        }

        public void Render(TextWriter writer, T1 arg1, T2 arg2, T3 arg3)
        {
            var argumentCounter = 0;
            foreach (var part in Parts)
            {
                if (part.IsArgument)
                {
                    switch (argumentCounter++)
                    {
                        case 0:
                            _converter1.Serialize(arg1, writer);
                            break;
                        case 1:
                            _converter2.Serialize(arg2, writer);
                            break;
                        case 2:
                            _converter3.Serialize(arg3, writer);
                            break;
                    }
                }
                else writer.Write(part.Text);
            }
        }
    }
}