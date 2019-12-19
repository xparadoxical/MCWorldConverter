using System;
using MCWorldConverter.Model;

namespace MCWorldConverter
{
    internal sealed class ConversionException : Exception
    {
        internal ConversionException(string msg, ref Block block, DataVersion v) : base($"{msg} Block: {block} Version: {v.S()}")
        { }
    }
}
