namespace MCWorldConverter
{
    internal sealed class InvalidConversionException : ConverterException
    {
        internal InvalidConversionException(DataVersion from, DataVersion to)
            : base($"Cannot convert from {from.S()} to {to.S()}.")
        { }

        internal InvalidConversionException(DataVersion v) : base($"Version is alerady {v.S()}.")
        { }

        internal InvalidConversionException() : base("Invalid conversion.")
        { }
    }
}
