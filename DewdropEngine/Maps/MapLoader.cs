#region

using DewDrop.Collision;
using DewDrop.Utilities;
using fNbt;

#endregion

namespace DewDrop.Maps;

public class MapLoader
{
    private string _mapName;

    public MapLoader(string mapName)
    {
        _mapName = mapName;
    }

    public Map Load()
    {
        // Ensure the file exists
        if (!File.Exists(_mapName))
        {
            throw new FileNotFoundException("Cannot find map file: \"" + _mapName + "\"");
        }

        // No checks needed, nbt will check if its an actual nbt file and then throw an exception if it isn't.
        NbtFile file = new(_mapName);
        NbtCompound root = file.RootTag;

        Map map = new();
        // we pass as ref so we can add to the list
        LoadCoreInformation(root, ref map);

        return map;
    }

    public Map Load(string mapName)
    {
        // Ensure the file exists
        if (!File.Exists(mapName))
        {
            throw new FileNotFoundException("Cannot find map file: \"" + mapName + "\"");
        }

        // No checks needed, nbt will check if its an actual nbt file and then throw an exception if it isn't.
        NbtFile file = new(mapName);
        NbtCompound root = file.RootTag;

        Map map = new();
        LoadCoreInformation(root, ref map);

        return map;
    }

    private void LoadCoreInformation(NbtCompound root, ref Map map)
    {
        LoadHeader(root, ref map);
        LoadTileChunks(root, ref map.TileChunkData);
        LoadCollisions(root, ref map.Collisions);
    }

    private void LoadHeader(NbtCompound root, ref Map map)
    {
        NbtCompound head = root.Get<NbtCompound>("head");

        map.Title = head.Get<NbtString>("title").Value;
        map.Subtitle = head.Get<NbtString>("subtitle").Value;
        map.Width = head.Get<NbtInt>("width").Value;
        map.Height = head.Get<NbtInt>("height").Value;
    }

    private void LoadTileChunks(NbtCompound mapTag, ref List<TileChunkData> info)
    {
        NbtTag tileTag = mapTag.Get("tiles");

        // i fucking love pattern matching!
        if (tileTag is ICollection<NbtTag> tileGroupCollection)
        {
            foreach (NbtTag tileGroupTag in tileGroupCollection)
            {
                if (tileGroupTag is NbtCompound tileGroup)
                {
                    int depth = tileGroup.Get<NbtInt>("depth").Value;
                    int xPosition = tileGroup.Get<NbtInt>("x").Value;
                    int yPosition = tileGroup.Get<NbtInt>("y").Value;
                    int width = tileGroup.Get<NbtInt>("w").Value;

                    byte[] tileByteArray = tileGroup.Get<NbtByteArray>("tiles").Value;
                    ushort[] tiles = new ushort[tileByteArray.Length / 2];
                    Buffer.BlockCopy(tileByteArray, 0, tiles, 0, tileByteArray.Length);

                    TileChunkData newTileGroup = new()
                    {
                        Depth = (uint)depth,
                        X = xPosition,
                        Y = yPosition,
                        Width = width,
                        Height = tiles.Length / 2 / width,
                        Tiles = tiles
                    };
                    info.Add(newTileGroup);
                }
            }
        }
    }

    private static void LoadCollisions(NbtCompound mapTag,  ref List<Mesh>  meshes)
    {
        NbtTag nbtTag = mapTag.Get("mesh");
        if (!(nbtTag is ICollection<NbtTag>))
        {
            return;
        }

        foreach (NbtList nbtList in nbtTag as IEnumerable<NbtTag>)
        {
            List<Vector2> points = new List<Vector2>();
            for (int tagIndex = 0; tagIndex < nbtList.Count; tagIndex += 2)
            {
                int x = nbtList.Get<NbtInt>(tagIndex).Value;
                int y = nbtList.Get<NbtInt>(tagIndex + 1).Value;
                points.Add(new Vector2(x, y));
            }

            Mesh mesh = new(points);
            meshes.Add(mesh);
        }
    }
}