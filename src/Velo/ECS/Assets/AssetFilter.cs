using System;
using System.Collections.Generic;
using Velo.ECS.Enumeration;
using Velo.Utils;

namespace Velo.ECS.Assets
{
    public abstract class AssetFilter : EntityFilter<Asset>
    {
        public event Action<Asset> Added; 
        
        protected AssetFilter(params int[] componentTypeIds) : base(componentTypeIds)
        {
        }

        protected override void OnAdded(Asset asset)
        {
            var evt = Added;
            evt?.Invoke(asset);
        }
    }

    public sealed class AssetFilter<TComponent1> : AssetFilter
        where TComponent1 : IComponent
    {
        private readonly List<Wrapper<Asset, TComponent1>> _wrappers;

        internal AssetFilter() : base(Typeof<TComponent1>.Id)
        {
            _wrappers = new List<Wrapper<Asset, TComponent1>>();
        }

        public List<Wrapper<Asset, TComponent1>>.Enumerator GetEnumerator() => _wrappers.GetEnumerator();

        public WhereFilter<Asset, TComponent1> Where(Predicate<Wrapper<Asset, TComponent1>> predicate)
        {
            return new WhereFilter<Asset, TComponent1>(_wrappers.GetEnumerator(), predicate);
        }

        protected override bool Add(Asset asset)
        {
            _wrappers.Add(new Wrapper<Asset, TComponent1>(asset, asset.Get<TComponent1>()));

            return true;
        }
    }

    public sealed class AssetFilter<TComponent1, TComponent2> : AssetFilter
        where TComponent1 : IComponent
        where TComponent2 : IComponent
    {
        private readonly List<Wrapper<Asset, TComponent1, TComponent2>> _wrappers;

        internal AssetFilter() : base(Typeof<TComponent1>.Id, Typeof<TComponent2>.Id)
        {
            _wrappers = new List<Wrapper<Asset, TComponent1, TComponent2>>();
        }

        public List<Wrapper<Asset, TComponent1, TComponent2>>.Enumerator GetEnumerator() => _wrappers.GetEnumerator();

        public WhereFilter<Asset, TComponent1, TComponent2> Where(
            Predicate<Wrapper<Asset, TComponent1, TComponent2>> predicate)
        {
            return new WhereFilter<Asset, TComponent1, TComponent2>(_wrappers.GetEnumerator(), predicate);
        }

        protected override bool Add(Asset asset)
        {
            _wrappers.Add(new Wrapper<Asset, TComponent1, TComponent2>(
                asset,
                asset.Get<TComponent1>(),
                asset.Get<TComponent2>()));

            return true;
        }
    }
}