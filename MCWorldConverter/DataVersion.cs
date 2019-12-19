using System.Text.RegularExpressions;

namespace MCWorldConverter
{
    internal enum DataVersion : int
    {
        Unknown = 0,

        V8 = 47, //this is just a protocol version

        V9 = 169, //1.9 - introduction of data versions
        V9_1 = 175,
        V9_2 = 176,
        V9_3 = 183,
        V9_4 = 184,

        V10 = 510,
        V10_1 = 511,
        V10_2 = 512,

        V11 = 819,
        V11_1 = 921,
        V11_2 = 922,

        V12 = 1139,
        V12_1 = 1241,
        V12_2 = 1343,

        V13 = 1519,
        V13_1 = 1628,
        V13_2 = 1631,

        V14 = 1952,
        V14_1 = 1957,
        V14_2 = 1963,
        V14_3 = 1968,
        V14_4 = 1976,

        V15 = 2225,
        V15_1 = 2227,
    }

    internal static class DataVersionExtensions
    {
        internal static bool Is9(this DataVersion v) => ((int)v).IsIn((int)DataVersion.V9, (int)DataVersion.V9_4);
        internal static bool Is10(this DataVersion v) => ((int)v).IsIn((int)DataVersion.V10, (int)DataVersion.V10_2);
        internal static bool Is11(this DataVersion v) => ((int)v).IsIn((int)DataVersion.V11, (int)DataVersion.V11_2);
        internal static bool Is12(this DataVersion v) => ((int)v).IsIn((int)DataVersion.V12, (int)DataVersion.V12_2);
        internal static bool Is13(this DataVersion v) => ((int)v).IsIn((int)DataVersion.V13, (int)DataVersion.V13_2);
        internal static bool Is14(this DataVersion v) => ((int)v).IsIn((int)DataVersion.V14, (int)DataVersion.V14_4);

        internal static string S(this DataVersion v) => Regex.Replace(v.ToString().Replace("V", "1."), @"_\d", "");
    }
}
