namespace Velo.Collections
{
    public static class SequenceResizeRule
    {
        public static int Default(int length)
        {
            if (length == 0) return 4;
            return length * 2;
        }

        public static int Increase(int length)
        {
            return length + 1;
        }
    }
}