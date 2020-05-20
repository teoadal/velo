using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Velo.Collections;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.Collections
{
    public class DangerousVectorShould : TestClass
    {
        private readonly string _arg;
        private readonly Type[] _types;

        private readonly DangerousVector<Type, string> _vector;

        public DangerousVectorShould()
        {
            _arg = nameof(DangerousVectorShould);
            _vector = new DangerousVector<Type, string>();

            _types = typeof(DangerousVector<,>).Assembly.DefinedTypes.Cast<Type>().ToArray();
        }

        [Fact]
        public void Add()
        {
            foreach (var type in _types)
            {
                var name = _vector.GetOrAdd(type, t => t.Name);
                name.Should().Be(type.Name);
            }
        }

        [Fact]
        public void AddExists()
        {
            foreach (var type in _types)
            {
                _vector.TryAdd(type, type.Name);
                _vector
                    .Invoking(vector => vector.GetOrAdd(type, t => t.Name))
                    .Should().NotThrow()
                    .Which.Should().Be(type.Name);
            }
        }
        
        [Fact]
        public void AddExistsParallel()
        {
            Parallel.ForEach(_types, type =>
            {
                _vector.TryAdd(type, type.Name);
                _vector
                    .Invoking(vector => vector.GetOrAdd(type, t => t.Name))
                    .Should().NotThrow()
                    .Which.Should().Be(type.Name);
            });
        }
        
        [Fact]
        public void AddExistsWithArg()
        {
            foreach (var type in _types)
            {
                _vector.TryAdd(type, type.Name);
                _vector
                    .Invoking(vector => vector.GetOrAdd(type, (t, arg) => t.Name, _arg))
                    .Should().NotThrow()
                    .Which.Should().Be(type.Name);
            }
        }
        
        [Fact]
        public void AddExistsParallelWithArg()
        {
            Parallel.ForEach(_types, type =>
            {
                _vector.TryAdd(type, type.Name);
                _vector
                    .Invoking(vector => vector.GetOrAdd(type, (t, arg) => t.Name, _arg))
                    .Should().NotThrow()
                    .Which.Should().Be(type.Name);
            });
        }
        
        [Fact]
        public void AddParallel()
        {
            Parallel.ForEach(_types, type =>
            {
                var name = _vector.GetOrAdd(type, t => t.Name);
                name.Should().Be(type.Name);
            });
        }

        [Fact]
        public void AddWithArg()
        {
            foreach (var type in _types)
            {
                var name = _vector.GetOrAdd(type, (t, arg) => t.Name + arg, _arg);
                name.Should().Be(type.Name + _arg);
            }
        }

        [Fact]
        public void AddWithArgParallel()
        {
            Parallel.ForEach(_types, type =>
            {
                var name = _vector.GetOrAdd(type, (t, arg) => t.Name + arg, _arg);
                name.Should().Be(type.Name + _arg);
            });
        }

        [Fact]
        public void Clear()
        {
            _vector.Add(typeof(Boo), nameof(Boo));

            _vector
                .Invoking(vector => vector.ClearSafe())
                .Should().NotThrow();

            _vector.Count.Should().Be(0);
        }

        [Fact]
        public void ClearParallel()
        {
            Parallel.For(0, 10, i =>
            {
                _vector
                    .Invoking(vector => vector.ClearSafe())
                    .Should().NotThrow();
            });
        }

        [Fact]
        public void InitializeFromDictionary()
        {
            var dictionary = _types.ToDictionary(type => type, type => type.Name);
            var vector = new DangerousVector<Type, string>(dictionary);

            vector.Count.Should().Be(dictionary.Count);
        }

        [Fact]
        public void TryAdd()
        {
            _vector
                .Invoking(vector => vector.TryAdd(typeof(object), string.Empty))
                .Should().NotThrow();
        }
    }
}