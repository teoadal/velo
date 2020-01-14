using System;

namespace Velo.Utils
{
    public ref struct Try<TException>
        where TException : Exception
    {
        private int _count;
        private bool _enabled;
        private Action<TException> _exceptionHandler;
        private bool _rethrow;

        public Try<TException> Catch(Action<TException> exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            return this;
        }

        public Try<TException> Count(int count)
        {
            _count = count;
            return this;
        }

        public Try<TException> Enabled(bool enabled = true)
        {
            _enabled = enabled;
            return this;
        }

        public Try<TException> Rethrow(bool rethrow = true)
        {
            _rethrow = rethrow;
            return this;
        }

        public void Run(Action action)
        {
            if (!_enabled) action();
            
            var counter = 0;
            while (counter <= _count)
            {
                counter++;

                try
                {
                    action();
                }
                catch (TException e)
                {
                    _exceptionHandler?.Invoke(e);
                    if (_rethrow) throw;
                }
            }
        }

        public void Run<T1>(Action<T1> action, T1 arg1)
        {
            if (!_enabled) action(arg1);
            
            var counter = 0;
            while (counter <= _count)
            {
                counter++;

                try
                {
                    action(arg1);
                }
                catch (TException e)
                {
                    _exceptionHandler?.Invoke(e);
                    if (_rethrow) throw;
                }
            }
        }
        
        public void Run<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (!_enabled) action(arg1, arg2);
            
            var counter = 0;
            while (counter <= _count)
            {
                counter++;

                try
                {
                    action(arg1, arg2);
                }
                catch (TException e)
                {
                    _exceptionHandler?.Invoke(e);
                    if (_rethrow) throw;
                }
            }
        }
        
        public TResult Run<T1, TResult>(Func<T1, TResult> function, T1 arg1, TResult defaultResult = default)
        {
            if (!_enabled) return function(arg1);

            var counter = 0;
            while (counter <= _count)
            {
                counter++;

                try
                {
                    return function(arg1);
                }
                catch (TException e)
                {
                    _exceptionHandler?.Invoke(e);
                    if (_rethrow) throw;
                }
            }

            return defaultResult;
        }

        public TResult Run<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 arg1, T2 arg2,
            TResult defaultResult = default)
        {
            if (!_enabled) return function(arg1, arg2);

            var counter = 0;
            while (counter <= _count)
            {
                counter++;

                try
                {
                    return function(arg1, arg2);
                }
                catch (TException e)
                {
                    _exceptionHandler?.Invoke(e);
                    if (_rethrow) throw;
                }
            }

            return defaultResult;
        }
    }
}