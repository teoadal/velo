using System;
using System.Collections.Generic;
using System.IO;
using Velo.Dependencies;
using Velo.Dependencies.Singletons;
using Velo.Emitting.Queries;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Boos.Emitting;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Utils
{
    public class ReflectionUtilsTests : TestBase
    {
        public ReflectionUtilsTests(ITestOutputHelper output) : base(output)
        {
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
                typeof(GetBooHandler),
                typeof(IQueryHandler<,>));

            Assert.Contains(typeof(GetBoo), genericInterfaceParameters);
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
        public void GenericInterfaceParameters_Throw_NotInterface()
        {
            Assert.Throws<InvalidDataException>(() =>
                ReflectionUtils.GetGenericInterfaceParameters(typeof(Dictionary<int, int>), typeof(Dictionary<,>)));
        }
        
        [Fact]
        public void GenericInterfaceParameters_Throw_NotImplemented()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                ReflectionUtils.GetGenericInterfaceParameters(typeof(Dictionary<int, int>), typeof(IQueryHandler<,>)));
        }
        
        [Fact]
        public void GenericInterfaceParameters_Throw_Abstract()
        {
            Assert.Throws<InvalidDataException>(() =>
                ReflectionUtils.GetGenericInterfaceParameters(typeof(ActivatorSingleton), typeof(Dependency)));
        }
    }
}