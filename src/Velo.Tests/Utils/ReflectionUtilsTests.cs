using System;
using System.Collections.Generic;
using System.IO;
using AutoFixture.Xunit2;
using Velo.CQRS.Queries;
using Velo.DependencyInjection.Dependencies;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Utils
{
    public class ReflectionUtilsTests : TestBase
    {
        public ReflectionUtilsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, AutoData]
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
            Assert.Throws<InvalidOperationException>(() => ReflectionUtils.GetConstructor(typeof(TestBase)));
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
            var disposable = new BooRepository(null, null);
            Assert.True(ReflectionUtils.IsDisposable(disposable, out _));
            Assert.True(ReflectionUtils.IsDisposableType(disposable.GetType()));
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
                ReflectionUtils.GetGenericInterfaceParameters(typeof(ScopeDependency), typeof(Dependency)));
        }
        
        
    }
}