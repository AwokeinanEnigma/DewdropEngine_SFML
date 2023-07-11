#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Collision;

public class Mesh
{
    private AABB aabb;

    /// <summary>
    ///     Creates a new mesh
    /// </summary>
    /// <param name="points">The bounds of the mesh</param>
    public Mesh(List<Vector2> points)
    {
        AddPoints(points);
    }

    /// <summary>
    ///     Creates a new mesh from a FloatRect
    /// </summary>
    /// <param name="rectangle">The bounds of the mesh </param>
    public Mesh(FloatRect rectangle)
    {
        AddRectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
    }

    /// <summary>
    ///     Creates a new mesh from an IntRect
    /// </summary>
    /// <param name="rectangle">The bounds of the mesh </param>
    public Mesh(IntRect rectangle)
    {
        AddRectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
    }

    /// <summary>
    ///     The vertices of the mesh
    /// </summary>
    public List<Vector2> Vertices { get; private set; }
    /// <summary>
    ///     Edges of the mesh
    /// </summary>
    public List<Vector2> Edges { get; private set; }
    /// <summary>
    ///     The normals of the mesh
    /// </summary>
    public List<Vector2> Normals { get; private set; }

    public AABB AABB => aabb;

    /// <summary>
    ///     The center of the mesh
    /// </summary>
    public Vector2 Center => new(aabb.Size.X / 2f, aabb.Size.Y / 2f);

    public void Destroy()
    {
        Vertices.Clear();
        Vertices = null;
        Edges.Clear();
        Edges = null;
    }

    private void AddPoints(List<Vector2> points)
    {
        Vertices = new List<Vector2>();
        Edges = new List<Vector2>();
        Normals = new List<Vector2>();
        for (int i = 0; i < points.Count; i++)
        {
            Vertices.Add(points[i]);
            int index = (i + 1) % points.Count;
            float x = points[index].X - points[i].X;
            float y = points[index].Y - points[i].Y;

            Vector2 Vector2 = new(x, y);
            Edges.Add(Vector2);

            Vector2 item = Vector2.RightNormal(Vector2);
            Normals.Add(item);
        }

        aabb = GetAABB();
    }

    private void AddRectangle(float x, float y, float width, float height)
    {
        AddPoints(new List<Vector2>
        {
            new(x, y),
            new(x + width, y),
            new(x + width, y + height),
            new(x, y + height)
        });
    }

    private AABB GetAABB()
    {
        float num = float.MinValue;
        float num2 = float.MinValue;
        float num3 = float.MaxValue;
        float num4 = float.MaxValue;
        foreach (Vector2 Vector2 in Vertices)
        {
            num3 = Vector2.X < num3 ? Vector2.X : num3;
            num = Vector2.X > num ? Vector2.X : num;
            num4 = Vector2.Y < num4 ? Vector2.Y : num4;
            num2 = Vector2.Y > num2 ? Vector2.Y : num2;
        }

        return new AABB(new Vector2(num3, num4), new Vector2(num - num3, num2 - num4));
    }
}