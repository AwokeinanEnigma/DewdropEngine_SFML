using DewDrop.Entities;
using DewDrop.Inspector;
using System.Collections;
using System.Reflection;
/// <summary>
/// Represents a custom painter in the Inspector.
/// </summary>
/// <remarks>
/// A custom painter is used to customize the way certain fields or properties are displayed and edited in the Inspector.
/// Each custom painter is associated with a specific type and provides methods for painting fields and properties of that type.
/// </remarks>
public abstract class CustomPainter {
    /// <summary>
    /// Gets or sets the type that the custom painter supports.
    /// </summary>
    public abstract Type Type { get; set; }

    /// <summary>
    /// Paints a field of the supported type.
    /// </summary>
    /// <param name="field">The field to be painted.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="entity">The entity that the field belongs to.</param>
    /// <param name="inspector">The Inspector instance.</param>
    /// <param name="history">The CommandHistory instance.</param>
    public abstract void PaintField (FieldInfo field, object value, Entity entity, Inspector inspector, CommandHistory history);

    /// <summary>
    /// Paints a property of the supported type.
    /// </summary>
    /// <param name="property">The property to be painted.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="entity">The entity that the property belongs to.</param>
    /// <param name="inspector">The Inspector instance.</param>
    /// <param name="history">The CommandHistory instance.</param>
    public abstract void PaintProperty (PropertyInfo property, object value, Entity entity, Inspector inspector, CommandHistory history);

    /// <summary>
    /// Paints a list of the supported type.
    /// </summary>
    /// <param name="list">The list to be painted.</param>
    /// <param name="index">The index of the element in the list.</param>
    /// <param name="entity">The entity that the list belongs to.</param>
    /// <param name="inspector">The Inspector instance.</param>
    /// <param name="history">The CommandHistory instance.</param>
    public abstract void PaintList (IList list, int index, Entity entity, Inspector inspector, CommandHistory history);

    /// <summary>
    /// Adds an element to a list of the supported type.
    /// </summary>
    /// <param name="list">The list to which an element will be added.</param>
    /// <param name="entity">The entity that the list belongs to.</param>
    /// <param name="inspector">The Inspector instance.</param>
    /// <param name="history">The CommandHistory instance.</param>
    public abstract void AddListElement (IList list, Entity entity, Inspector inspector, CommandHistory history);

    /// <summary>
    /// Disposes the custom painter, cleaning up any resources it is using.
    /// </summary>
    public abstract void Dispose ();
}