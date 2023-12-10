#region

using DewDrop.Collision;
using DewDrop.Maps.MapData;

#endregion
namespace DewDrop.Maps;

public struct Map {
	public Map () {}

    /// <summary>
    ///     The title of the map.
    /// </summary>
    public string Title = "Unloaded";
    /// <summary>
    ///     The map's subtitle.
    /// </summary>
    public string Subtitle = "Unknown";
    /// <summary>
    ///     The map's width in pixels
    /// </summary>
    public int Width = 0;
    /// <summary>
    ///     The map's height in pixels
    /// </summary>
    public int Height = 0;
    /// <summary>
    ///     A list containing data that's used to make tile chunks.
    /// </summary>
    public List<TileChunkData> TileChunkData = new List<TileChunkData>();
	public List<Mesh> Collisions = new List<Mesh>();
	public List<Trigger> Triggers = new List<Trigger>();
}
