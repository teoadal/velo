using System;
using System.Threading.Tasks;
using FluentAssertions;
using Velo.Collections;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections
{
    public class DangerousVectorShould : TestClass
    {
        private readonly string _arg;
        private readonly DangerousVector<Type, string> _vector;

        public DangerousVectorShould(ITestOutputHelper output) : base(output)
        {
            _arg = nameof(DangerousVectorShould);
            _vector = new DangerousVector<Type, string>();
        }

        [Fact]
        public void Add()
        {
            var types = typeof(DangerousVector<,>).Assembly.DefinedTypes;
            foreach (var type in types)
            {
                var name = _vector.GetOrAdd(type, t => t.Name);
                name.Should().Be(type.Name);
            }
        }

        [Fact]
        public void AddMultiThreading()
        {
            var types = typeof(DangerousVector<,>).Assembly.DefinedTypes;
            Parallel.ForEach(types, type =>
            {
                var name = _vector.GetOrAdd(type, t => t.Name);
                name.Should().Be(type.Name);
            });
        }

        [Fact]
        public void AddWithArg()
        {
            var types = typeof(DangerousVector<,>).Assembly.DefinedTypes;
            foreach (var type in types)
            {
                var name = _vector.GetOrAdd(type, (t, arg) => t.Name + arg, _arg);
                name.Should().Be(type.Name + _arg);
            }
        }

        [Fact]
        public void AddWithArgMultiThreading()
        {
            var types = typeof(DangerousVector<,>).Assembly.DefinedTypes;
            Parallel.ForEach(types, type =>
            {
                var name = _vector.GetOrAdd(type, (t, arg) => t.Name + arg, _arg);
                name.Should().Be(type.Name + _arg);
            });
        }

        [Fact]
        public void TryAdd()
        {
            _vector
                .Invoking(vector => vector.TryAdd(typeof(object), string.Empty))
                .Should().NotThrow();
        }
    }
}