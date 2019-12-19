using System;
using System.Collections.Generic;
using System.Linq;
using NBTExplorer.Model;

namespace MCWorldConverter.Data
{
    internal sealed class WorldManagerV8 : WorldManager
    {
        internal WorldManagerV8(DirectoryDataNode worldDirectory) : base(worldDirectory, DataVersion.V8) => LoadChunkManagers<ChunkManagerV8>();

        internal override WorldManagerV14 ToV14() => throw new NotImplementedException();
        internal override WorldManagerV8 ToV8() => throw new InvalidConversionException(Version, Version);
    }
}
