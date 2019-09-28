using System;
using System.Diagnostics;
using Velo.Serialization;

namespace Velo.TestsModels.Infrastructure
{
    public interface ISession
    {
        Guid Id { get; }

        JConverter Converter { get; }
    }

    [DebuggerDisplay("{" + nameof(Id) + "}")]
    public class Session : ISession
    {
        public JConverter Converter { get; }

        public Guid Id { get; }

        public Session(JConverter converter)
        {
            Converter = converter;
            Id = Guid.NewGuid();
        }
    }
}