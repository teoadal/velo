using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Velo.DependencyInjection.Dependencies;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    [DebuggerDisplay("Implementation = {_constructor.DeclaringType}")]
    internal sealed class CompiledResolver : DependencyResolver
    {
        private Func<IDependencyScope, object> _builder;
        private readonly ConstructorInfo _constructor;
        private readonly IDependencyEngine _dependencyEngine;

        public CompiledResolver(Type implementation, IDependencyEngine dependencyEngine)
            : base(implementation)
        {
            _constructor = ReflectionUtils.GetConstructor(implementation);
            _dependencyEngine = dependencyEngine;
        }

        protected override object ResolveInstance(Type contract, IDependencyScope scope)
        {
            if (_builder == null) _builder = CreateBuilder(scope);
            return _builder(scope);
        }

        private Func<IDependencyScope, object> CreateBuilder(IDependencyScope scope)
        {
            var constructorParameters = _constructor.GetParameters();

            var argument = Expression.Parameter(typeof(IDependencyScope), "scope");
            var parameters = new Expression[constructorParameters.Length];
            for (var i = 0; i < constructorParameters.Length; i++)
            {
                var parameter = constructorParameters[i];
                var parameterType = parameter.ParameterType;
                var required = !parameter.HasDefaultValue;

                var parameterDependency = _dependencyEngine.GetDependency(parameterType, required);
                parameters[i] = BuildParameter(parameterDependency, parameterType, argument, scope);
            }

            var body = Expression.New(_constructor, parameters);
            var result = Expression.Lambda<Func<IDependencyScope, object>>(body, argument).Compile();

            return result;
        }

        private Expression BuildParameter(IDependency parameterDependency, Type parameterType, Expression argument,
            IDependencyScope scope)
        {
            if (parameterDependency == null) return Expression.Default(parameterType);

            switch (parameterDependency.Lifetime)
            {
                case DependencyLifetime.Scoped:
                case DependencyLifetime.Transient:
                    var dependencyConstant = Expression.Constant(parameterDependency);

                    // ReSharper disable AssignNullToNotNullAttribute
                    var getInstanceMethod = parameterDependency.GetType().GetMethod(nameof(IDependency.GetInstance));
                    var parameterInstanceCall = Expression.Call(
                        dependencyConstant, getInstanceMethod,
                        Expression.Constant(parameterType), argument);
                    // ReSharper restore AssignNullToNotNullAttribute

                    return Expression.Convert(parameterInstanceCall, parameterType);

                case DependencyLifetime.Singleton:
                    var parameterConstant = parameterDependency.GetInstance(parameterType, scope);
                    return Expression.Constant(parameterConstant);
            }

            throw Error.InvalidDependencyLifetime();
        }
    }
}