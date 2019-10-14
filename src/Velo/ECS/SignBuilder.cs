using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.ECS
{
    internal static class SignBuilder
    {
        public static Sign Create(IComponent[] components)
        {
            switch (components.Length)
            {
                case 0:
                    return new Sign(Sign.EMPTY_INDEX);
                case 1:
                    return new Sign(GetTypeId(components[0]));
                case 2:
                    return new Sign(GetTypeId(components[0]), GetTypeId(components[1]));
                case 3:
                    return new Sign(GetTypeId(components[0]), GetTypeId(components[1]), GetTypeId(components[2]));
                case 4:
                    return new Sign(GetTypeId(components[0]), GetTypeId(components[1]), GetTypeId(components[2]),
                        GetTypeId(components[3]));
                case 5:
                    return new Sign(GetTypeId(components[0]), GetTypeId(components[1]), GetTypeId(components[2]),
                        GetTypeId(components[3]), GetTypeId(components[4]));
                default:
                    throw Error.OutOfRange();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetTypeId(IComponent component)
        {
            return Typeof.GetTypeId(component.GetType());
        }
    }
}