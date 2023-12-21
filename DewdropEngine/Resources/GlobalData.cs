using DewDrop.Utilities;
using fNbt;
using System.Collections.Generic;

namespace DewDrop.Resources;

public static class GlobalData
{
	static readonly Dictionary<string, object> _Data = new Dictionary<string, object>();

	public static void WriteBool(string key, bool value)
	{
			
		_Data[key] = value;
	}

	public static bool GetBool(string key)
	{
		if (_Data.TryGetValue(key, out object value))
		{
			return (bool)value;
		}
		return false;
	}

	public static void WriteInt(string key, int value)
	{
		_Data[key] = value;
	}

	public static int GetInt(string key)
	{
		if (_Data.TryGetValue(key, out object value))
		{
			return (int)value;
		}
		return 0;
	}

	public static void WriteFloat(string key, float value)
	{
			
		_Data[key] = value;
	}

	public static float GetFloat(string key)
	{
		if (_Data.TryGetValue(key, out object value))
		{
			return (float)value;
		}
		return 0f;
	}

	public static void WriteString(string key, string value)
	{
		_Data[key] = value;
	}

	public static string GetString(string key)
	{
		if (_Data.TryGetValue(key, out object value))
		{
			return (string)value;
		}
		return "";
	}
		
	internal static void LoadFromNbt(NbtCompound compound)
	{
		_Data.Clear();
		foreach (NbtTag tag in compound)
		{
			switch (tag)
			{
			case NbtByte nbtByte:
				_Data.Add(tag.Name,nbtByte.Value != 0);
				break;
			case NbtInt nbtInt:
				_Data.Add(tag.Name, nbtInt.Value);
				break;
			case NbtFloat nbtFloat:
				_Data.Add(tag.Name, nbtFloat.Value);
				break;
			case NbtString nbtString:
				_Data.Add(tag.Name, nbtString.Value);
				break;
			default:
				Outer.LogError($"Unknown tag type {tag.GetType().Name} in GlobalData!", null);
				break;
			}
		}
	}
		
	internal static NbtCompound SerializeToNbt()
	{
		NbtCompound compound = new NbtCompound();

		foreach (KeyValuePair<string, object> entry in _Data)
		{
			if (entry.Value is bool)
			{
				compound.Add(new NbtByte(entry.Key, (bool)entry.Value ? (byte)1 : (byte)0));
			}
			else if (entry.Value is int)
			{
				compound.Add(new NbtInt(entry.Key, (int)entry.Value));
			}
			else if (entry.Value is float)
			{
				compound.Add(new NbtFloat(entry.Key, (float)entry.Value));
			}
			else if (entry.Value is string)
			{
				compound.Add(new NbtString(entry.Key, (string)entry.Value));
			}
		}

		compound.Name = "GlobalData";
		return compound;
	}

}