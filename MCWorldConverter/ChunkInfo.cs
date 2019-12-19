using System.Collections.Generic;
using NBTExplorer.Model;

namespace MCWorldConverter
{
    internal sealed class ChunkInfo
    {
        internal RegionChunkDataNode Node { get; set; }
        internal (int x, int z) Region { get; private set; }
        internal (int x, int z) ChunkInWorld { get; private set; }
        internal (int x, int z) ChunkInRegion { get; private set; }
        internal (int x, int z)? BlockInChunk { get; private set; }

        internal bool IsIn(ChunkInfo n, ChunkInfo p)
        {
            (int nx, int nz) = n.ChunkInWorld;
            (int px, int pz) = p.ChunkInWorld;
            (int x, int z) = ChunkInWorld;

            return x.IsIn(nx, px) && z.IsIn(nz, pz);
        }

        internal static ChunkInfo FromBlockInWorld(int x, int z)
        {
            return new ChunkInfo()
            {
                Region = (RegionFromBlock(x), RegionFromBlock(z)),
                ChunkInWorld = (ChunkFromBlock(x), ChunkFromBlock(z)),
                ChunkInRegion = (LocalChunkFromBlock(x), LocalChunkFromBlock(z)),
                BlockInChunk = (LocalBlockFromBlock(x), LocalBlockFromBlock(z))
            };
        }

        internal static ChunkInfo FromChunkInRegion(RegionChunkDataNode chunk)
        {
            if (!RegionFileDataNode.RegionCoordinates((chunk.Parent as RegionFileDataNode).NodePathName, out int rx, out int rz))
                throw new System.InvalidOperationException($"Region file name '{(chunk.Parent as RegionFileDataNode).NodePathName}' is invalid and cannot be parsed.");
            int x = chunk.X, z = chunk.Z;

            return new ChunkInfo()
            {
                Region = (rx, rz),
                ChunkInRegion = (x, z),
                ChunkInWorld = (ChunkFromLocalChunk(rx, x), ChunkFromLocalChunk(rz, z)),
                Node = chunk
            };
        }

        private static int Mod(int a, int b) => ((a % b) + b) % b;

        private static int RegionFromBlock(int block) => (block >> 4) >> 5;
        private static int ChunkFromBlock(int block) => block >> 4;
        private static int LocalChunkFromBlock(int block) => Mod(block >> 4, 32);
        private static int LocalBlockFromBlock(int block) => Mod(block, 16);

        private static int ChunkFromLocalChunk(int region, int localChunk) => region * 32 + localChunk;

        public static bool operator ==(ChunkInfo a, ChunkInfo b) => a.Equals(b);
        public static bool operator !=(ChunkInfo a, ChunkInfo b) => !a.Equals(b);
        public override bool Equals(object obj) => !(obj is null) && obj is ChunkInfo i
            && Region.x == i.Region.x && Region.z == i.Region.z
                && ChunkInWorld.x == i.ChunkInWorld.x && ChunkInWorld.z == i.ChunkInWorld.z
                && ChunkInRegion.x == i.ChunkInRegion.x && ChunkInRegion.z == i.ChunkInRegion.z
                && BlockInChunk?.x == i.BlockInChunk?.x && BlockInChunk?.z == i.BlockInChunk?.z;

        public override string ToString() => $"{nameof(Region)}=({Region.x}, {Region.z}), {nameof(ChunkInWorld)}=({ChunkInWorld.x}, {ChunkInWorld.z}), {nameof(ChunkInRegion)}=({ChunkInRegion.x}, {ChunkInRegion.z}), {nameof(BlockInChunk)}=({BlockInChunk?.x}, {BlockInChunk?.z})";

        public override int GetHashCode()
        {
            var hashCode = -575882153;
            hashCode = hashCode * -1521134295 + Region.GetHashCode();
            hashCode = hashCode * -1521134295 + ChunkInWorld.GetHashCode();
            hashCode = hashCode * -1521134295 + ChunkInRegion.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<(int x, int z)?>.Default.GetHashCode(BlockInChunk);
            return hashCode;
        }
    }
}
