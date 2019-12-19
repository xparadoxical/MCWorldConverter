namespace MCWorldConverter
{
    internal sealed class UnsupportedVersionException : ConverterException
    {
        internal UnsupportedVersionException(DataVersion v) : base($"Version {(int)v} is unsupported.")
        { }
    }
}
