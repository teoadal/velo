using System;

namespace Velo.Dependencies
{
    public sealed class DependencyContainer
    {
        public T Activate<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public object Activate(Type type)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type)
        {
            throw new NotImplementedException();
        }
    }
}