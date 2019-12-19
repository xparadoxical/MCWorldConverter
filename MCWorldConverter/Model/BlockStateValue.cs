using System;
using System.Collections.Generic;

namespace MCWorldConverter.Model
{
    internal struct BlockStateValue
    {
        private object Value { get; }

        private BlockStateValue(string value) => Value = value;
        private BlockStateValue(int value) => Value = value;
        private BlockStateValue(bool value) => Value = value;

        public override bool Equals(object obj) => obj is BlockStateValue state && Value == state.Value;
        public override int GetHashCode() => -1937169414 + EqualityComparer<object>.Default.GetHashCode(Value);
        public override string ToString() => Value.ToString();

        public static implicit operator BlockStateValue(string a) => new BlockStateValue(a);
        public static implicit operator BlockStateValue(int a) => new BlockStateValue(a);
        public static implicit operator BlockStateValue(bool a) => new BlockStateValue(a);
        public static explicit operator string(BlockStateValue a) => (string)a.Value;
        public static explicit operator int(BlockStateValue a) => (int)a.Value;
        public static explicit operator uint(BlockStateValue a) => (uint)a.Value;
        public static implicit operator bool(BlockStateValue a) => (bool)a.Value;

        public static bool operator ==(BlockStateValue a, BlockStateValue b) => a.Equals(b);
        public static bool operator !=(BlockStateValue a, BlockStateValue b) => !a.Equals(b);

    }
}
