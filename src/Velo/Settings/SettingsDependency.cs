using System;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.Utils;

namespace Velo.Settings
{
    internal sealed class SettingsDependency<T> : Dependency
    {
        private readonly IDependency _configurationDependency;
        private readonly string _path;
            
        public SettingsDependency(IDependency configurationDependency, string path) 
            : base(new []{ Typeof<T>.Raw }, DependencyLifetime.Transient)
        {
            _configurationDependency = configurationDependency;
            _path = path;
        }

        public override object GetInstance(Type contract, IDependencyScope scope)
        {
            var configuration = GetConfiguration(scope);
            return configuration.Get<T>(_path);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IConfiguration GetConfiguration(IDependencyScope scope)
        {
            return (IConfiguration) _configurationDependency.GetInstance(Typeof<IConfiguration>.Raw, scope);
        }
        
        public override void Dispose()
        {
        }
    }
}