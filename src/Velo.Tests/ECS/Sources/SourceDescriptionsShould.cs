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
        public SourceDescriptionsShould(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, AutoData]
        public void AddAliasIfNotExists(string alias)
        {
            SourceDescriptions
                .GetOrAddAlias(alias)
                .Should().BeGreaterThan(0);
        }

        [Theory, AutoData]
        public void GetAliasIfExists(string alias)
        {
            var id = SourceDescriptions.GetOrAddAlias(alias);
            SourceDescriptions.GetOrAddAlias(alias).Should().Be(id);
        }

        [Fact]
        public void GetComponentName()
        {
            var componentType = typeof(TestComponent1);

            SourceDescriptions
                .GetComponentName(componentType)
                .Should().Be(SourceDescriptions.BuildTypeName(componentType));
        }

        [Fact]
        public void GetEntityTypeByName()
        {
            var entityType = typeof(TestAsset);

            SourceDescriptions
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
            var id = SourceDescriptions.GetOrAddAlias(alias);

            SourceDescriptions.TryGetAlias(id, out var actual).Should().BeTrue();
            actual.Should().Be(alias);
        }

        [Fact]
        public void UnsuccessfulTryGetExistingAlias()
        {
            SourceDescriptions.TryGetAlias(0, out _).Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ThrowIfAliasNullOrWhitespace(string alias)
        {
            Assert.Throws<InvalidOperationException>(() =>
                SourceDescriptions.GetOrAddAlias(alias));
        }

        [Fact]
        public void ThrowIfComponentNameNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                SourceDescriptions.GetComponentType(Fixture.Create<string>()));
        }

        [Fact]
        public void ThrowIfComponentTypeNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                SourceDescriptions.GetComponentName(typeof(object)));
        }

        [Fact]
        public void ThrowIfEntityNameNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                SourceDescriptions.GetEntityType(Fixture.Create<string>()));
        }
    }
}