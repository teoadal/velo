using System;
using System.Collections.Generic;
using System.Globalization;
using AutoFixture;
using FluentAssertions;
using Moq;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.Settings.Provider;
using Velo.Settings.Sources;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Settings;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Settings.Provider
{
    public class SettingsProviderShould : TestClass
    {
        private readonly ConvertersCollection _converters;
        private readonly string _property;
        private readonly JsonValue _propertyValue;
        private readonly string _nestedProperty;
        private readonly JsonObject _nestedValue;
        private readonly JsonObject _out;
        private readonly ISettingsProvider _settings;
        private readonly Mock<ISettingsSource> _source;

        public SettingsProviderShould(ITestOutputHelper output) : base(output)
        {
            _converters = new ConvertersCollection(CultureInfo.InvariantCulture);
            
            _property = "property";
            _propertyValue = JsonValue.True;
            _nestedProperty = "nested";
            _nestedValue = (JsonObject) _converters.Get<BigObject>().Write(Fixture.Create<BigObject>());
            _out = new JsonObject
            {
                [_property] = _propertyValue,
                [_nestedProperty] = _nestedValue
            };

            _source = BuildSource();
            _settings = new SettingsProvider(new[] {_source.Object});
        }

        [Fact]
        public void Contain()
        {
            _settings.Contains(_property).Should().BeTrue();
        }

        [Fact]
        public void ContainNested()
        {
            const string property = nameof(BigObject.String);
            _settings.Contains($"{_nestedProperty}.{property}").Should().BeTrue();
        }
        
        [Fact]
        public void ContainDeepNested()
        {
            const string boo = nameof(BigObject.Boo);
            const string booProperty = nameof(Boo.Type);
            
            _settings.Contains($"{_nestedProperty}.{boo}.{booProperty}").Should().BeTrue();
        }
        
        [Fact]
        public void ContainSource()
        {
            ((SettingsProvider)_settings).Sources.Should().Contain(_source.Object);
        }
        
        [Fact]
        public void GetValue()
        {
            var value = _settings.Get(_property);
            value.Should().Be(_propertyValue.Value);
        }

        [Fact]
        public void GetNestedValue()
        {
            const string property = nameof(BigObject.String);
            
            var value = _settings.Get($"{_nestedProperty}.{property}");
            value.Should().Be(((JsonValue) _nestedValue[property]).Value);
        }
        
        [Fact]
        public void GetDeepNestedValue()
        {
            const string boo = nameof(BigObject.Boo);
            const string booProperty = nameof(Boo.Type);
            
            var value = _settings.Get($"{_nestedProperty}.{boo}.{booProperty}");
            var jsonValue = (JsonValue) ((JsonObject) _nestedValue[boo])[booProperty];
            value.Should().Be(jsonValue.Value);
        }
        
        [Fact]
        public void GetValueTyped()
        {
            var value = _settings.Get<bool>(_property);
            value.Should().Be(_converters.Get<bool>().Read(_propertyValue));
        }

        [Fact]
        public void GetNestedValueTyped()
        {
            const string property = nameof(BigObject.Array);
            
            var value = _settings.Get<int[]>($"{_nestedProperty}.{property}");
            value.Should().Contain(_converters.Get<int[]>().Read(_nestedValue[property]));
        }
        
        [Fact]
        public void GetDeepNestedValueTyped()
        {
            const string boo = nameof(BigObject.Boo);
            const string booProperty = nameof(Boo.Type);
            
            var value = _settings.Get<ModelType>($"{_nestedProperty}.{boo}.{booProperty}");
            var expected = _converters.Get<ModelType>().Read(((JsonObject) _nestedValue[boo])[booProperty]);
            value.Should().Be(expected);
        }

        [Fact]
        public void OverrideValues()
        {
            var settings = new SettingsProvider(new ISettingsSource[]
            {
                new JsonFileSource("Settings/appsettings.json", true),
                new JsonFileSource("Settings/appsettings.develop.json", true),
            });

            var logLevelSettings = settings.Get<LogLevelSettings>("Logging.LogLevel");
            logLevelSettings.Default.Should().Be("Debug");
            logLevelSettings.System.Should().Be("Debug");
            logLevelSettings.Microsoft.Should().Be("Information");
        }
        
        [Fact]
        public void NotContain()
        {
            _settings.Contains("abc").Should().BeFalse();
        }
        
        [Fact]
        public void NotContainNested()
        {
            _settings.Contains("abc.abc").Should().BeFalse();
        }
        
        [Fact]
        public void NotContainDeepNested()
        {
            _settings.Contains("abc.abc.abc").Should().BeFalse();
        }

        [Fact]
        public void ReloadCallSources()
        {
            _settings.Reload();

            var outValue = _out;
            _source.Verify(source => source.TryGet(out outValue), Times.Exactly(2)); // load and reload
        }

        [Fact]
        public void TryGetValue()
        {
            const string boo = nameof(BigObject.Boo);

            _settings.TryGet<Boo>($"{_nestedProperty}.{boo}", out var booValue).Should().BeTrue();
            
            var expected = _converters.Get<Boo>().Read((JsonObject) _nestedValue[boo]);
            booValue.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void TryGetValueFalse()
        {
            _settings.TryGet<Boo>($"{_nestedProperty}.abc", out _).Should().BeFalse();
        }
        
        [Fact]
        public void ThrowPathNotFound()
        {
            _settings
                .Invoking(settings => settings.Get("abc.abc"))
                .Should().Throw<KeyNotFoundException>();
        }
        
        [Fact]
        public void ThrowNestedPathNotFound()
        {
            _settings
                .Invoking(settings => settings.Get($"{_nestedProperty}.abc"))
                .Should().Throw<KeyNotFoundException>();
        }
        
        [Fact]
        public void ThrowInvalidCast()
        {
            _settings
                .Invoking(settings => settings.Get($"{_nestedProperty}.{nameof(BigObject.Array)}"))
                .Should().Throw<InvalidCastException>();
        }
        
        private Mock<ISettingsSource> BuildSource()
        {
            var source = new Mock<ISettingsSource>();

            var outResult = _out;
            source
                .Setup(s => s.TryGet(out outResult))
                .Returns(true);

            return source;
        }
    }
}