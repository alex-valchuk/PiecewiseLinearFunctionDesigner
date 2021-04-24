using System;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Exceptions
{
    public class InvalidFileTypeException : Exception
    {
        public InvalidFileTypeException(string message, string supportedFileTypes)
            : base(message)
        {
            SupportedFileTypes = supportedFileTypes;
        }

        public string SupportedFileTypes { get; }
    }
}