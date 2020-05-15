using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Collections;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections
{
    public class CollectionUtilsShould : TestClass
    {
        public CollectionUtilsShould(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, AutoData]
        public void AddToEnd(int value)
        {
            var array = new int[0];
            
            CollectionUtils.Add(ref array, value);

            array.Should().Contain(value);
            array.Length.Should().Be(1);
        }
        
        [Fact]
        public void DisposeValuesIfDisposable_ConcurrentDictionary()
        {
            var collection = new ConcurrentDictionary<int, IRepository>();
            var booRepository = new BooRepository(null, null);
            collection.TryAdd(1, booRepository);
            
            CollectionUtils.DisposeValuesIfDisposable(collection);
            
            booRepository.Disposed.Should().BeTrue();
        }
        
        [Fact]
        public void DisposeValuesIfDisposable_Dictionary()
        {
            var collection = new Dictionary<int, IRepository>();
            var booRepository = new BooRepository(null, null);
            collection.Add(1, booRepository);
            
            CollectionUtils.DisposeValuesIfDisposable(collection);

            booRepository.Disposed.Should().BeTrue();
        }
        
        [Fact]
        public void DisposeValuesIfDisposable_Array()
        {
            var collection = new IRepository[1];
            var booRepository = new BooRepository(null, null);
            collection[0] = booRepository;
            
            CollectionUtils.DisposeValuesIfDisposable(collection);

            booRepository.Disposed.Should().BeTrue();
        }

        [Fact]
        public void EnsureCapacity()
        {
            var array = new int[0];
            
            CollectionUtils.EnsureCapacity(ref array, 5);
            
            array.Length.Should().Be(5);
        }

        [Fact]
        public void EnsureUnique()
        {
            var array = Fixture.CreateMany<int>().ToArray();
            array = array.Concat(array).ToArray();

            bool existsNotUnique = false;
            CollectionUtils.EnsureUnique(array, value => existsNotUnique = true);

            existsNotUnique.Should().BeTrue();
        }

        [Theory, AutoData]
        public void InsertWithoutResize(int value)
        {
            var array = new int[5];
            
            CollectionUtils.Insert(ref array, 1, value);
            array[1].Should().Be(value);
        }
        
        [Theory, AutoData]
        public void InsertWithResize(int value)
        {
            var array = new int[0];
            
            CollectionUtils.Insert(ref array, 1, value);
            array[1].Should().Be(value);
        }
    }
}