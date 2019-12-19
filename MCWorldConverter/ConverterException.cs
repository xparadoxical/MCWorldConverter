using System;

namespace MCWorldConverter
{
    internal abstract class ConverterException : Exception
    {
        internal ConverterException(string msg) : base(msg)
        { }
    }
}
