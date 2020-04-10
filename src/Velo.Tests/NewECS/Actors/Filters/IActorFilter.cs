namespace Velo.Tests.NewECS.Actors.Filters
{
    public interface IActorFilter
    {
        int Length { get; }

        bool Contains(int actorId);
    }
}