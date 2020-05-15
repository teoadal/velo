using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.ECS.Actors;
using Velo.ECS.Assets;
using Velo.ECS.Sources;
using Velo.TestsModels.ECS;
using Velo.Text;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Sources
{
    public class SourceDescriptionsShould : ECSTestClass
    {
        private readonly SourceDescriptions _sourceDescriptions;

        public SourceDescriptionsShould(ITestOutputHelper output) : base(output)
        {
            _sourceDescriptions = new SourceDescriptions();
        }

        [Theory, AutoData]
        public void AddAliasIfNotExists(string alias)
        {
            _sourceDescriptions
                .GetOrAddAlias(alias)
                .Should().BeGreaterThan(0);
        }

        [Theory, AutoData]
        public void GetAliasIfExists(string alias)
        {
            var id = _sourceDescriptions.GetOrAddAlias(alias);
            _sourceDescriptions.GetOrAddAlias(alias).Should().Be(id);
        }

        [Fact]
        public void GetComponentName()
        {
            var componentType = typeof(TestComponent1);

            _sourceDescriptions
                .GetComponentName(componentType)
                .Should().Be(SourceDescriptions.BuildTypeName(componentType));
        }

        [Fact]
        public void GetEntityTypeByName()
        {
            var entityType = typeof(TestAsset);

            _sourceDescriptions
                .GetEntityType(SourceDescriptions.BuildTypeName(entityType))
                .Should().Be(entityType);
        }

        [Fact]
        public void ReplaceComponentWord()
        {
            SourceDescriptions
                .BuildTypeName(typeof(TestComponent1))
                .Should().Be(nameof(TestComponent1).Cut("Component"));
        }

        [Theory]
        [InlineData(typeof(Asset))]
        [InlineData(typeof(TestAsset))]
        [InlineData(typeof(Actor))]
        [InlineData(typeof(TestActor))]
        public void NotReplaceWordsInEntityTypeName(Type type)
        {
            SourceDescriptions
                .BuildTypeName(type)
                .Should().Be(type.Name);
        }

        [Theory, AutoData]
        public void SuccessfulTryGetExistingAlias(string alias)
        {
            var id = _sourceDescriptions.GetOrAddAlias(alias);

            _sourceDescriptions.TryGetAlias(id, out var actual).Should().BeTrue();
            actual.Should().Be(alias);
        }

        [Fact]
        public void UnsuccessfulTryGetExistingAlias()
        {
            _sourceDescriptions.TryGetAlias(0, out _).Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ThrowIfAliasNullOrWhitespace(string alias)
        {
            _sourceDescriptions
                .Invoking(descriptions => descriptions.GetOrAddAlias(alias))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowIfComponentNameNotFound()
        {
            _sourceDescriptions
                .Invoking(descriptions => descriptions.GetComponentType(Fixture.Create<string>()))
                .Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void ThrowIfComponentTypeNotFound()
        {
            _sourceDescriptions
                .Invoking(descriptions => descriptions.GetComponentName(typeof(object)))
                .Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void ThrowIfEntityNameNotFound()
        {
            _sourceDescriptions
                .Invoking(descriptions => descriptions.GetEntityType(Fixture.Create<string>()))
                .Should().Throw<KeyNotFoundException>();
        }
    }
}