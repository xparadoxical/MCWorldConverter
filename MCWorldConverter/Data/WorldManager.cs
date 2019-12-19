using System;
using System.Collections.Generic;
using System.Linq;
using NBTExplorer.Model;

namespace MCWorldConverter.Data
{
    internal abstract class WorldManager
    {
        internal WorldManager(DirectoryDataNode worldDirectory, DataVersion version)
        {
            Node = worldDirectory;
            Version = version;
            RegionFiles = Node.Get("region").Expanded().Nodes.Cast<RegionFileDataNode>().ToList();
        }

        internal DataVersion Version { get; private protected set; }
        internal DirectoryDataNode Node { get; private protected set; }
        internal List<ChunkManager> ChunkManagers { get; private protected set; }
        internal List<RegionFileDataNode> RegionFiles { get; }

        private protected void LoadChunkManagers<M>()
            where M : ChunkManager => ChunkManagers = RegionFiles.SelectMany(region => region.Expanded().Nodes)
                                                                 .Cast<RegionChunkDataNode>()
                                                                 .Select(chunk => (M)Activator.CreateInstance(typeof(M), chunk))
                                                                 .Cast<ChunkManager>()
                                                                 .ToList();

        internal abstract WorldManagerV8 ToV8();
        internal abstract WorldManagerV14 ToV14();

        internal static WorldManager FromWorldDir(DirectoryDataNode worldDirectory)
        {
            var versionTag = worldDirectory.Get("level.dat")
                                           .Get("Data")
                                           .Get<TagCompoundDataNode>("Version");
            DataVersion v = versionTag is null ? (default) : (DataVersion)versionTag?.GetInt("Id");

            if (v.Is14()) return new WorldManagerV14(worldDirectory);
            else if (v == DataVersion.Unknown) return new WorldManagerV8(worldDirectory);

            throw new UnsupportedVersionException(v);
        }
    }
}
