namespace Velo.ECS.Components
{
    internal static class ComponentExtensions
    {
        public static bool ContainsComponents<TComponent1, TComponent2>(this IComponent[] array)
            where TComponent1 : IComponent where TComponent2 : IComponent
        {
            if (array.Length < 2) return false;

            var counter = 0;

            foreach (var component in array)
            {
                switch (component)
                {
                    case TComponent1 _:
                    case TComponent2 _:
                        counter++;
                        break;
                }

                if (counter == 2) return true;
            }

            return false;
        }

        public static bool TryGetComponents<TComponent1, TComponent2>(
            this IComponent[] array,
            out TComponent1 component1,
            out TComponent2 component2)
        {
            if (array.Length < 2)
            {
                component1 = default!;
                component2 = default!;

                return false;
            }

            component1 = default!;
            component2 = default!;

            var counter = 0;
            foreach (var component in array)
            {
                switch (component)
                {
                    case TComponent1 found1:
                        component1 = found1;
                        counter++;
                        continue;
                    case TComponent2 found2:
                        component2 = found2;
                        counter++;
                        continue;
                }

                if (counter == 2) return true;
            }

            return counter == 2;
        }
    }
}