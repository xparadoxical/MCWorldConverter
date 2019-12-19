using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBTExplorer.Model;

namespace MCWorldConverter.Data
{
    internal abstract class ChunkManager
    {
        internal ChunkManager(RegionChunkDataNode chunk)
        {
            Node = chunk;
            Info = ChunkInfo.FromChunkInRegion(chunk);
        }

        internal RegionChunkDataNode Node { get; }
        internal DataVersion Version { get; private protected set; }
        internal ChunkInfo Info { get; }

        internal abstract void Load();
        internal abstract Block GetBlockFromWorld(int x, int y, int z);
        internal abstract ChunkManagerV8 ToV8();
        internal abstract ChunkManagerV14 ToV14();

        protected IEnumerable<int> IndicesFromLongList(IEnumerable<long> blockStates, int indexSize)
        {
            var bitsB = new StringBuilder();
            for (int i = blockStates.Count() - 1; i >= 0; i--)
            {
                bitsB.Append(Convert.ToString(blockStates.ElementAt(i), 2).PadLeft(64, '0'));
            }

            string bits = bitsB.ToString();

            for (int i = bits.Length - 1; i >= indexSize - 1; i -= indexSize)
            {
                yield return Convert.ToInt32(bits.Substring(i - 3, indexSize), 2);
            }
        }
    }
}
