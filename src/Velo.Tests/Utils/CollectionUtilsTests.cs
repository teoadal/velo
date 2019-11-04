using System.Collections.Concurrent;
using System.Collections.Generic;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Utils
{
    public class CollectionUtilsTests : TestBase
    {
        public CollectionUtilsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void DisposeValuesIfDisposable_ConcurrentDictionary()
        {
            var collection = new ConcurrentDictionary<int, IRepository>();
            var booRepository = new BooRepository(null, null);
            collection.TryAdd(1, booRepository);
            
            CollectionUtils.DisposeValuesIfDisposable(collection);
            Assert.True(booRepository.Disposed);
        }
        
        [Fact]
        public void DisposeValuesIfDisposable_Dictionary()
        {
            var collection = new Dictionary<int, IRepository>();
            var booRepository = new BooRepository(null, null);
            collection.Add(1, booRepository);
            
            CollectionUtils.DisposeValuesIfDisposable(collection);
            Assert.True(booRepository.Disposed);
        }
        
        [Fact]
        public void DisposeValuesIfDisposable_Array()
        {
            var collection = new IRepository[1];
            var booRepository = new BooRepository(null, null);
            collection[0] = booRepository;
            
            CollectionUtils.DisposeValuesIfDisposable(collection);
            Assert.True(booRepository.Disposed);
        }
    }
}