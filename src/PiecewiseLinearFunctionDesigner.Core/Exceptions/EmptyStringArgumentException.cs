using System;

namespace PiecewiseLinearFunctionDesigner.Core.Exceptions
{
    public class EmptyStringArgumentException : ArgumentException
    {
        public EmptyStringArgumentException(string argumentName)
            : base("Argument cannot be null, empty or whitespace", argumentName)
        {
        }
    }
}