#region

using fNbt;

#endregion
namespace DewDrop.Maps;

public abstract class ExtraMapLoaderPart {
	public abstract object Load (NbtCompound map);
}
