using Velo.CQRS.Commands;

namespace Velo.TestsModels.Emitting
{
    public interface IMeasureCommand : ICommand
    {
        bool Measured { get; set; }
    }
}