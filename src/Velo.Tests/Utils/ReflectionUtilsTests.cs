using System;
using System.Collections.Generic;
using System.IO;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.CQRS.Commands;
using Velo.CQRS.Queries;
using Velo.DependencyInjection.Dependencies;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Velo.Utils;
using Xunit;

namespace Velo.Tests.Utils
{
    public class ReflectionUtilsTests : TestClass
    {
        [Theory]
        [AutoData]
        public void GetArrayElementType(Type elementType)
        {
            var arrayType = elementType.MakeArrayType();
            Assert.Equal(elementType, ReflectionUtils.GetArrayElementType(arrayType));
        }
        
        [Fact]
        public void GetConstructor()
        {
            var constructor = ReflectionUtils.GetConstructor(typeof(BooRepository));
            Assert.NotNull(constructor);
        }

        [Fact]
        public void GetEmptyConstructor()
        {
            var constructor = ReflectionUtils.GetEmptyConstructor(typeof(List<string>));
            Assert.NotNull(constructor);
        }

        [Fact]
        public void GetEmptyConstructor_Without_Empty_Constructor()
        {
            var constructor = ReflectionUtils.GetEmptyConstructor(typeof(BooRepository));
            Assert.Null(constructor);
        }

        [Fact]
        public void GetConstructor_Throw_Static()
        {
            Assert.Throws<InvalidOperationException>(() => ReflectionUtils.GetConstructor(typeof(ReflectionUtils)));
        }

        [Fact]
        public void GetConstructor_Throw_Abstract()
        {
            Assert.Throws<InvalidOperationException>(() => ReflectionUtils.GetConstructor(typeof(TestClass)));
        }

        [Fact]
        public void GetConstructor_Throw_Interface()
        {
            Assert.Throws<InvalidOperationException>(() => ReflectionUtils.GetConstructor(typeof(IDisposable)));
        }

        [Fact]
        public void GenericInterfaceParameters()
        {
            var genericInterfaceParameters = ReflectionUtils.GetGenericInterfaceParameters(
                typeof(Processor),
                typeof(IQueryProcessor<,>));

            Assert.Contains(typeof(Query), genericInterfaceParameters);
            Assert.Contains(typeof(Boo), genericInterfaceParameters);
        }

        [Fact]
        public void GenericInterfaceImplementation()
        {
            var genericInterface = typeof(ICommandProcessor<>);
            var type = typeof(TestsModels.Emitting.Boos.Create.Processor);
            
            var implementations = ReflectionUtils.GetGenericInterfaceImplementations(type, genericInterface);
            
            implementations.Length.Should().Be(1);
            
            Assert.True(implementations.Contains(typeof(ICommandProcessor<TestsModels.Emitting.Boos.Create.Command>)));
        }
        
        [Fact]
        public void GenericInterfaceImplementations()
        {
            var genericInterfaces = new[] {typeof(ICommandProcessor<>), typeof(ICommandPostProcessor<>)};
            var type = typeof(TestsModels.Emitting.Boos.Create.Processor);
            
            var implementations = ReflectionUtils.GetGenericInterfaceImplementations(type, genericInterfaces);
            Assert.True(implementations.Contains(typeof(ICommandProcessor<TestsModels.Emitting.Boos.Create.Command>)));
            Assert.Equal(1, implementations.Length);
        }
        
        [Fact]
        public void GenericInterfaceParameters_FirstLevelInterface()
        {
            var genericInterfaceParameters = ReflectionUtils.GetGenericInterfaceParameters(
                typeof(Dictionary<int, string>),
                typeof(IDictionary<,>));

            Assert.Contains(typeof(int), genericInterfaceParameters);
            Assert.Contains(typeof(string), genericInterfaceParameters);
        }

        [Fact]
        public void GetName()
        {
            Assert.Equal("List<Int32>", ReflectionUtils.GetName(typeof(List<int>)));
            Assert.Equal("Dictionary<Int32, String>", ReflectionUtils.GetName(typeof(Dictionary<int, string>)));
            Assert.Equal("Dictionary<Int32, List<String>>", ReflectionUtils.GetName(typeof(Dictionary<int, List<string>>)));
            Assert.Equal("List<String>[]", ReflectionUtils.GetName(typeof(List<string>[])));
            Assert.Equal(nameof(IDisposable), ReflectionUtils.GetName(typeof(IDisposable)));
        }

        [Fact]
        public void IsDisposable()
        {
            var disposable = new BooRepository(null);
            Assert.True(ReflectionUtils.IsDisposable(disposable, out _));
            Assert.True(ReflectionUtils.IsDisposableType(disposable.GetType()));
        }

        [Theory]
        [AutoData]
        public void ThrowGetArrayElement_NotArray(Type elementType)
        {
            Assert.Throws<InvalidDataException>(() => ReflectionUtils.GetArrayElementType(elementType));
        }
        
        [Fact]
        public void Throw_GenericInterfaceParameters_NotInterface()
        {
            Assert.Throws<InvalidDataException>(() =>
                ReflectionUtils.GetGenericInterfaceParameters(typeof(Dictionary<int, int>), typeof(Dictionary<,>)));
        }
        
        [Fact]
        public void Throw_GenericInterfaceParameters_NotImplemented()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                ReflectionUtils.GetGenericInterfaceParameters(typeof(Dictionary<int, int>), typeof(IQueryProcessor<,>)));
        }
        
        [Fact]
        public void Throw_GenericInterfaceParameters_Abstract()
        {
            Assert.Throws<InvalidDataException>(() =>
                ReflectionUtils.GetGenericInterfaceParameters(typeof(ScopedDependency), typeof(Dependency)));
        }
        
        
    }
}