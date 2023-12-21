#region

using DewDrop.Utilities;
using fNbt;
using SFML.Graphics;
using SFML.System;

#endregion

namespace DewDrop.Graphics;

/// <summary>
/// Manages all loaded textures in the game.
/// </summary>
public class TextureManager {
    /// <summary>
    ///     The instance of the texture manager
    /// </summary>
    public static TextureManager Instance { get; } = new TextureManager();

    // this keeps track of how many instances of a texture are currently in use
    // so esssentially, if you have 5 instances of a texture, this dictionary will have a value of 5 for that texture
    // Purge() will remove any textures that have a value of 0, meaning that they are not in use
    readonly Dictionary<int, int> _instances;

	//keeps track of every texture that has ever been loaded within the game session
	readonly Dictionary<string, int> _allFilenameHashes;

	//only has textures that are currently loaded in memory
	readonly Dictionary<int, string> _activeFilenameHashes;

	readonly Dictionary<int, ITexture> _textures;

	
	public Dictionary<int, int> Instances => _instances;
	public Dictionary<int,string > ActiveTextures => _instances.ToDictionary(x => x.Key, x => _activeFilenameHashes[x.Key]);
	
	TextureManager () {
		_instances = new Dictionary<int, int>();
		_textures = new Dictionary<int, ITexture>();
		_allFilenameHashes = new Dictionary<string, int>();
		_activeFilenameHashes = new Dictionary<int, string>();
		
	}

	/// <summary>
	/// Loads a spritesheet texture from an NBT tag.
	/// </summary>
	/// <param name="root">The root NBT compound tag.</param>
	/// <param name="spriteFile">The file path of the sprite.</param>
	/// <returns>The loaded SpritesheetTexture.</returns>
	SpritesheetTexture LoadFromNbtTag (NbtCompound root, string spriteFile) {
		// this code is all dave/carbine code therefore i wil not look at it!
		NbtTag paletteTag = root.Get("pal");
		IEnumerable<NbtTag> palettes = paletteTag as NbtList ?? ((NbtCompound)paletteTag).Tags;

		uint intValue = (uint)root.Get<NbtInt>("w").IntValue;
		byte[] byteArrayValue = root.Get<NbtByteArray>("img").ByteArrayValue;
		List<int[]> list = new List<int[]>();
		foreach (NbtTag palette in palettes) {
			if (palette.TagType == NbtTagType.IntArray) {
				list.Add(((NbtIntArray)palette).IntArrayValue);
			}
		}

		SpriteDefinition spriteDefinition = default;
		Dictionary<int, SpriteDefinition> spriteDefinitions = new Dictionary<int, SpriteDefinition>();

		NbtCompound allSprites = root.Get<NbtCompound>("spr");
		if (allSprites != null) {
			foreach (NbtTag potentialSprite in allSprites.Tags) {
				if (potentialSprite is NbtCompound spriteCompound) {
					string text = spriteCompound.Name.ToLowerInvariant();

					int[] coordinatesArray = spriteCompound.TryGet("crd", out NbtIntArray dummyIntArray) ? dummyIntArray.IntArrayValue : new int[2];
					int[] boundsArray = spriteCompound.TryGet("bnd", out dummyIntArray) ? dummyIntArray.IntArrayValue : new int[2];
					int[] originArray = spriteCompound.TryGet("org", out dummyIntArray) ? dummyIntArray.IntArrayValue : new int[2];

					byte[] optionsArray = spriteCompound.TryGet("opt", out NbtByteArray nbtByteArray) ? nbtByteArray.ByteArrayValue : new byte[3];

					IList<NbtTag> speedSet = spriteCompound.Get<NbtList>("spd");
					int frames = spriteCompound.TryGet("frm", out NbtInt nbtInt) ? nbtInt.IntValue : 1;

					// this is only found on tilesets put through VEMC
					NbtIntArray dataArray = spriteCompound.Get<NbtIntArray>("d");
					int[] data = dataArray?.IntArrayValue;

					Vector2 coords = new Vector2(coordinatesArray[0], coordinatesArray[1]);
					Vector2 bounds = new Vector2(boundsArray[0], boundsArray[1]);
					Vector2 origin = new Vector2(originArray[0], originArray[1]);

					// options are encoded as arrays
					// you can guess the values from here but i'll elaborate

					// 0 - flip the sprite horizontally
					// 1 - flip the sprite vertically 
					// 2 - animation mode

					bool flipX = optionsArray[0] == 1;
					bool flipY = optionsArray[1] == 1;
					int mode = optionsArray[2];

					float[] speeds = speedSet != null ? new float[speedSet.Count] : Array.Empty<float>();
					for (int i = 0; i < speeds.Length; i++) {
						NbtTag speedValue = speedSet[i];
						speeds[i] = speedValue.FloatValue;
					}

					SpriteDefinition newSpriteDefinition = new SpriteDefinition(text, coords, bounds, origin, frames, speeds, flipX, flipY, mode, data);
					if (spriteDefinition.Name == string.Empty) {
						spriteDefinition = newSpriteDefinition;
					}

					int key = text.GetHashCode();
					spriteDefinitions.Add(key, newSpriteDefinition);
				}
			}
		}

		return new SpritesheetTexture(intValue, list.ToArray(), byteArrayValue, spriteDefinitions, spriteDefinition, spriteFile);
	}

	/// <summary>
	/// Retrieves raw spritesheet data.
	/// </summary>
	/// <param name="spriteFile">The file path of the sprite.</param>
	/// <returns>A tuple containing the byte array of the spritesheet and an array of palette data.</returns>
	public Tuple<byte[], int[][]> GetRawSpritesheetData (string spriteFile) {
		NbtFile nbtFile = new NbtFile(spriteFile);
		NbtCompound root = nbtFile.RootTag;
		NbtTag paletteTag = root.Get("pal");
		IEnumerable<NbtTag> palettes = paletteTag as NbtList ?? ((NbtCompound)paletteTag).Tags;

		byte[] byteArrayValue = root.Get<NbtByteArray>("img").ByteArrayValue;
		List<int[]> list = new List<int[]>();
		foreach (NbtTag palette in palettes) {
			if (palette.TagType == NbtTagType.IntArray) {
				list.Add(((NbtIntArray)palette).IntArrayValue);
			}
		}
		var data = new Tuple<byte[], int[][]>(byteArrayValue, list.ToArray());
		return data;
	}

	/// <summary>
	/// Reloads all textures.
	/// </summary>
	/// <remarks>
	/// Uses a lot of memory, so only use this when you need to. Probably best to only use this during development rather than release.
	/// </remarks>
	public void ReloadTextures () {
		foreach (KeyValuePair<int, ITexture> keyValuePair in _textures) {
			ITexture value = keyValuePair.Value;
			value.Reload();
		}
	}

	/// <summary>
	/// Returns a SpritesheetTexture object by name.
	/// </summary>
	/// <param name="spriteFile">The path to load the IndexedTexture from.</param>
	/// <returns>A SpritesheetTexture object.</returns>
	public SpritesheetTexture UseSpritesheet(string spriteFile) {
	    // Create hash so we can fetch it later
	    int num = spriteFile.GetHashCode(); //Hash.Get(spriteFile);

	    // Create IndexTexture variable so we can initialize it later.
	    SpritesheetTexture indexedTexture;

	    // To save memory, we're not going to load the texture again. Instead, we'll just return an instance from our dictionary of loaded textures.
	    if (!_textures.ContainsKey(num)) {



		    string message = $"The sprite file \"{spriteFile}\" does not exist.";
		    if (!File.Exists(spriteFile)) {
			    throw new FileNotFoundException(message, spriteFile);
		    }

		    NbtFile nbtFile = new NbtFile(spriteFile);
		    indexedTexture = LoadFromNbtTag(nbtFile.RootTag, spriteFile);
		    
		    _instances.Add(num, 1);
		    _textures.Add(num, indexedTexture);
		    
		    // We don't need to check if we haven't already added it to our activeFilenameHashes dict, because Unuse() will remove the entry from activeFilenameHashes
		    _activeFilenameHashes.Add(num, spriteFile);

		    // Before adding the texture's sprite to our allFilenameHashes dict, first check if we have already cached it before
		    _allFilenameHashes.TryAdd(spriteFile, num);
		    
	    } else {
		    indexedTexture = (SpritesheetTexture)_textures[num];
		    _instances[num] += 1;
		    //Debug.Log("returning cached texture");
	    }

	    return indexedTexture;
    }
    
	/// <summary>
	/// Retrieves or creates an AsepriteTexture from a sprite file.
	/// </summary>
	/// <param name="spriteFile">The path to the sprite file.</param>
	/// <returns>An AsepriteTexture instance.</returns>
	public AsepriteTexture UseAsepriteTexture (string spriteFile) {
	    // Create hash so we can fetch it later
	    int num = spriteFile.GetHashCode(); //Hash.Get(spriteFile);

	    // Create IndexTexture variable so we can initialize it later.
	    AsepriteTexture indexedTexture;

	    // To save memory, we're not going to load the texture again. Instead, we'll just return an instance from our dictionary of loaded textures.
	    if (!_textures.ContainsKey(num)) {
		    // We don't need to check if we haven't already added it to our activeFilenameHashes dict, because Unuse() will remove the entry from activeFilenameHashes
		    _activeFilenameHashes.TryAdd(num, spriteFile);

		    // Before adding the texture's sprite to our allFilenameHashes dict, first check if we have already cached it before
		    _allFilenameHashes.TryAdd(spriteFile, num);


		    string message = $"The sprite file \"{spriteFile}\" does not exist.";
		    if (!File.Exists(spriteFile)) {
			    throw new FileNotFoundException(message, spriteFile);
		    }

		    indexedTexture = new AsepriteTexture(spriteFile);
		    _instances.Add(num, 1);
		    _textures.Add(num, indexedTexture);
	    } else {
		    indexedTexture = (AsepriteTexture)_textures[num];
		    _instances[num] += 1;
		    //Debug.Log("returning cached texture");
	    }

	    return indexedTexture;
    }
	
	/// <summary>
	/// Creates a TextureHolder from the framebuffer.
	/// </summary>
	/// <returns>A TextureHolder made from the framebuffer.</returns>
	public TextureHolder UseFramebuffer()
    {
	    int hashCode = Engine.Frame.GetHashCode();
	    
	    RenderStates states = new RenderStates(BlendMode.Alpha, Transform.Identity, Engine.RenderTexture.Texture, null);
	    VertexArray vertexArray = new VertexArray(PrimitiveType.Quads, 4U);
	    vertexArray[0U] = new Vertex(new Vector2f(0f, 0f), new Vector2f(0f, Engine.ScreenSize.y));
	    vertexArray[1U] = new Vertex(new Vector2f(Engine.ScreenSize.x, 0f), Engine.ScreenSize);
	    vertexArray[2U] = new Vertex(Engine.ScreenSize, new Vector2f(Engine.ScreenSize.x, 0f));
	    vertexArray[3U] = new Vertex(new Vector2f(0f, Engine.ScreenSize.x), new Vector2f(0f, 0f));

	    RenderTexture renderTexture = new RenderTexture((uint)Engine.ScreenSize.x, (uint)Engine.ScreenSize.y);
	    renderTexture.Clear(Color.Black);
	    renderTexture.Draw(vertexArray, states);
	    Texture tex = new Texture(renderTexture.Texture);
	    renderTexture.Dispose();
	    vertexArray.Dispose();
	    TextureHolder fullColorTexture = new TextureHolder(tex);
	    _instances.Add(hashCode, 1);
	    _textures.Add(hashCode, fullColorTexture);
	    return fullColorTexture;
    }

	/// <summary>
	/// Unuses a collection of ITexture objects.
	/// </summary>
	/// <param name="textures">The collection of ITexture objects to unuse.</param>
	public void Unuse (ICollection<ITexture> textures) {
		if (textures != null) {
			foreach (ITexture texture in textures) {
				Unuse(texture);
			}
		}
	}

	/// <summary>
	/// Removes the given texture from the list of active textures.
	/// </summary>
	/// <param name="texture">The texture to be removed.</param>
	public void Unuse(ITexture texture)
	{
		// Iterate through the textures dictionary
		foreach (KeyValuePair<int, ITexture> keyValuePair in _textures)
		{
			int key = keyValuePair.Key;
			ITexture value = keyValuePair.Value;
			// Check if the current texture matches the one to be removed
			if (value == texture)
			{
				// Decrease the count of instances for the given key
				_instances[key] -= 1;
				break;
			}
		}
	}

	/// <summary>
	/// Removes unused textures from memory.
	/// </summary>
	public void Purge () {
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, ITexture> keyValuePair in _textures) {
			int key = keyValuePair.Key;
			//Outer.Log(_allFilenameHashes.First(x => x.Value == key).Key);
			ITexture value = keyValuePair.Value;
			/*if (value is AsepriteTexture asepriteTexture) {
				//Outer.Log(asepriteTexture + " is an aseprite texture with instances " + _instances[key]);
			}*/
			if (value != null && _instances[key] <= 0) {
				list.Add(key);
			//	Outer.Log($"Purging texture {_allFilenameHashes.First(x => x.Value == key).Key} with hash { key}");
			}
		}

		foreach (int key2 in list) {
			_textures[key2].Dispose();
			_textures[key2] = null;
			_instances.Remove(key2);
			_textures.Remove(key2);
			_activeFilenameHashes.Remove(key2);
		}
	}

	/// <summary>
	/// Dumps every loaded texture to a log file.
	/// </summary>
	public void DumpEveryLoadedTexture () {
		List<string> textures = new List<string>();
		foreach (KeyValuePair<string, int> keyntex in _allFilenameHashes) {
			textures.Add($"name == '{keyntex.Key}' :: hash == '{keyntex.Value}'");
		}

		StreamWriter streamWriter = new StreamWriter("Data/Logs/all_loaded_textures.log");
		textures.ForEach(x => streamWriter.WriteLine(x));
		streamWriter.Close();
	}

	/// <summary>
	/// Dumps the loaded textures to a log file.
	/// </summary>
	public void DumpLoadedTextures () {
		List<string> textures = new List<string>();
		foreach (KeyValuePair<int, string> keyntex in _activeFilenameHashes) {
			textures.Add($"name == '{keyntex.Value}' :: hash == '{keyntex.Key}'");
		}


		StreamWriter streamWriter = new StreamWriter("loaded_textures.log");
		textures.ForEach(x => streamWriter.WriteLine(x));
		streamWriter.Close();
	}
}
