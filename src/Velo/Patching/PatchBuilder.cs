using System;
using System.Collections.Concurrent;
using Velo.Utils;

namespace Velo.Patching
{
    public sealed class PatchBuilder
    {
        private readonly ConcurrentDictionary<int, IPatchObject> _objects;
        
        public PatchBuilder()
        {
            _objects = new ConcurrentDictionary<int, IPatchObject>(Environment.ProcessorCount, 20);
        }

        public Patch<T> CreatePatch<T>() where T : class
        {
            var patchObject = _objects.GetOrAdd(Typeof<T>.Id, _ => new PatchObject<T>());
            return new Patch<T>((PatchObject<T>) patchObject);
        }
    }
}