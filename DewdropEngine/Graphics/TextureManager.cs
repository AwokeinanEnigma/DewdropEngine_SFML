#region

using DewDrop.Utilities;
using fNbt;

#endregion

namespace DewDrop.Graphics;

/// <summary>
///     Class that manages all loaded textures
/// </summary>
public class TextureManager
{
    /// <summary>
    ///     The instance of the texture manager
    /// </summary>
    public static TextureManager Instance => instance;

    private Dictionary<int, int> instances;

    //keeps track of every texture that has ever been loaded within the game session
    private Dictionary<string, int> allFilenameHashes;

    //only has textures that are currently loaded in memory
    private Dictionary<int, string> activeFilenameHashes;

    private Dictionary<int, ITexture> textures;

    private static TextureManager instance = new();

    private TextureManager()
    {
        instances = new Dictionary<int, int>();
        textures = new Dictionary<int, ITexture>();
        allFilenameHashes = new Dictionary<string, int>();
        activeFilenameHashes = new Dictionary<int, string>();
    }

    /// <summary>
    ///     Creates an IndexedTexture from an NBTCompound
    /// </summary>
    /// <param name="root">The NBTCompound to load an IndexedTexture from</param>
    /// <returns></returns>
    private SpritesheetTexture LoadFromNbtTag(NbtCompound root)
    {
        // this code is all dave/carbine code therefore i wil not look at it!
        NbtTag paletteTag = root.Get("pal");
        IEnumerable<NbtTag> palettes = paletteTag is NbtList ? (NbtList)paletteTag : ((NbtCompound)paletteTag).Tags;

        uint intValue = (uint)root.Get<NbtInt>("w").IntValue;
        byte[] byteArrayValue = root.Get<NbtByteArray>("img").ByteArrayValue;
        List<int[]> list = new();
        foreach (NbtTag palette in palettes)
        {
            if (palette.TagType == NbtTagType.IntArray)
            {
                list.Add(((NbtIntArray)palette).IntArrayValue);
            }
        }

        SpriteDefinition spriteDefinition = default;
        Dictionary<int, SpriteDefinition> spriteDefinitions = new();

        NbtCompound allSprites = root.Get<NbtCompound>("spr");
        if (allSprites != null)
        {
            foreach (NbtTag potentialSprite in allSprites.Tags)
            {
                if (potentialSprite is NbtCompound)
                {
                    NbtCompound spriteCompound = (NbtCompound)potentialSprite;
                    string text = spriteCompound.Name.ToLowerInvariant();

                    NbtIntArray dummyIntArray;
                    int[] coordinatesArray = spriteCompound.TryGet("crd", out dummyIntArray) ? dummyIntArray.IntArrayValue : new int[2];
                    int[] boundsArray = spriteCompound.TryGet("bnd", out dummyIntArray) ? dummyIntArray.IntArrayValue : new int[2];
                    int[] originArray = spriteCompound.TryGet("org", out dummyIntArray) ? dummyIntArray.IntArrayValue : new int[2];

                    byte[] optionsArray = spriteCompound.TryGet("opt", out NbtByteArray nbtByteArray) ? nbtByteArray.ByteArrayValue : new byte[3];

                    IList<NbtTag> speedSet = spriteCompound.Get<NbtList>("spd");
                    int frames = spriteCompound.TryGet("frm", out NbtInt nbtInt) ? nbtInt.IntValue : 1;

                    // this is only found on tilesets put through VEMC
                    NbtIntArray dataArray = spriteCompound.Get<NbtIntArray>("d");
                    int[] data = dataArray == null ? null : dataArray.IntArrayValue;

                    Vector2 coords = new(coordinatesArray[0], coordinatesArray[1]);
                    Vector2 bounds = new(boundsArray[0], boundsArray[1]);
                    Vector2 origin = new(originArray[0], originArray[1]);

                    // options are encoded as arrays
                    // you can guess the values from here but i'll elaborate

                    // 0 - flip the sprite horizontally
                    // 1 - flip the sprite vertically 
                    // 2 - animation mode

                    bool flipX = optionsArray[0] == 1;
                    bool flipY = optionsArray[1] == 1;
                    int mode = optionsArray[2];

                    float[] speeds = speedSet != null ? new float[speedSet.Count] : System.Array.Empty<float>();
                    for (int i = 0; i < speeds.Length; i++)
                    {
                        NbtTag speedValue = speedSet[i];
                        speeds[i] = speedValue.FloatValue;
                    }

                    SpriteDefinition newSpriteDefinition = new(text, coords, bounds, origin, frames, speeds, flipX, flipY, mode, data);
                    if (spriteDefinition.Name == "")
                    {
                        spriteDefinition = newSpriteDefinition;
                    }

                    int key = text.GetHashCode();
                    spriteDefinitions.Add(key, newSpriteDefinition);
                }
            }
        }

        return new SpritesheetTexture(intValue, list.ToArray(), byteArrayValue, spriteDefinitions, spriteDefinition);
    }


    /// <summary>
    ///     Returns an IndexTexture by name
    /// </summary>
    /// <param name="spriteFile">The path to load the IndexedTexture from</param>
    /// <returns></returns>
    public SpritesheetTexture Use(string spriteFile)
    {
        // Create hash so we can fetch it later
        int num = spriteFile.GetHashCode(); //Hash.Get(spriteFile);

        // Create IndexTexture variable so we can initialize it later.
        SpritesheetTexture indexedTexture;

        // To save memory, we're not going to load the texture again. Instead, we'll just return an instance from our dictionary of loaded textures.
        if (!textures.ContainsKey(num))
        {
            // We don't need to check if we haven't already added it to our activeFilenameHashes dict, because Unuse() will remove the entry from activeFilenameHashes
            activeFilenameHashes.Add(num, spriteFile);

            // Before adding the texture's sprite to our allFilenameHashes dict, first check if we have already cached it before
            if (!allFilenameHashes.ContainsKey(spriteFile))
            {
                allFilenameHashes.Add(spriteFile, num);
            }


            if (!File.Exists(spriteFile))
            {
                string message = string.Format("The sprite file \"{0}\" does not exist.", spriteFile);
                throw new FileNotFoundException(message, spriteFile);
            }

            NbtFile nbtFile = new(spriteFile);
            indexedTexture = LoadFromNbtTag(nbtFile.RootTag);
            instances.Add(num, 1);
            textures.Add(num, indexedTexture);
        }
        else
        {
            indexedTexture = (SpritesheetTexture)textures[num];
            instances[num] += 1;
            //Debug.Log("returning cached texture");
        }

        return indexedTexture;
    }

    /*public FullColorTexture UseFramebuffer()
    {
        int hashCode = Engine.Frame.GetHashCode();
        
        RenderStates states = new RenderStates(BlendMode.Alpha, Transform.Identity, Engine.FrameBuffer.Texture, null);
        VertexArray vertexArray = new VertexArray(PrimitiveType.Quads, 4U);
        vertexArray[0U] = new Vertex(new Vector2f(0f, 0f), new Vector2f(0f, Engine.SCREEN_HEIGHT));
        vertexArray[1U] = new Vertex(new Vector2f(Engine.SCREEN_WIDTH, 0f), Engine.SCREEN_SIZE);
        vertexArray[2U] = new Vertex(Engine.SCREEN_SIZE, new Vector2f(Engine.SCREEN_WIDTH, 0f));
        vertexArray[3U] = new Vertex(new Vector2f(0f, Engine.SCREEN_HEIGHT), new Vector2f(0f, 0f));

        RenderTexture renderTexture = new RenderTexture(Engine.SCREEN_WIDTH, Engine.SCREEN_HEIGHT);
        renderTexture.Clear(Color.Black);
        renderTexture.Draw(vertexArray, states);
        Texture tex = new Texture(renderTexture.Texture);
        renderTexture.Dispose();
        vertexArray.Dispose();
        FullColorTexture fullColorTexture = new FullColorTexture(tex);
        this.instances.Add(hashCode, 1);
        this.textures.Add(hashCode, fullColorTexture);
        return fullColorTexture;
    }*/

    public void Unuse(ICollection<ITexture> textures)
    {
        if (textures != null)
        {
            foreach (ITexture texture in textures)
            {
                Unuse(texture);
            }
        }
    }

    public void Unuse(ITexture texture)
    {
        foreach (KeyValuePair<int, ITexture> keyValuePair in textures)
        {
            int key = keyValuePair.Key;
            ITexture value = keyValuePair.Value;
            if (value == texture)
            {
                activeFilenameHashes.Remove(key);
                /*Dictionary<int, int> dictionary;
                int key2;
                (dictionary = this.instances)[key2 = key] = dictionary[key2] - 1;*/
                instances[key] -= 1;

                break;
            }
        }
    }

    public void Purge()
    {
        List<int> list = new();
        foreach (KeyValuePair<int, ITexture> keyValuePair in textures)
        {
            int key = keyValuePair.Key;
            ITexture value = keyValuePair.Value;
            if (value != null && instances[key] <= 0)
            {
                list.Add(key);
            }
        }

        foreach (int key2 in list)
        {
            textures[key2].Dispose();
            textures[key2] = null;
            instances.Remove(key2);
            textures.Remove(key2);
        }
    }

    public void DumpEveryLoadedTexture()
    {
        List<string> textures = new();
        foreach (KeyValuePair<string, int> keyntex in allFilenameHashes)
        {
            textures.Add($"name == '{keyntex.Key}' :: hash == '{keyntex.Value}'");
        }

        StreamWriter streamWriter = new("Data/Logs/all_loaded_textures.log");
        textures.ForEach(x => streamWriter.WriteLine(x));
        streamWriter.Close();
    }

    public void DumpLoadedTextures()
    {
        List<string> textures = new();
        foreach (KeyValuePair<int, string> keyntex in activeFilenameHashes)
        {
            textures.Add($"name == '{keyntex.Value}' :: hash == '{keyntex.Key}'");
        }

        StreamWriter streamWriter = new("Data/Logs/loaded_textures.log");
        textures.ForEach(x => streamWriter.WriteLine(x));
        streamWriter.Close();
    }
}