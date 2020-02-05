using System.Threading.Tasks;

namespace Velo.CQRS.Pipeline
{
    public interface IPipelineBehaviour<in T>
    {
        ValueTask BeginPipeline(T input);
        
        ValueTask EndPipeline(T input);
    }
}