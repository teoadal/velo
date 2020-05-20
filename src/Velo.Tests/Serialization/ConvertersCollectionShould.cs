#nullable enable
using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Velo.Serialization;
using Velo.Serialization.Collections;
using Velo.Serialization.Objects;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.Serialization
{
    public class ConvertersCollectionShould : DeserializationTests
    {
        private readonly IConvertersCollection _converters;

        public ConvertersCollectionShould()
        {
            _converters = BuildConvertersCollection();
        }

        [Theory]
        [MemberData(nameof(Values))]
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
        public void GetArrayLikeConverter()
        {
            _converters.Get(typeof(IEnumerable<int>)).Should().BeOfType<ArrayConverter<int>>();
            _converters.Get(typeof(IReadOnlyCollection<int>)).Should().BeOfType<ArrayConverter<int>>();
            _converters.Get(typeof(ICollection<int>)).Should().BeOfType<ArrayConverter<int>>();
        }
        
        [Fact]
        public void GetListLikeConverter()
        {
            _converters.Get(typeof(IList<int>))
                .Should().BeOfType<ListConverter<int>>();
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