namespace MCWorldConverter.Model
{
    internal static class BlockState
    {
        #region Block states (constants)
        /// <summary>int (0..15)</summary>
        internal const string rotation = nameof(rotation);
        /// <summary>string (down, up, north, south, west, east)</summary>
        internal const string facing = nameof(facing);
        /// <summary>bool (false, true)</summary>
        internal const string open = nameof(open);
        /// <summary>bool (false, true)</summary>
        internal const string occupied = nameof(occupied);
        /// <summary>string (foot, head)</summary>
        internal const string part = nameof(part);
        /// <summary>int (0..24)</summary>
        internal const string age = nameof(age);
        /// <summary>string (ceiling, double_wall, floor, single_wall)</summary>
        internal const string attachment = nameof(attachment);
        /// <summary>bool (false, true)</summary>
        internal const string lit = nameof(lit);
        /// <summary>string (x, y, z)</summary>
        internal const string axis = nameof(axis);
        /// <summary>bool (false, true)</summary>
        internal const string has_bottle_0 = nameof(has_bottle_0);
        /// <summary>bool (false, true)</summary>
        internal const string has_bottle_1 = nameof(has_bottle_1);
        /// <summary>bool (false, true)</summary>
        internal const string has_bottle_2 = nameof(has_bottle_2);
        /// <summary>bool (false, true)</summary>
        internal const string drag = nameof(drag);
        /// <summary>string (ceiling, floor, wall)</summary>
        internal const string face = nameof(face);
        /// <summary>bool (false, true)</summary>
        internal const string powered = nameof(powered);
        /// <summary>bool (false, true)</summary>
        internal const string signal_fire = nameof(signal_fire);
        /// <summary>bool (false, true)</summary>
        internal const string waterlogged = nameof(waterlogged);
        /// <summary>int (0..6)</summary>
        internal const string bites = nameof(bites);
        /// <summary>int (0..15)</summary>
        internal const string level = nameof(level);
        /// <summary>string ([chest] left, right, single, [piston] normal, sticky, [slab] bottom, top, double)</summary>
        internal const string type = nameof(type);
        /// <summary>bool (false, true)</summary>
        internal const string down = nameof(down);
        /// <summary>bool/string (false, true, [redstone] none, side, up)</summary>
        internal const string east = nameof(east);
        /// <summary>bool/string (false, true, [redstone] none, side, up)</summary>
        internal const string north = nameof(north);
        /// <summary>bool/string (false, true, [redstone] none, side, up)</summary>
        internal const string south = nameof(south);
        /// <summary>bool/string (false, true, [redstone] none, side, up)</summary>
        internal const string west = nameof(west);
        /// <summary>bool (false, true)</summary>
        internal const string up = nameof(up);
        /// <summary>bool (false, true)</summary>
        internal const string conditional = nameof(conditional);
        /// <summary>bool (false, true)</summary>
        internal const string inverted = nameof(inverted);
        /// <summary>int (0..15)</summary>
        internal const string power = nameof(power);
        /// <summary>bool (false, true)</summary>
        internal const string triggered = nameof(triggered);
        /// <summary>string ([doors] lower, upper, [stairs, trapdoors] bottom, top)</summary>
        internal const string half = nameof(half);
        /// <summary>string (left, right)</summary>
        internal const string hinge = nameof(hinge);
        /// <summary>bool (false, true)</summary>
        internal const string eye = nameof(eye);
        /// <summary>int (0..7)</summary>
        internal const string moisture = nameof(moisture);
        /// <summary>bool (false, true)</summary>
        internal const string in_wall = nameof(in_wall);
        /// <summary>bool (false, true)</summary>
        internal const string snowy = nameof(snowy);
        /// <summary>bool (false, true)</summary>
        internal const string has_record = nameof(has_record);
        /// <summary>bool (false, true)</summary>
        internal const string hanging = nameof(hanging);
        /// <summary>int (0..7)</summary>
        internal const string distance = nameof(distance);
        /// <summary>bool (false, true)</summary>
        internal const string persistent = nameof(persistent);
        /// <summary>bool (false, true)</summary>
        internal const string has_book = nameof(has_book);
        /// <summary>string (banjo, basedrum, bass, bell, bit, chime, cow_bell, didgeridoo, flute, guitar, harp, hat, iron_xylophone, pling, snare, xylophone)</summary>
        internal const string instrument = nameof(instrument);
        /// <summary>int (0..24)</summary>
        internal const string note = nameof(note);
        /// <summary>bool (false, true)</summary>
        internal const string extended = nameof(extended);
        /// <summary>bool (false, true)</summary>
        internal const string @short = nameof(@short);
        /// <summary>string ([rails] east_west, north_east, north_south, north_west, south_east, south_west, [stairs] inner_left, inner_right, outer_left, outer_right, straight)</summary>
        internal const string shape = nameof(shape);
        /// <summary>string ([comparator] compare, subtract, [structure block] corner, data, load, save)</summary>
        internal const string mode = nameof(mode);
        /// <summary>int (1..4)</summary>
        internal const string delay = nameof(delay);
        /// <summary>bool (false, true)</summary>
        internal const string locked = nameof(locked);
        /// <summary>int (0..1)</summary>
        internal const string stage = nameof(stage);
        /// <summary>bool (false, true)</summary>
        internal const string bottom = nameof(bottom);
        /// <summary>int (1..4)</summary>
        internal const string pickles = nameof(pickles);
        /// <summary>int (1..8)</summary>
        internal const string layers = nameof(layers);
        /// <summary>bool (false, true)</summary>
        internal const string unstable = nameof(unstable);
        /// <summary>bool (false, true)</summary>
        internal const string attached = nameof(attached);
        /// <summary>bool (false, true)</summary>
        internal const string disarmed = nameof(disarmed);
        /// <summary>int (1..4)</summary>
        internal const string eggs = nameof(eggs);
        /// <summary>int (0..2)</summary>
        internal const string hatch = nameof(hatch);
        /// <summary>bool (false, true)</summary>
        internal const string falling = nameof(falling);
        /// <summary>bool (false, true)</summary>
        internal const string enabled = nameof(enabled);
        #endregion
    }
}
