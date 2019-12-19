using NBTExplorer.Model;

namespace MCWorldConverter.Data
{
    internal class WorldManagerV14 : WorldManager
    {
        public WorldManagerV14(DirectoryDataNode worldDirectory) : base(worldDirectory, DataVersion.V14) => LoadChunkManagers<ChunkManagerV14>();

        internal override WorldManagerV14 ToV14() => throw new InvalidConversionException(Version, Version);
        internal override WorldManagerV8 ToV8() => throw new System.NotImplementedException();
    }
}