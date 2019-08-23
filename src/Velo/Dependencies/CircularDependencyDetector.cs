using System;
using System.Collections.Generic;

namespace Velo.Dependencies
{
    internal sealed class CircularDependencyDetector
    {
        private int _callDepth;
        private readonly Stack<Type> _callStack;
        private readonly int _maxResolveDepth;

        public CircularDependencyDetector(int maxResolveDepth)
        {
            _callDepth = 0;
            _callStack = new Stack<Type>();
            _maxResolveDepth = maxResolveDepth;
        }

        public void Call(Type type, bool required)
        {
            _callDepth++;
            
            if (_callDepth > _maxResolveDepth || required && _callStack.Contains(type))
            {
                var resolveStack = string.Join(" -> ", _callStack);
                throw new InvalidOperationException($"Circular dependency detected ({resolveStack})");
            }
            
            _callStack.Push(type);
        }
        
        public void Resolved()
        {
            _callDepth--;
            _callStack.Pop();
        }

    }
}