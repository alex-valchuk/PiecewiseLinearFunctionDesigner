using System;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Exceptions
{
    public class InvalidFileTypeException : Exception
    {
        public InvalidFileTypeException(string message)
            : base(message)
        {
        }
    }
}