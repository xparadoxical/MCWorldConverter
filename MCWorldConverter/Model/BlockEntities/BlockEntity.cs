using NBTExplorer.Model;

namespace MCWorldConverter.Model.BlockEntities
{
    internal abstract class BlockEntity
    {
        internal TagListDataNode NBT { get; set; }
        internal int X => NBT.GetInt("x");
        internal int Y => NBT.GetInt("y");
        internal int Z => NBT.GetInt("z");
        internal abstract string Id { get; }

        internal BlockEntity(TagCompoundDataNode nbt) => NBT = nbt.Get<TagListDataNode>("Items");
    }
}
