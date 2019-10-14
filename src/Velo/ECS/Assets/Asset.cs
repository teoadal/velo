namespace Velo.ECS.Assets
{
    public class Asset : Entity
    {
        internal Asset(int id, IComponent[] components) : base(id, components)
        {
        }
    }
}