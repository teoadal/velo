using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection.Dependencies
{
    public class ScopedDependencyShould : DITestClass
    {
        private readonly Type _contract;
        private readonly ScopedDependency _dependency;
        private readonly Mock<IDependencyScope> _scope;

        private Mock<IBooRepository> _instance;

        public ScopedDependencyShould(ITestOutputHelper output) : base(output)
        {
            _contract = typeof(IBooRepository);

            var resolver = MockResolver(_contract, (type, scope) =>
            {
                _instance = new Mock<IDisposable>().As<IBooRepository>();
                return _instance.Object;
            });

            _dependency = new ScopedDependency(new[] {_contract}, resolver.Object);

            _scope = MockScope();
        }

        [Fact]
        public void DisposeInstanceAfterDestroyScope()
        {
            _dependency.GetInstance(_contract, _scope.Object);

            RaiseScopeDestroy(_scope);

            _instance.As<IDisposable>().Verify(instance => instance.Dispose());
        }

        [Fact]
        public void DisposeAllInstancesAfterDispose()
        {
            var scopes = Enumerable.Range(0, 5).Select(_ => MockScope());
            var instances = new List<Mock<IBooRepository>>();

            foreach (var scope in scopes)
            {
                _dependency.GetInstance(_contract, scope.Object);
                instances.Add(_instance);
            }

            _dependency.Dispose();

            foreach (var instance in instances)
            {
                instance.As<IDisposable>().Verify(i => i.Dispose(), Times.Once);
            }
        }

        [Fact]
        public void GetInstance()
        {
            _dependency
                .GetInstance(_contract, _scope.Object)
                .Should().Be(_instance.Object);
        }

        [Fact]
        public void GetEqualInstanceInOneScope()
        {
            var scope = _scope.Object;
            var firstInstance = _dependency.GetInstance(_contract, scope);
            var secondInstance = _dependency.GetInstance(_contract, scope);

            firstInstance.Should().Be(secondInstance);
        }

        [Fact]
        public void GetNotEqualInstanceInDifferentScopes()
        {
            var firstScope = _scope.Object;
            var secondScope = MockScope().Object;

            var firstInstance = _dependency.GetInstance(_contract, firstScope);
            var secondInstance = _dependency.GetInstance(_contract, secondScope);

            firstInstance.Should().NotBe(secondInstance);
        }

        [Fact]
        public void SubscribeOnceToOneScope()
        {
            var scope = _scope.Object;
            _dependency.GetInstance(_contract, scope);
            _dependency.GetInstance(_contract, scope);

            _scope.VerifyAdd(s => s.Destroy += It.IsAny<Action<IDependencyScope>>(), Times.Once);
        }

        [Fact]
        public void SubscribeToScope()
        {
            _dependency.GetInstance(_contract, _scope.Object);
            _scope.VerifyAdd(scope => scope.Destroy += It.IsAny<Action<IDependencyScope>>());
        }

        [Fact]
        public void UnsubscribeFromScope()
        {
            _dependency.GetInstance(_contract, _scope.Object);

            RaiseScopeDestroy(_scope);

            _scope.VerifyRemove(s => s.Destroy -= It.IsAny<Action<IDependencyScope>>());
        }

        [Fact]
        public void UnsubscribeFromAllScopes()
        {
            var scopes = Enumerable
                .Range(0, 5).Select(_ => MockScope())
                .ToArray();

            foreach (var scope in scopes)
            {
                _dependency.GetInstance(_contract, scope.Object);
            }

            _dependency.Dispose();

            foreach (var scope in scopes)
            {
                scope.VerifyRemove(s => s.Destroy -= It.IsAny<Action<IDependencyScope>>());
            }
        }

        private static void RaiseScopeDestroy(Mock<IDependencyScope> scope)
        {
            scope.Raise(s => s.Destroy += null, scope.Object);
        }
    }
}