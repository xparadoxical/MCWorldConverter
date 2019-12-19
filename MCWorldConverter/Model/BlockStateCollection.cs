using System.Linq;
using System.Collections.Generic;

namespace MCWorldConverter.Model
{
    internal sealed class BlockStateCollection : Dictionary<string, BlockStateValue>
    {
        internal BlockStateCollection(params (string key, string value)[] blockStates) : base()
        {
            foreach ((string key, string value) in blockStates)
            {
                if (value == "false" || value == "true")
                    Add(key, bool.Parse(value));
                else if (int.TryParse(value, out int i))
                    Add(key, i);
                else
                    Add(key, value);
            }
        }

        public override string ToString() => "{" + string.Join(",", this.Select(pair => pair.Key + "=" + pair.Value)) + "}";
    }
}
