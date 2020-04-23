#nullable enable
using System;
using System.Globalization;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Velo.Serialization;
using Velo.Serialization.Converters;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization
{
    public class ConvertersCollectionShould : DeserializationTests
    {
        private readonly IConvertersCollection _converters;

        public ConvertersCollectionShould(ITestOutputHelper output) : base(output)
        {
            _converters = new ConvertersCollection(CultureInfo.InvariantCulture);
        }

        [Theory, MemberData(nameof(Values))]
        public void GetConverter(Type type)
        {
            var converterType = typeof(IJsonConverter<>).MakeGenericType(type);
            
            _converters
                .Invoking(converter => converter.Get(type))
                .Should().NotThrow()
                .Which.Should().BeAssignableTo(converterType);
        }

        [Fact]
        public void GetConverterGeneric()
        {
            _converters.Get<Boo>()
                .Should().BeOfType<ObjectConverter<Boo>>();
        }

        [Fact]
        public void ThrowGetVoidConverter()
        {
            _converters
                .Invoking(converters => converters.Get(typeof(void)))
                .Should().Throw<ArgumentException>();
        }
        
        public static TheoryData<Type> Values
        {
            get
            {
                var fixture = new Fixture();
                
                Boo[]? nullableArray = fixture.Create<Boo[]?>();
                Boo? nullableBoo = fixture.Create<Boo?>();
                string? nullableString = fixture.Create<string?>();
                
                return new TheoryData<Type>
                {
                    typeof(Boo[]), typeof(Boo?[]), nullableArray!.GetType(),
                    typeof(bool), typeof(bool?),
                    typeof(DateTime), typeof(DateTime?),
                    typeof(double), typeof(double?),
                    typeof(ServiceLifetime), typeof(ServiceLifetime?),
                    typeof(float), typeof(float?),
                    typeof(int), typeof(int?),
                    typeof(Boo), nullableBoo!.GetType(),
                    typeof(string), nullableString!.GetType(),
                    typeof(TimeSpan), typeof(TimeSpan?)
                };
            }
        }
    }
}