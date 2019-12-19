﻿using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagIntDataNode : TagDataNode
    {
        public TagIntDataNode(TagNodeInt tag)
            : base(tag)
        { }

        protected new TagNodeInt Tag
        {
            get { return base.Tag as TagNodeInt; }
        }

        public override bool Parse(string value)
        {
            if (!int.TryParse(value, out int data))
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
