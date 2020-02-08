namespace Velo.Logging
{
    // ReSharper disable once UnusedTypeParameter
    public interface ILogger<TSource>
    {
        void Debug(string template);

        void Debug<T1>(string template, T1 arg1);

        void Debug<T1, T2>(string template, T1 arg1, T2 arg2);

        void Debug<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3);

        void Debug(string template, params object[] args);
        
        void Info(string template);

        void Info<T1>(string template, T1 arg1);

        void Info<T1, T2>(string template, T1 arg1, T2 arg2);

        void Info<T1, T2, T3>(string template, T1 arg1, T2 arg2, T3 arg3);

        void Info(string template, params object[] args);
    }
}