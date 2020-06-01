using System;
using System.Linq.Expressions;
using System.Reflection;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.ECS.Actors.Context;

namespace Velo.ECS.Injection
{
    internal sealed class GroupFactory<TContext> : IDependencyFactory
        where TContext : class
    {
        private readonly Type _baseGroupType;
        private readonly MethodInfo _resolveMethod;

        public GroupFactory(Type baseGroupType)
        {
            _baseGroupType = baseGroupType;
            _resolveMethod = typeof(TContext).GetMethod(nameof(IActorContext.GetGroup))!;
        }

        public bool Applicable(Type contract)
        {
            return _baseGroupType.IsAssignableFrom(contract);
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