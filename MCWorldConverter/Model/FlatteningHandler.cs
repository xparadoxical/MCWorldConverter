using System;
using System.Linq;

#pragma warning disable CA1307 // Specify StringComparison
namespace MCWorldConverter.Model
{
    //Flattening: https://minecraft.gamepedia.com/Java_Edition_1.13/Flattening
    //Pre-flattening blocks: https://gamepedia.cursecdn.com/minecraft_gamepedia/8/8c/DataValuesBeta.png?version=7333f5c44a3eb559e6f9aa61b31bf765
    //DVs before flattening: https://minecraft.gamepedia.com/Java_Edition_data_values/Pre-flattening
    //DVs: https://minecraft.gamepedia.com/Java_Edition_data_values
    internal static class FlatteningHandler
    {
        internal static OldBlock ToOldBlock(this Block b, DataVersion targetVersion)
        {
            string prefix = null, suffix = null;
            bool acaciaOrDarkOak = false;
            bool trySplitName(string _suffix)
            {
                _suffix = '_' + _suffix;
                if (b.Name.EndsWith(_suffix))
                {
                    prefix = b.Name.Replace(_suffix, "");
                    suffix = _suffix;
                    acaciaOrDarkOak = new[] { "acacia", "dark_oak" }.Contains(prefix);
                    return true;
                }

                return false;
            }

            uint fluidDV() => (boolean(BlockState.falling) << 3) | (ushort)(-(int)b.State[BlockState.level] + 8);
            uint logDV(bool isBarkWood = false)
            {
                uint type = treeType(true);
                uint orientation = isBarkWood ? 0b1100u : (axis() << 2);
                return orientation | type;
            }
            uint leavesDV() => (b.State[BlockState.persistent] ? 0b0100u : 0) | treeType(true);
            uint bedDV()
            {
                uint facing = (string)b.State[BlockState.facing] switch
                {
                    "south" => 0b0000,
                    "west" => 0b0001u,
                    "north" => 0b0010u,
                    "east" => 0b0011u,
                    string s => throw new ConversionException($"Invalid direction {s}.", ref b, targetVersion)
                };
                uint occupied = b.State[BlockState.occupied] ? 0b0100u : 0;
                uint part = (string)b.State[BlockState.part] switch
                {
                    "foot" => 0b0000,
                    "head" => 0b1000u,
                    string s => throw new ConversionException($"Invalid bed part {s}.", ref b, targetVersion)
                };

                return part | occupied | facing;
            }
            uint railDV()
            {
                return (string)b.State[BlockState.shape] switch
                {
                    "north_south" => 0u,
                    "east_west" => 1,
                    "ascending_east" => 2,
                    "ascending_west" => 3,
                    "ascending_north" => 4,
                    "ascending_south" => 5,
                    "south_east" => 6,
                    "south_west" => 7,
                    "north_west" => 8,
                    "north_east" => 9,
                    string s => throw new ConversionException($"Invalid rail shape {s}.", ref b, targetVersion)
                };
            }
            uint redstoneRailDV()
            {
                uint shape = (string)b.State[BlockState.shape] switch
                {
                    "north_south" => 0b0000,
                    "east_west" => 0b0001u,
                    "ascending_east" => 0b0010u,
                    "ascending_west" => 0b0011u,
                    "ascending_north" => 0b0100u,
                    "ascending_south" => 0b0101u,
                    string s => throw new ConversionException($"Invalid rail shape {s}.", ref b, targetVersion)
                };
                uint active = boolean(BlockState.powered) << 3;
                return active | shape;
            }
            uint pistonDV() => (boolean(BlockState.extended) << 3) | facing();
            uint pistonHeadDV()
            {
                uint type = (string)b.State[BlockState.type] switch
                {
                    "normal" => 0b0000,
                    "sticky" => 0b1000u,
                    string s => throw new ConversionException($"Invalid piston type {s}.", ref b, targetVersion)
                };
                return type | facing();
            }
            uint stairsDV()
            {
                uint direction = 6 - facing() - 1;
                return ((string)b.State[BlockState.half] == "top" ? 0b0100u : 0u) | direction;
            }
            uint doorDV()
            {
                if (b.State[BlockState.half] == "lower")
                {
                    uint facing = (string)b.State[BlockState.facing] switch
                    {
                        "east" => 0b0000u,
                        "south" => 0b0001,
                        "west" => 0b0010,
                        "north" => 0b0011,
                        string s => throw new ConversionException($"Invalid door facing value {s}.", ref b, targetVersion)
                    };
                    uint open = boolean(BlockState.open) << 2;
                    return open | facing;
                }
                else //half=upper
                {
                    uint hinge = (string)b.State[BlockState.hinge] == "left" ? 0u : 1u;
                    uint powered = boolean(BlockState.powered) << 1;
                    return 0b1000u | powered | hinge;
                }
            }
            uint leverDV()
            {
                uint powered = boolean(BlockState.powered) << 3;
                string face = (string)b.State[BlockState.face];
                string facing = (string)b.State[BlockState.facing];

                switch ((face, facing))
                {
                    case ("floor", "north"):
                    case ("floor", "south"):
                        return powered | 5;
                    case ("floor", "east"):
                    case ("floor", "west"):
                        return powered | 6;
                    case ("wall", "north"):
                        return powered | 4;
                    case ("wall", "south"):
                        return powered | 3;
                    case ("wall", "east"):
                        return powered | 1;
                    case ("wall", "west"):
                        return powered | 2;
                    case ("ceiling", "north"):
                    case ("ceiling", "south"):
                        return powered;
                    case ("ceiling", "east"):
                    case ("ceiling", "west"):
                        return powered | 7;
                    default:
                        throw new ConversionException("excuse me what the fuck", ref b, targetVersion);
                }
            }
            uint buttonDV() => (boolean(BlockState.powered) << 3) | ((string)b.State[BlockState.facing] == "down" ? 0u : 6u - facing());
            uint pumpkinDV()
            {
                return (string)b.State[BlockState.facing] switch
                {
                    "south" => 0u,
                    "west" => 1,
                    "north" => 2,
                    "east" => 3,
                    _ => throw new ConversionException("wut", ref b, targetVersion)
                };
            }
            uint repeaterDV()
            {
                uint facing = (string)b.State[BlockState.facing] switch
                {
                    "north" => 0u,
                    "east" => 1,
                    "south" => 2,
                    "west" => 3,
                    _ => throw new ConversionException("why", ref b, targetVersion)
                };
                return (((uint)b.State[BlockState.delay] - 1) << 2) | facing;
            }
            uint trapdoorDV()
            {
                uint facing = (string)b.State[BlockState.facing] switch
                {
                    "south" => 0u,
                    "north" => 1,
                    "east" => 2,
                    "west" => 3,
                    _ => throw new ConversionException("E", ref b, targetVersion)
                };
                return (b.State[BlockState.half] == "top" ? 0b1000u : 0) | (boolean(BlockState.open) << 2) | facing;
            }
            uint mushroomBlockDV(bool stem = false)
            {
                const uint north = 0b100000u,
                           south = 0b010000u,
                           down  = 0b001000u,
                           up    = 0b000100u,
                           west  = 0b000010u,
                           east  = 0b000001u;

                uint flags = (b.State[BlockState.north] ? north : 0) |
                             (b.State[BlockState.south] ? south : 0) |
                             (b.State[BlockState.down]  ? down : 0) |
                             (b.State[BlockState.up]    ? up : 0) |
                             (b.State[BlockState.west]  ? west : 0) |
                             (b.State[BlockState.east]  ? east : 0);

                if (!stem)
                {
                    return flags switch
                    {
                        up | west | north => 1,
                        up | north => 2,
                        up | north | east => 3,
                        up | west => 4,
                        up => 5,
                        up | east => 6,
                        up | south | west => 7,
                        up | south => 8,
                        up | east | south => 9,
                        0b111111u => 14,
                        _ => 0
                    };
                }
                else
                {
                    return flags switch
                    {
                        west | east | north | south => 10,
                        0b111111u => 15,
                        _ => 0
                    };
                }
            }
            uint fenceGateDV()
            {
                uint facing = (string)b.State[BlockState.facing] switch
                {
                    "south" => 0,
                    "west" => 1,
                    "north" => 2,
                    "east" => 3,
                    _ => throw new ConversionException("Invalid facing.", ref b, targetVersion)
                };
                return (boolean(BlockState.open) << 2) | facing;
            }
            uint endPortalFrameDV()
            {
                uint facing = (string)b.State[BlockState.facing] switch
                {
                    "south" => 0,
                    "west" => 1,
                    "north" => 2,
                    "east" => 3,
                    _ => throw new ConversionException("Invalid facing.", ref b, targetVersion)
                };
                return (boolean(BlockState.eye) << 3) | facing;
            }
            uint cocoaDV()
            {
                uint facing = (string)b.State[BlockState.facing] switch
                {
                    "north" => 0,
                    "east" => 1,
                    "south" => 2,
                    "west" => 3,
                    _ => throw new ConversionException("Invalid facing.", ref b, targetVersion)
                };
                return ((uint)b.State[BlockState.age] << 2) | facing;
            }
            uint tripwireHookDV()
            {
                uint facing = (string)b.State[BlockState.facing] switch
                {
                    "south" => 0,
                    "west" => 1,
                    "north" => 2,
                    "east" => 3,
                    _ => throw new ConversionException("Invalid facing.", ref b, targetVersion)
                };
                return (boolean(BlockState.powered) << 3) | (boolean(BlockState.attached) << 2) | facing;
            }
            uint comparatorDV()
            {
                uint facing = (string)b.State[BlockState.facing] switch
                {
                    "north" => 0,
                    "east" => 1,
                    "south" => 2,
                    "west" => 3,
                    _ => throw new ConversionException("Invalid facing.", ref b, targetVersion)
                };
                uint subtraction = (string)b.State[BlockState.mode] == "subtract" ? 1u: 0u;
                return (boolean(BlockState.powered) << 3) | (subtraction << 2) | facing;
            }
            uint structureBlockDV()
            {
                return (string)b.State[BlockState.mode] switch
                {
                    "save" => 0,
                    "load" => 1,
                    "corner" => 2,
                    "data" => 3,
                    _ => throw new ConversionException("Invalid structure block mode.", ref b, targetVersion)
                };
            }

            uint treeType(bool separateOldFromNew = false)
            {
                return prefix switch
                {
                    "oak" => 0b0000,
                    "spruce" => 0b0001,
                    "birch" => 0b0010,
                    "jungle" => 0b0011,
                    "acacia" => separateOldFromNew ? 0b0000 : 0b0100u,
                    "dark_oak" => separateOldFromNew ? 0b0001 : 0b0101u,
                    _ => throw new ConversionException($"Invalid tree type {prefix}.", ref b, targetVersion)
                };
            }
            uint stoneSlabType()
            {
                return prefix switch
                {
                    "smooth_stone" => 0u,
                    "sandstone" => 1,
                    "petrified_oak" => 2,
                    "cobblestone" => 3,
                    "brick" => 4,
                    "stone_brick" => 5,
                    "nether_brick" => 6,
                    "quartz" => 7,
                    string s => throw new ConversionException($"Invalid stone slab type {s}.", ref b, targetVersion)
                };
            }
            uint facing()
            {
                return (string)b.State[BlockState.facing] switch
                {
                    "down" => 0b0000,
                    "up" => 0b0001,
                    "north" => 0b0010,
                    "south" => 0b0011,
                    "west" => 0b0100,
                    "east" => 0b0101,
                    string s => throw new ConversionException($"Invalid direction {s}.", ref b, targetVersion)
                };
            }
            uint color()
            {
                return prefix switch
                {
                    "white" => 0,
                    "orange" => 1,
                    "magenta" => 2,
                    "light_blue" => 3,
                    "yellow" => 4,
                    "lime" => 5,
                    "pink" => 6,
                    "gray" => 7,
                    "light_gray" => 8,
                    "cyan" => 9,
                    "purple" => 10,
                    "blue" => 11,
                    "brown" => 12,
                    "green" => 13,
                    "red" => 14,
                    "black" => 15,
                    string s => throw new ConversionException($"Invalid color {s}.", ref b, targetVersion)
                };
            }
            uint stoneBrickType()
            {
                return prefix switch
                {
                    "stone" => 0u,
                    "mossy_stone" => 1,
                    "cracked_stone" => 2,
                    "chiseled_stone" => 3,
                    _ => throw new ConversionException("no", ref b, targetVersion)
                };
            }
            uint axis()
            {
                return (string)b.State[BlockState.axis] switch
                {
                    "y" => 0u,
                    "x" => 1u,
                    "z" => 2u,
                    string s => throw new ConversionException($"Invalid block state {BlockState.axis}={s}.", ref b, targetVersion)
                };
            }

            uint boolean(string blockState) => b.State[blockState] ? 1u : 0;

            OldBlock same(uint id) => (b.Name, id);

            OldBlock warnUnknownBlock()
            {
                Console.WriteLine($"Block unhandled by FlatteningHandler: {b}");
                return ("air", 0);
            }

            //TODO make it versioned, e.g. some blocks should be replaced with air if the target version is too low
            switch (b.Name)
            {
                //the same block with different DVs <- different blocks with the same suffix
                case string _ when trySplitName("planks"):
                    return ("planks", 5, treeType());
                case string _ when trySplitName("sapling"):
                    return ("sapling", 6, treeType());
                case string _ when trySplitName("log"):
                case string _ when trySplitName("wood"):
                    return (acaciaOrDarkOak ? "log2" : "log", acaciaOrDarkOak ? 162u : 17u, logDV(suffix == "wood"));
                case string _ when trySplitName("leaves"):
                    return (acaciaOrDarkOak ? "leaves2" : "leaves", acaciaOrDarkOak ? 161u : 18u, leavesDV());
                case string _ when trySplitName("bed"):
                    return ("bed", 26, bedDV());
                case string _ when trySplitName("wool"):
                    return ("wool", 35, color());
                #region Slabs
                case string _ when trySplitName("slab") && new[] { "oak", "spruce", "birch", "jungle", "acacia", "dark_oak" }.Contains(prefix):
                    string type = (string)b.State[BlockState.type];
                    bool dbl = type == "double";
                    return (dbl ? "double_wooden_slab" : "wooden_slab", dbl ? 125u : 126u, treeType() | (type == "top" ? 0b1000u : 0u));
                case "smooth_stone":
                    return ("double_stone_slab", 43, 8);
                case "smooth_sandstone_slab":
                    return ("double_stone_slab", 43, 9);
                case "smooth_quartz_slab":
                    return ("double_stone_slab", 43, 15);
                case "red_sandstone_slab":
                    type = (string)b.State[BlockState.type];
                    dbl = type == "double";
                    return (dbl ? "double_stone_slab2" : "stone_slab2", dbl ? 181u : 182u, type == "top" ? 0b1000u : 0u);
                case "smooth_red_sandstone_slab":
                    type = (string)b.State[BlockState.type];
                    dbl = type == "double";
                    return (dbl ? "double_stone_slab2" : "stone_slab2", dbl ? 181u : 182u, dbl || type == "top" ? 0b1000u : 0u);
                case string _ when trySplitName("slab") && prefix == "purpur":
                    type = (string)b.State[BlockState.type];
                    dbl = type == "double";
                    return (dbl ? "double_purpur_slab" : "purpur_slab", dbl ? 204u : 205u, type == "top" ? 0b1000u : 0u);
                case "prismarine_slab":
                case "prismarine_brick_slab":
                case "dark_prismarine_slab":
                    return ("air", 0); //TODO check on 1.12
                case string _ when trySplitName("slab") && !new[] { "prismarine", "prismarine_brick", "dark_prismarine" }.Contains(prefix):
                    type = (string)b.State[BlockState.type];
                    dbl = type == "double";
                    return (dbl ? "double_stone_slab" : "stone_slab", dbl ? 43u : 44u, stoneSlabType() | (type == "top" ? 0b1000u : 0u));
                case string s when s.EndsWith("_wall_sign"):
                    return ("wall_sign", 68, facing());
                case string s when s.EndsWith("_sign"):
                    return ("sign", 63, (uint)b.State[BlockState.rotation]);
                #endregion
                case string s when trySplitName("door"):
                    if (prefix == "oak")
                        return ("wooden_door", 64, doorDV());
                    else if (prefix == "iron")
                        return ("iron_door", 71, doorDV());
                    else
                        return (s, 192 + treeType(), doorDV());
                case string s when trySplitName("pressure_plate"):
                    if (prefix.Contains("weighted"))
                        return (s, prefix.StartsWith("heavy") ? 148u : 147u, (uint)b.State[BlockState.power]);
                    else if (prefix == "stone")
                        return (s, 70, boolean(BlockState.powered));
                    else
                        return ("wooden_pressure_plate", 72, boolean(BlockState.powered));
                case string _ when trySplitName("torch") && prefix.StartsWith("redstone"):
                    uint dv = prefix.EndsWith("wall") ? 6u - facing() : 5u;
                    if (b.State[BlockState.lit])
                        return ("redstone_torch", 76, dv);
                    else
                        return ("unlit_redstone_torch", 75, dv);
                case string s when trySplitName("fence"):
                    if (prefix == "oak")
                        return ("fence", 85);
                    else
                        return (s, 187 + treeType());
                case "pumpkin":
                case "carved_pumpkin":
                    return ("pumpkin", 86, pumpkinDV());
                case string _ when trySplitName("stained_glass"):
                    return ("stained_glass", 95, color());
                case string _ when trySplitName("trapdoor"):
                    if (prefix == "iron")
                        return ("iron_trapdoor", 167, trapdoorDV());
                    else
                        return ("trapdoor", 96, trapdoorDV());
                case string s when s.StartsWith("infested_"):
                    s = s.Remove(0, 9); //remove "infested_"
                    if (s.EndsWith("stone"))
                        return ("monster_egg", 97, s[0] == 'c' ? 1u /*cobblestone*/ : 0u /*stone*/);
                    else
                        return ("monster_egg", 97, 2 + stoneBrickType());
                case string _ when trySplitName("bricks"):
                    return ("stonebrick", 98, stoneBrickType());
                case string _ when trySplitName("stem"):
                    if (prefix.Replace("attached_", "") == "pumpkin")
                        return ("pumpkin_stem", 104, (uint)b.State[BlockState.age]);
                    else
                        return ("melon_stem", 105, (uint)b.State[BlockState.age]);
                case string s when trySplitName("fence_gate"):
                    if (prefix == "oak")
                        return ("fence_gate", 107, fenceGateDV());
                    else
                        return (s, 182 + treeType());
                case string s when s.StartsWith("potted_"):
                    //TODO insert the flower type into a TileEntity
                    return ("flower_pot", 140);
                case string s when s.EndsWith("_button") && !s.StartsWith("stone_"):
                    return ("wooden_button", 143, buttonDV());
                case string _ when trySplitName("skull"):
                    //TODO insert the skull type/texture data into a TileEntity
                    if (prefix.EndsWith("_wall"))
                        return ("skull", 144, facing());
                    else
                        //TODO insert the rotation of a skull into a TileEntity
                        return ("skull", 144, 1);
                case string s when b.Name.EndsWith("anvil"):
                    return ("anvil", 145, s.StartsWith("damaged") ? 2u : s.StartsWith("chipped") ? 1u : 0u);
                case string _ when trySplitName("terracotta"):
                    return ("stained_hardened_clay", 159, color());
                case string _ when trySplitName("stained_glass_pane"):
                    return ("stained_glass_pane", 160, color());
                case string _ when trySplitName("carpet"):
                    return ("carpet", 171, color());
                case string _ when trySplitName("banner"):
                    bool wall = prefix.EndsWith("_wall");
                    if (wall)
                    {
                        prefix = prefix.Replace("_wall", "");
                        dv = (uint)b.State[BlockState.rotation];
                    }
                    else
                        dv = facing();
                    //TODO insert the banner color into a TileEntity
                    return (wall ? "wall_banner" : "standing_banner", wall ? 177u : 176u, dv);
                case string _ when trySplitName("concrete"):
                    return ("concrete", 251, color());
                case string _ when trySplitName("concrete_powder"):
                    return ("concrete", 252, color());
                default:
                    //one block with different DVs <- split <- multiple blocks
                    //OR oldBlock <- rename <- newBlock
                    return b.Name switch
                    {
                        "stone" => same(1),
                        "granite" => ("stone", 1, 1),
                        "polished_granite" => ("stone", 1, 2),
                        "diorite" => ("stone", 1, 3),
                        "polished_diorite" => ("stone", 1, 4),
                        "andesite" => ("stone", 1, 5),
                        "polished_andesite" => ("stone", 1, 6),

                        "grass_block" => ("grass", 2),

                        "dirt" => same(3),
                        "coarse_dirt" => ("dirt", 3, 1),
                        "podzol" => ("dirt", 3, 2),

                        "cobblestone" => same(4),

                        "bedrock" => same(7),

                        "water" => same(8),
                        "flowing_water" => ("water", 8, fluidDV()),
                        "lava" => same(10),
                        "flowing_lava" => ("lava", 10, fluidDV()),

                        "sand" => same(12),
                        "red_sand" => ("sand", 12, 1),

                        "gravel" => same(13),
                        "gold_ore" => same(14),
                        "iron_ore" => same(15),
                        "coal_ore" => same(16),

                        "sponge" => same(19),
                        "wet_sponge" => ("sponge", 19, 1),

                        "glass" => same(20),
                        "lapis_ore" => same(21),
                        "lapis_block" => same(22),

                        "dispenser" => ("dispenser", 23, (boolean(BlockState.triggered) << 3) | facing()),

                        "sandstone" => same(24),
                        "chiseled_sandstone" => ("sandstone", 24, 1),
                        "cut_sandstone" => ("sandstone", 24, 2),

                        "note_block" => ("noteblock", 25), //TODO set the 'note' tag to a 'note' blockstate value

                        "powered_rail" => ("golden_rail", 27, redstoneRailDV()),
                        "detector_rail" => ("detector_rail", 28, redstoneRailDV()),

                        "sticky_piston" => ("piston", 29, pistonDV()),
                        "cobweb" => ("web", 30),
                        "grass" => ("tallgrass", 31, 1),
                        "fern" => ("tallgrass", 31, 2),
                        "dead_bush" => ("deadbush", 32),

                        "piston" => ("piston", 33, pistonDV()),
                        "piston_head" => ("piston_head", 34, pistonHeadDV()),

                        "dandelion" => ("yellow_flower", 37),
                        "poppy" => ("red_flower", 38, 0),
                        "blue_orchid" => ("red_flower", 38, 1),
                        "allium" => ("red_flower", 38, 2),
                        "azure_bluet" => ("red_flower", 38, 3),
                        "red_tulip" => ("red_flower", 38, 4),
                        "orange_tulip" => ("red_flower", 38, 5),
                        "white_tulip" => ("red_flower", 38, 6),
                        "pink_tulip" => ("red_flower", 38, 7),
                        "oxeye_daisy" => ("red_flower", 38, 8),
                        "brown_mushroom" => same(39),
                        "red_mushroom" => same(40),
                        "gold_block" => same(41),
                        "iron_block" => same(42),

                        "bricks" => ("brick_block", 45),
                        "tnt" => same(46),
                        "bookshelf" => same(47),
                        "mossy_cobblestone" => same(48),
                        "obsidian" => same(49),
                        "torch" => ("torch", 50, 5),
                        "wall_torch" => ("torch", 50, 6u - facing()),
                        "fire" => ("fire", 51, (uint)b.State[BlockState.age]),
                        "spawner" => ("mob_spawner", 52),
                        "oak_stairs" => ("oak_stairs", 53, stairsDV()),
                        "chest" => ("chest", 54, facing()),
                        "redstone_wire" => ("redstone_wire", 55u, (uint)b.State[BlockState.power]),
                        "diamond_ore" => same(56),
                        "diamond_block" => same(57),
                        "crafting_table" => same(58),
                        "wheat" => ("wheat", 59, (uint)b.State[BlockState.age]),
                        "farmland" => ("farmland", 60, (uint)b.State[BlockState.moisture]),
                        "furnace" => b.State[BlockState.lit] ? ("lit_furnace", 62u, facing()) : ("furnace", 61u, facing()),

                        "ladder" => ("ladder", 65, facing()),
                        "rail" => ("rail", 66, railDV()),
                        "cobblestone_stairs" => ("stone_stairs", 67, stairsDV()),

                        "lever" => ("lever", 69, leverDV()),

                        "redstone_ore" => b.State[BlockState.lit] ? ("lit_redstone_ore", 74u) : ("redstone_ore", 73u),

                        "stone_button" => ("stone_button", 77, buttonDV()),
                        "snow" => ("snow_layer", 78, (uint)b.State[BlockState.layers] - 1),
                        "ice" => same(79),
                        "snow_block" => ("snow", 80),
                        "cactus" => ("cactus", 81, (uint)b.State[BlockState.age]),
                        "clay" => same(82),
                        "sugar_cane" => ("reeds", 83, (uint)b.State[BlockState.age]),
                        "jukebox" => ("jukebox", 84, boolean(BlockState.has_record)),

                        "netherrack" => same(87),
                        "soul_sand" => same(88),
                        "glowstone" => same(89),
                        "nether_portal" => ("portal", 90, (string)b.State[BlockState.axis] == "x" ? 0b0001u : 0b0010u),
                        "jack_o_lantern" => ("lit_pumpkin", 91, pumpkinDV()),
                        "cake" => ("cake", 92, (uint)b.State[BlockState.bites]),
                        "repeater" => b.State[BlockState.powered] ? ("powered_repeater", 94u, repeaterDV()) : ("unpowered_repeater", 93u, repeaterDV()),

                        "brown_mushroom_block" => ("brown_mushroom_block", 99, mushroomBlockDV()),
                        "mushroom_stem" => ("brown_mushroom_block", 99, mushroomBlockDV(true)),
                        "red_mushroom_block" => ("red_mushroom_block", 100, mushroomBlockDV()),
                        "iron_bars" => same(101),
                        "glass_pane" => same(102),
                        "melon" => ("melon_block", 103),

                        "vine" => ("vine", 106, (boolean(BlockState.east) << 3) | (boolean(BlockState.north) << 2) | (boolean(BlockState.west) << 1) | boolean(BlockState.south)),

                        "brick_stairs" => (b.Name, 108, stairsDV()),
                        "stone_brick_stairs" => (b.Name, 109, stairsDV()),
                        "mycelium" => (b.Name, 110, stairsDV()),
                        "lily_pad" => ("waterlily", 111),
                        "nether_bricks" => ("nether_brick", 112),
                        "nether_brick_fence" => same(113),
                        "nether_brick_stairs" => (b.Name, 114, stairsDV()),
                        "nether_wart" => (b.Name, 115, (uint)b.State[BlockState.age]),
                        "enchanting_table" => (b.Name, 116),
                        "brewing_stand" => (b.Name, 117, (boolean(BlockState.has_bottle_0) << 2) | (boolean(BlockState.has_bottle_1) << 1) | boolean(BlockState.has_bottle_2)),
                        "cauldron" => (b.Name, 118, (uint)b.State[BlockState.level]),
                        "end_portal" => same(119),
                        "end_portal_frame" => (b.Name, 120, endPortalFrameDV()),
                        "end_stone" => same(121),
                        "dragon_egg" => same(122),
                        "redstone_lamp" => b.State[BlockState.lit] ? ("lit_redstone_lamp", 124u) : ("redstone_lamp", 123u),

                        "cocoa" => (b.Name, 127, cocoaDV()),
                        "sandstone_stairs" => (b.Name, 128, stairsDV()),
                        "emerald_ore" => same(129),
                        "ender_chest" => (b.Name, 130, facing()),
                        "tripwire_hook" => (b.Name, 131, tripwireHookDV()),
                        "tripwire" => (b.Name, 132, (boolean(BlockState.disarmed) << 3) | (boolean(BlockState.attached) << 2) | boolean(BlockState.powered)),
                        "emerald_block" => same(133),
                        "spruce_stairs" => (b.Name, 134, stairsDV()),
                        "birch_stairs" => (b.Name, 135, stairsDV()),
                        "jungle_stairs" => (b.Name, 136, stairsDV()),
                        "command_block" => (b.Name, 137, (boolean(BlockState.conditional) << 3) | facing()),
                        "beacon" => same(138),
                        "cobblestone_wall" => same(139),
                        "mossy_cobblestone_wall" => ("cobblestone_wall", 139, 1),
                        "flower_pot" => same(140),

                        "carrots" => (b.Name, 141, (uint)b.State[BlockState.age]),
                        "potatoes" => (b.Name, 142, (uint)b.State[BlockState.age]),

                        "trapped_chest" => (b.Name, 146, facing()),
                        "light_weighted_pressure_plate" => (b.Name, 147, (uint)b.State[BlockState.power]),
                        "heavy_weighted_pressure_plate" => (b.Name, 148, (uint)b.State[BlockState.power]),
                        "comparator" => b.State[BlockState.powered] ? ("powered_comparator", 150u, comparatorDV()) : ("unpowered_comparator", 149u, comparatorDV()),
                        "daylight_detector" => b.State[BlockState.inverted] ? ("daylight_detector_inverted", 178u, (uint)b.State[BlockState.power]) : ("daylight_detector", 151u, (uint)b.State[BlockState.power]),
                        "redstone_block" => same(152),
                        "quartz_ore" => same(153),
                        "hopper" => (b.Name, 154, (boolean(BlockState.enabled) << 3) | facing()),
                        "quartz_block" => same(155),
                        "chiseled_quartz_block" => ("quartz_block", 155, 1),
                        "quartz_pillar" => ("quartz_block", 155, 2u + axis()),
                        "quartz_stairs" => (b.Name, 156, stairsDV()),
                        "activator_rail" => (b.Name, 157, redstoneRailDV()),
                        "dropper" => (b.Name, 158, (boolean(BlockState.triggered) << 3) | facing()),

                        "acacia_stairs" => (b.Name, 163, stairsDV()),
                        "dark_oak_stairs" => (b.Name, 164, stairsDV()),
                        "slime_block" => same(165),
                        "barrier" => same(166),

                        "prismarine" => same(168),
                        "prismarine_bricks" => ("prismarine", 168, 1),
                        "dark_prismarine" => ("prismarine", 168, 2),
                        "sea_lantern" => same(169),
                        "hay_block" => (b.Name, 170, axis() << 2),

                        "terracotta" => ("hardened_clay", 172),
                        "coal_block" => same(173),
                        "packed_ice" => same(174),
                        "sunflower" => ("double_plant", 175, b.State[BlockState.half] == "upper" ? 8u : 0u),
                        "lilac" => ("double_plant", 175, 1 | (b.State[BlockState.half] == "upper" ? 8u : 0u)),
                        "tall_grass" => ("double_plant", 175, 2 | (b.State[BlockState.half] == "upper" ? 8u : 0u)),
                        "large_fern" => ("double_plant", 175, 3 | (b.State[BlockState.half] == "upper" ? 8u : 0u)),
                        "rose_bush" => ("double_plant", 175, 4 | (b.State[BlockState.half] == "upper" ? 8u : 0u)),
                        "peony" => ("double_plant", 175, 5 | (b.State[BlockState.half] == "upper" ? 8u : 0u)),

                        "red_sandstone" => same(179),
                        "chiseled_red_sandstone" => ("red_sandstone", 179, 1),
                        "cut_red_sandstone" => ("red_sandstone", 179, 2),
                        
                        "end_rod" => (b.Name, 198, facing()),
                        "chorus_plant" => (b.Name, 199, 0 /*TODO sides?*/),
                        "chorus_fruit" => (b.Name, 200, (uint)b.State[BlockState.age]),
                        "purpur_block" => same(201),
                        "purpur_pillar" => (b.Name, 202, 0 /*TODO axis?*/),
                        "purpur_stairs" => (b.Name, 203, stairsDV()),

                        "end_stone_bricks" => ("end_bricks", 206),
                        "beetroots" => (b.Name, 207, (uint)b.State[BlockState.age]),
                        "grass_path" => same(208),
                        "end_gateway" => same(209),
                        "repeating_command_block" => (b.Name, 210, (boolean(BlockState.conditional) << 3) | facing()),
                        "chain_command_block" => (b.Name, 211, (boolean(BlockState.conditional) << 3) | facing()),
                        "frosted_ice" => (b.Name, 212, 0 /*TODO age?*/),
                        "magma_block" => ("magma", 213),
                        "nether_wart_block" => same(214),
                        "red_nether_bricks" => ("red_nether_brick", 215),
                        "bone_block" => (b.Name, 216, 0 /*TODO axis?*/),
                        "structure_void" => same(217),
                        "observer" => (b.Name, 218, (boolean(BlockState.powered) << 3) | facing()),
                        //TODO check out the shulker box ids and dvs
                        //TODO check out the glazed terracotta ids and dvs

                        "structure_block" => (b.Name, 255, structureBlockDV()),
                        _ => warnUnknownBlock()
                    };
            }
        }

        internal static Block ToNewBlock(this OldBlock b) => throw new NotImplementedException();
    }
}
#pragma warning restore CA1307 // Specify StringComparison
