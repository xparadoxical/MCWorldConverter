using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagLongDataNode : TagDataNode
    {
        public TagLongDataNode(TagNodeLong tag)
            : base(tag)
        { }

        protected new TagNodeLong Tag
        {
            get { return base.Tag as TagNodeLong; }
        }

        public override bool Parse(string value)
        {
            if (!long.TryParse(value, out long data))
                return false;

            Tag.Data = data;
            IsDataModified = true;

            return true;
        }

        public override bool EditNode()
        {
            return EditScalarValue(Tag);
        }
    }
}
