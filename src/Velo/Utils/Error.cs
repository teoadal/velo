using System;
using System.Collections.Generic;

namespace Velo.Utils
{
    public static class Error
    {
        public static ArgumentException Exists(string message)
        {
            return new ArgumentException(message);
        }
        
        public static InvalidOperationException InvalidOperation(string message = null)
        {
            if (string.IsNullOrWhiteSpace(message)) message = "Invalid operation or unpredictable behavior";
            return new InvalidOperationException(message);
        }
        
        public static KeyNotFoundException NotFound(string message = null)
        {
            if (string.IsNullOrWhiteSpace(message)) message = "Element not found";
            return new KeyNotFoundException(message);
        }
    }
}