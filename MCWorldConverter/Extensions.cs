using System;
using System.Collections.Generic;
using System.Linq;
using NBTExplorer.Model;

namespace MCWorldConverter
{
    internal static class Extensions
    {
        internal static N Expanded<N>(this N node)
            where N : DataNode
        {
            node.Expand();
            return node;
        }

        //private static IEnumerable<N> Expanded<N>(this IEnumerable<N> nodes)
        //    where N : DataNode
        //{
        //    return nodes.Select(node => node.Expanded());
        //}

        internal static IEnumerable<N> Only<N>(this IEnumerable<DataNode> nodes)
            where N : DataNode => nodes.Where(node => node is N).Cast<N>();

        internal static bool IsIn(this int i, int min, int max) => i >= min && i <= max;

        internal static T Get<T>(this DataNode tag, string name)
            where T : DataNode => tag.Expanded().Nodes.FirstOrDefault(node => node.NodeName == name) as T;

        internal static DataNode Get(this DataNode tag, string name) => tag.Get<DataNode>(name);

        internal static sbyte GetSByte(this TagDataNode.Container c, string name) => (sbyte)c.Get<TagByteDataNode>(name).Tag.ToTagByte().Data;
        internal static sbyte[] GetSByteArray(this TagDataNode.Container c, string name) => c.Get<TagByteArrayDataNode>(name).Tag.ToTagByteArray().Data.Select(b => unchecked((sbyte)b)).ToArray();
        internal static byte GetByte(this TagDataNode.Container c, string name) => c.Get<TagByteDataNode>(name).Tag.ToTagByte().Data;
        internal static byte[] GetByteArray(this TagDataNode.Container c, string name) => c.Get<TagByteArrayDataNode>(name).Tag.ToTagByteArray().Data;
        internal static double GetDouble(this TagDataNode.Container c, string name) => c.Get<TagDoubleDataNode>(name).Tag.ToTagByte().Data;
        internal static float GetFloat(this TagDataNode.Container c, string name) => c.Get<TagFloatDataNode>(name).Tag.ToTagFloat().Data;
        internal static int GetInt(this TagDataNode.Container c, string name) => c.Get<TagIntDataNode>(name).Tag.ToTagInt().Data;
        internal static int[] GetIntArray(this TagDataNode.Container c, string name) => c.Get<TagIntArrayDataNode>(name).Tag.ToTagIntArray().Data;
        internal static long GetLong(this TagDataNode.Container c, string name) => c.Get<TagLongDataNode>(name).Tag.ToTagLong().Data;
        internal static long[] GetLongArray(this TagDataNode.Container c, string name) => c.Get<TagLongArrayDataNode>(name).Tag.ToTagLongArray().Data;
        internal static short GetShort(this TagDataNode.Container c, string name) => c.Get<TagShortDataNode>(name).Tag.ToTagShort().Data;
        internal static short[] GetShortArray(this TagDataNode.Container c, string name) => c.Get<TagShortArrayDataNode>(name).Tag.ToTagShortArray().Data;
        internal static string GetString(this TagDataNode.Container c, string name) => c.Get<TagStringDataNode>(name).Tag.ToTagString().Data;

        internal static T As<T>(this object o) => o is T ? (T)o : default;
    }
}
