using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Collections.Local;
using Velo.DependencyInjection;
using Velo.Mapping;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Mapping
{
    public class MapperShould : TestClass
    {
        private readonly CompiledMapper<Foo> _mapper;

        public MapperShould(ITestOutputHelper output) : base(output)
        {
            _mapper = new CompiledMapper<Foo>();
        }

        [Fact]
        public void AddDependency()
        {
            var provider = new DependencyCollection()
                .AddMapper()
                .BuildProvider();

            provider.GetRequiredService<IMapper<Foo>>();
            provider.GetRequiredService<IMapper<Boo>>();
        }

        [Theory]
        [AutoData]
        public void ConvertAnonymousToFoo(bool boolValue, float floatValue, int intValue)
        {
            var source = new
            {
                Bool = boolValue,
                Float = floatValue,
                Int = intValue
            };

            var foo = _mapper.Map(source);

            foo.Bool.Should().Be(source.Bool);
            foo.Float.Should().Be(source.Float);
            foo.Int.Should().Be(source.Int);
        }

        [Theory]
        [AutoData]
        public void ConvertBooToFoo(Boo source)
        {
            var foo = _mapper.Map(source);

            foo.Bool.Should().Be(source.Bool);
            foo.Float.Should().Be(source.Float);
            foo.Int.Should().Be(source.Int);
        }

        [Theory]
        [AutoData]
        public void ConvertFooToFoo(Foo source)
        {
            var foo = _mapper.Map(source);
            foo.Should().BeEquivalentTo(source);
        }

        [Theory]
        [AutoData]
        public void ConvertObjectToFoo(object source)
        {
            var foo = _mapper.Map(source);

            foo.Bool.Should().Be(default);
            foo.Float.Should().Be(default);
            foo.Int.Should().Be(default);
        }

        [Theory]
        [AutoData]
        public void ConvertManyDifferentTypes(Boo boo, Foo foo, bool boolValue, float floatValue, int intValue)
        {
            for (var i = 0; i < 5; i++)
            {
                ConvertBooToFoo(boo);
            }

            for (var i = 0; i < 5; i++)
            {
                ConvertFooToFoo(foo);
            }
            
            for (var i = 0; i < 5; i++)
            {
                ConvertAnonymousToFoo(boolValue, floatValue, intValue);
            }
        }
        
        [Fact]
        public void NotThrowIfBadSource()
        {
            var list = new LocalList<object>(new object(), typeof(MapperShould));

            foreach (var source in list)
            {
                var foo = _mapper.Map(source);

                foo.Bool.Should().Be(default);
                foo.Float.Should().Be(default);
                foo.Int.Should().Be(default);
            }
        }

        [Fact]
        public void ThrowIfDefaultConstructorNotExists()
        {
            Assert.Throws<KeyNotFoundException>(
                () => new CompiledMapper<ClassWithoutDefaultConstructor>());
        }

        [Fact]
        public void ThrowIfSourceNull()
        {
            Assert.Throws<NullReferenceException>(() => _mapper.Map(null));
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class ClassWithoutDefaultConstructor
        {
            // ReSharper disable UnusedParameter.Local
            public ClassWithoutDefaultConstructor(int a, float b)
            {
            }
            // ReSharper restore UnusedParameter.Local
        }
    }
}