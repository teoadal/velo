namespace Velo.Collections
{
    public static class LocalVectorExtensions
    {
        public static int Sum(this LocalVector<int> vector)
        {
            var sum = 0;
            
            foreach (var element in vector)
            {
                sum += element;
            }

            return sum;
        }
    }
}