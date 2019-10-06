using System;
using System.Diagnostics;
using Velo.Serialization;

namespace Velo.TestsModels.Infrastructure
{
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