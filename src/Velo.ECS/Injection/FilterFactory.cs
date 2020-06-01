using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.ECS.Actors.Context;
using Velo.Utils;

namespace Velo.ECS.Injection
{
    internal sealed class FilterFactory<TContext> : IDependencyFactory
        where TContext : class
    {
        private readonly Type _baseFilterType;
        private readonly Dictionary<int, MethodInfo> _resolveMethods;

        public FilterFactory(Type baseFilterType)
        {
            _baseFilterType = baseFilterType;

            _resolveMethods = typeof(TContext)
                .GetMethods()
                .Where(method => method.IsGenericMethod && method.Name == nameof(IActorContext.GetFilter))
                .ToDictionary(method => method.GetGenericArguments().Length);
        }

        public bool Applicable(Type contract)
        {
            return _baseFilterType.IsAssignableFrom(contract);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var genericArguments = contract.GenericTypeArguments;

            if (!_resolveMethods.TryGetValue(genericArguments.Length, out var genericMethod))
            {
                throw Error.InvalidOperation($"Resolve {ReflectionUtils.GetName(contract)} isn't supported");
            }

            var callMethod = genericMethod.MakeGenericMethod(genericArguments);
            var parameter = Expression.Parameter(typeof(TContext));
            var resolveMethod = Expression
                .Lambda<Func<TContext, object>>(Expression.Call(parameter, callMethod), parameter)
                .Compile();

            return new ContextDependency<TContext>(contract, resolveMethod);
        }
    }
}