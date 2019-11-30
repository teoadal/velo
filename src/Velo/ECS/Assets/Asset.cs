namespace Velo.ECS.Assets
{
    public class Asset : Entity
    {
        public Asset(int id, IComponent[] components) : base(id, components)
        {
        }
    }
}