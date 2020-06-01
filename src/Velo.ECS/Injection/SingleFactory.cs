using System;
using System.Linq.Expressions;
using System.Reflection;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.ECS.Actors.Context;
using Velo.Utils;

namespace Velo.ECS.Injection
{
    internal sealed class SingleFactory<TContext> : IDependencyFactory
        where TContext : class
    {
        private readonly Type _singleGenericType;
        private readonly MethodInfo _resolveMethod;

        public SingleFactory(Type singleGenericType)
        {
            _singleGenericType = singleGenericType;
            _resolveMethod = typeof(TContext).GetMethod(nameof(IActorContext.GetSingle))!;
        }

        public bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, _singleGenericType);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var callMethod = _resolveMethod.MakeGenericMethod(contract.GenericTypeArguments);
            var parameter = Expression.Parameter(typeof(TContext));

            var resolveMethod = Expression
                .Lambda<Func<TContext, object>>(Expression.Call(parameter, callMethod), parameter)
                .Compile();

            return new ContextDependency<TContext>(contract, resolveMethod);
        }
    }
}