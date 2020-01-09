namespace Velo.Collections
{
    public static class LocalVectorExtensions
    {
        public static int Sum(this LocalList<int> list)
        {
            var sum = 0;
            
            foreach (var element in list)
            {
                sum += element;
            }

            return sum;
        }
    }
}