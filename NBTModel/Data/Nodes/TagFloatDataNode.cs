using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagFloatDataNode : TagDataNode
    {
        public TagFloatDataNode(TagNodeFloat tag)
            : base(tag)
        { }

        protected new TagNodeFloat Tag
        {
            get { return base.Tag as TagNodeFloat; }
        }

        public override bool Parse(string value)
        {
            if (!float.TryParse(value, out float data))
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
