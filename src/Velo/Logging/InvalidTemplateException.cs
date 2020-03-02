using System;

namespace Velo.Logging
{
    public class InvalidTemplateException : Exception
    {
        public InvalidTemplateException(string template, Exception inner)
            : base($"Template '{template}' is used elsewhere with other types of arguments or with other arguments", inner)
        {
        }
        
        public InvalidTemplateException(Exception inner)
            : base($"Template used elsewhere with other types of arguments or with other arguments", inner)
        {
        }
    }
}