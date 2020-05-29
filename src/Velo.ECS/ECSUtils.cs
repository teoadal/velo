using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Velo.Collections.Local;
using Velo.ECS.Systems;
using Velo.Utils;

namespace Velo.ECS
{
    internal static class ECSUtils
    {
        private static readonly Type[] SystemTypes =
        {
            typeof(IBootstrapSystem),
            typeof(ICleanupSystem),
            typeof(IInitSystem),
            typeof(IBeforeUpdateSystem),
            typeof(IUpdateSystem),
            typeof(IAfterUpdateSystem)
        };

        public static Func<TSystem, CancellationToken, Task> BuildSystemUpdateMethod<TSystem>()
        {
            var systemType = typeof(TSystem);
            
            string methodName;
            if (systemType == typeof(IAfterUpdateSystem)) methodName = nameof(IAfterUpdateSystem.AfterUpdate);
            else if (systemType == typeof(IBeforeUpdateSystem)) methodName = nameof(IBeforeUpdateSystem.BeforeUpdate);
            else if (systemType == typeof(IBootstrapSystem)) methodName = nameof(IBootstrapSystem.Bootstrap);
            else if (systemType == typeof(ICleanupSystem)) methodName = nameof(ICleanupSystem.Cleanup);
            else if (systemType == typeof(IInitSystem)) methodName = nameof(IInitSystem.Init);
            else if (systemType == typeof(IUpdateSystem)) methodName = nameof(IUpdateSystem.Update);
            else throw Error.InvalidOperation("Isn't system type");

            var system = Expression.Parameter(systemType);
            var token = Expression.Parameter(typeof(CancellationToken));
            var call = ExpressionUtils.Call(system, methodName, token);

            return Expression
                .Lambda<Func<TSystem, CancellationToken, Task>>(call, system, token)
                .Compile();
        }
        
        public static bool TryGetImplementedSystemInterfaces(Type implementation, out Type[] interfaces)
        {
            var assignable = new LocalList<Type>();
            foreach (var typeInterface in implementation.GetInterfaces())
            {
                if (Array.IndexOf(SystemTypes, typeInterface) == -1) continue;

                assignable.Add(typeInterface);
            }

            if (assignable.Length == 0)
            {
                interfaces = null!;
                return false;
            }

            interfaces = assignable.ToArray();
            return true;
        }
    }
}