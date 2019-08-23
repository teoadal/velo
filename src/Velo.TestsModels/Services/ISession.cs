using Velo.Serialization;

namespace Velo.TestsModels.Services
{
    public interface ISession
    {
        JConverter Converter { get; }
    }

    public class Session : ISession
    {
        public JConverter Converter { get; }
        
        public Session(JConverter converter)
        {
            Converter = converter;
        }
    }
}