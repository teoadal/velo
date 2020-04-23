namespace Velo.ECS.Actors.Filters
{
    public interface IActorFilter
    {
        int Length { get; }

        bool Contains(int actorId);
    }
}