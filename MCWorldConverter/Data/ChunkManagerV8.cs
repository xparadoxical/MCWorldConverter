using System;
using NBTExplorer.Model;

namespace MCWorldConverter.Data
{
    internal sealed class ChunkManagerV8 : ChunkManager
    {
        public ChunkManagerV8(RegionChunkDataNode chunk) : base(chunk) => Version = DataVersion.V8;

        internal override Block GetBlockFromWorld(int x, int y, int z) => throw new NotImplementedException();
        internal override void Load() => throw new NotImplementedException();
        internal override ChunkManagerV14 ToV14() => throw new NotImplementedException();
        internal override ChunkManagerV8 ToV8() => throw new InvalidConversionException(Version, Version);
    }
}
