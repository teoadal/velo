using System;
using System.Collections.Concurrent;
using Velo.Patching.Methods;
using Velo.Utils;

namespace Velo.Patching
{
    public sealed class PatchBuilder
    {
        private readonly ConcurrentDictionary<int, IPatchMethods> _objects;
        
        public PatchBuilder()
        {
            _objects = new ConcurrentDictionary<int, IPatchMethods>(Environment.ProcessorCount, 20);
        }

        public Patch<T> CreatePatch<T>() where T : class
        {
            var patchObject = _objects.GetOrAdd(Typeof<T>.Id, _ => new PatchMethods<T>());
            return new Patch<T>((PatchMethods<T>) patchObject);
        }
    }
}