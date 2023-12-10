namespace DewDrop.Wren; 

/// <summary>
/// If you don't want a class, method, property, constructor, or field serialized by the WrenWrapperGenerator, add this attribute to it.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Field)]
public class WrenBlackList : Attribute
{
}