using System;
using Velo.Serialization;

namespace Velo.TestsModels.Infrastructure
{
    public interface ISession
    {
        Guid Id { get; }

        JConverter Converter { get; }
    }
}