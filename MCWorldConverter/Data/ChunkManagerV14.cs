using System;
using System.Collections.Generic;
using System.Linq;
using NBTExplorer.Model;

namespace MCWorldConverter.Data
{
    internal sealed class ChunkManagerV14 : ChunkManager
    {
        internal ChunkManagerV14(RegionChunkDataNode chunk) : base(chunk) => Version = DataVersion.V14;

        internal Dictionary<int, List<Block>> Palettes { get; private set; }
        internal Dictionary<int, List<long>> BlockStates { get; private set; }
        internal Dictionary<int, int> SizeOfIndex { get; private set; }

        internal override Block GetBlockFromWorld(int x, int y, int z) => throw new NotImplementedException();
        internal override void Load()
        {
            var level = Node.Get("Level");
            IEnumerable<TagCompoundDataNode> sections = level.Get("Sections")
                                                             .Expanded().Nodes
                                                             .Cast<TagCompoundDataNode>()
                                                             .Where(tag => tag.GetSByte("Y") != -1);
            int idx = 0;
            Palettes = sections.ToDictionary(_ => idx++, section => section.Get<TagListDataNode>("Palette")
                                                                           .Expanded().Nodes
                                                                           .Cast<TagCompoundDataNode>()
                                                                           .Select(comp => new Block(comp))
                                                                           .ToList());
            idx = 0;
            BlockStates = sections.ToDictionary(_ => idx++, section => section.GetLongArray("BlockStates").ToList());
            SizeOfIndex = BlockStates.ToDictionary(pair => pair.Key, pair => 64 * pair.Value.Count / 4096);
        }

        internal override ChunkManagerV14 ToV14() => throw new InvalidConversionException(Version, Version);
        internal override ChunkManagerV8 ToV8()
        {
            var blocks = new List<OldBlock>();
            foreach (int chunkY in Palettes.Keys)
            {
                foreach (int blockIndex in IndicesFromLongList(BlockStates[chunkY], SizeOfIndex[chunkY]))
                {
                    blocks.Add(FlatteningHandler.ToOldBlock(Palettes[chunkY][blockIndex], Version));
                    //TODO put all the blocks into FlatteningHandler, output a byte array and a list of tile entities.
                }
            }
        }
    }
}
