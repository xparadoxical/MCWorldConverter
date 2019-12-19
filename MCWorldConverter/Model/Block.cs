using System;
using System.Linq;
using System.Text.RegularExpressions;
using NBTExplorer.Model;

namespace MCWorldConverter.Model
{
    internal struct Block
    {
        internal string Namespace { get; }
        internal string Name { get; }
        internal string NamespacedID => Namespace + ":" + Name;
        internal BlockStateCollection State { get; }

        internal Block(string name, params (string key, string value)[] blockStates)
        {
            Match m = Regex.Match(name, @"([a-z_0-9]+)\:([a-z_]+)");
            if (!m.Success) throw new ArgumentException("Invalid ID.", nameof(name));

            Namespace = m.Groups[1].Value;
            Name = m.Groups[2].Value;
            State = new BlockStateCollection(blockStates);
        }

        internal Block(TagCompoundDataNode paletteEntry)
        {
            Match m = Regex.Match(paletteEntry.GetString("Name"), @"([a-z_0-9]+)\:([a-z_]+)");
            if (!m.Success) throw new ArgumentException("Invalid ID.", nameof(paletteEntry));

            Namespace = m.Groups[1].Value;
            Name = m.Groups[2].Value;
            State = new BlockStateCollection(paletteEntry.Get<TagCompoundDataNode>("Properties")
                                                               .Expanded().Nodes
                                                               .Cast<TagStringDataNode>()
                                                               .Select(str => (key: str.NodeName, value: str.Tag.ToTagString().Data))
                                                               .ToArray());
        }

        public override string ToString() => Namespace + ":" + Name + (State.Count > 0 ? State.ToString() : "");

        public static implicit operator Block(string name) => new Block(name);
        public static implicit operator Block((string name, (string, string)[] blockStates) a) => new Block(a.name, a.blockStates);
    }

    internal struct OldBlock
    {
        internal string Name { get; }
        internal uint ID { get; }
        internal uint Data { get; }

        internal OldBlock(string name, uint id, uint data = 0)
        {
            Name = name;
            ID = id;
            Data = data;
        }

        public override string ToString() => $"{Name} ({ID}) {Data}";

        public static implicit operator OldBlock((string name, uint id, uint data) a) => new OldBlock(a.name, a.id, a.data);
        public static implicit operator OldBlock((string name, uint id) a) => new OldBlock(a.name, a.id, 0);
    }
}
