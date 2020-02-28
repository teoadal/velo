using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Velo.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Utils
{
    public class TypeofUtilsTests : TestClass
    {
        private readonly Type[] _types;

        public TypeofUtilsTests(ITestOutputHelper output) : base(output)
        {
            _types = new[] {typeof(Boo), typeof(BooRepository), typeof(Foo), typeof(FooRepository)};
        }

        [Fact]
        public void GetId()
        {
            Parallel.ForEach(_types, t => Typeof.GetTypeId(t));

            var unique = new HashSet<int>();
            foreach (var type in _types)
            {
                Assert.True(unique.Add(Typeof.GetTypeId(type)));
            }
        }

        [Fact]
        public void GenericIds()
        {
            static int Check<T>()
            {
                var genericTypeId = Typeof<T>.Id;
                Assert.Equal(Typeof.GetTypeId(typeof(T)), genericTypeId);
                return genericTypeId;
            }

            var unique = new HashSet<int>();

            Assert.True(unique.Add(Check<Boo>()));
            Assert.True(unique.Add(Check<BooRepository>()));
            Assert.True(unique.Add(Check<Foo>()));
            Assert.True(unique.Add(Check<FooRepository>()));
        }
    }
}