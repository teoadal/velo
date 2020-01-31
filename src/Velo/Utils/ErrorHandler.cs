using System;

namespace Velo.Utils
{
    public static class Error<TException>
        where TException : Exception
    {
        public static ErrorHandler<TException> Catch(Action<TException> exceptionHandler)
        {
            return new ErrorHandler<TException>(exceptionHandler);
        }

        public static ErrorHandler<TException> Map(Func<TException, Exception> exceptionMapper)
        {
            return new ErrorHandler<TException>(null, exceptionMapper);
        }

        public static ErrorHandler<TException> Try(int attempts = 1)
        {
            return new ErrorHandler<TException>(null, null, attempts, true);
        }
    }

    public ref struct ErrorHandler<TException>
        where TException : Exception
    {
        private int _attempts;
        private Action<TException> _attemptHandler;
        private bool _enabled;
        private Action<TException> _exceptionHandler;
        private Func<TException, Exception> _exceptionMapper;
        private bool _rethrow;

        internal ErrorHandler(Action<TException> handler, Func<TException, Exception> mapper = null, int attempts = 1,
            bool rethrow = false)
        {
            _attempts = attempts;
            _attemptHandler = null;
            _enabled = true;
            _exceptionHandler = handler;
            _exceptionMapper = mapper;
            _rethrow = rethrow;
        }

        public ErrorHandler<TException> Attempt(Action<TException> attemptHandler)
        {
            _attemptHandler = attemptHandler;
            return this;
        }

        public ErrorHandler<TException> Catch(Action<TException> exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            return this;
        }

        public ErrorHandler<TException> Map(Func<TException, Exception> exceptionMapper)
        {
            _exceptionMapper = exceptionMapper;
            return this;
        }

        public ErrorHandler<TException> Enabled(bool enabled = true)
        {
            _enabled = enabled;
            return this;
        }

        public ErrorHandler<TException> Rethrow(bool rethrow = true)
        {
            _rethrow = rethrow;
            return this;
        }

        public ErrorHandler<TException> Try(int attempts = 1)
        {
            _attempts = attempts;
            return this;
        }

        public readonly void Action(Action action)
        {
            if (!_enabled) action();

            TException lastException = null;
            for (var counter = 0; counter < _attempts; counter++)
            {
                try
                {
                    action();
                }
                catch (TException e)
                {
                    Catch(e);
                    lastException = e;
                }
            }

            if (lastException != null) FinallyCatch(lastException);
        }

        public readonly void Action<T1>(Action<T1> action, T1 arg1)
        {
            if (!_enabled) action(arg1);

            TException lastException = null;
            for (var counter = 0; counter < _attempts; counter++)
            {
                try
                {
                    action(arg1);
                }
                catch (TException e)
                {
                    Catch(e);
                    lastException = e;
                }
            }

            if (lastException != null) FinallyCatch(lastException);
        }

        public readonly void Action<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (!_enabled) action(arg1, arg2);

            TException lastException = null;
            for (var counter = 0; counter < _attempts; counter++)
            {
                try
                {
                    action(arg1, arg2);
                }
                catch (TException e)
                {
                    Catch(e);
                    lastException = e;
                }
            }

            if (lastException != null) FinallyCatch(lastException);
        }

        public readonly TResult Return<TResult>(Func<TResult> function, TResult defaultResult = default)
        {
            if (!_enabled) return function();

            TException lastException = null;
            for (var counter = 0; counter < _attempts; counter++)
            {
                try
                {
                    return function();
                }
                catch (TException e)
                {
                    Catch(e);
                    lastException = e;
                }
            }

            if (lastException != null) FinallyCatch(lastException);

            return defaultResult;
        }

        public readonly TResult Return<T1, TResult>(Func<T1, TResult> function, T1 arg1,
            TResult defaultResult = default)
        {
            if (!_enabled) return function(arg1);

            TException lastException = null;
            for (var counter = 0; counter < _attempts; counter++)
            {
                try
                {
                    function(arg1);
                }
                catch (TException e)
                {
                    Catch(e);
                    lastException = e;
                }
            }

            if (lastException != null) FinallyCatch(lastException);

            return defaultResult;
        }

        public readonly TResult Return<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 arg1, T2 arg2,
            TResult defaultResult = default)
        {
            if (!_enabled) return function(arg1, arg2);

            TException lastException = null;
            for (var counter = 0; counter < _attempts; counter++)
            {
                try
                {
                    function(arg1, arg2);
                }
                catch (TException e)
                {
                    Catch(e);
                    lastException = e;
                }
            }

            if (lastException != null) FinallyCatch(lastException);

            return defaultResult;
        }

        private readonly void Catch(TException e)
        {
            _attemptHandler?.Invoke(e);
        }

        private readonly void FinallyCatch(TException e)
        {
            _exceptionHandler?.Invoke(e);

            if (_rethrow)
            {
                throw _exceptionMapper == null ? e : _exceptionMapper(e);
            }
        }
    }
}