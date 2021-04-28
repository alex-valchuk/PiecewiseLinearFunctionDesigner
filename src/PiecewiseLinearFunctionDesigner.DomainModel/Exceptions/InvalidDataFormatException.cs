using System;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Exceptions
{
    public class InvalidDataFormatException : Exception
    {
        public InvalidDataFormatException(string message)
            : base(message)
        {
        }
    }
}