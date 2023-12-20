#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Collision;

/// <summary>
/// Represents a Mesh used for collision detection.
/// </summary>
public class Mesh {
	/// <summary>
	/// The Axis-Aligned Bounding Box (AABB) of the Mesh.
	/// </summary>
	AABB _aabb;

	/// <summary>
	/// Initializes a new instance of the Mesh class with specified points.
	/// </summary>
	/// <param name="points">The points of the Mesh.</param>
	public Mesh (List<Vector2> points) {
		AddPoints(points);
	}

	/// <summary>
	/// Initializes a new instance of the Mesh class with specified rectangle.
	/// </summary>
	/// <param name="rectangle">The rectangle of the Mesh.</param>
	public Mesh (FloatRect rectangle) {
		AddRectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
	}

	/// <summary>
	/// Initializes a new instance of the Mesh class with specified rectangle.
	/// </summary>
	/// <param name="rectangle">The rectangle of the Mesh.</param>
	public Mesh (IntRect rectangle) {
		AddRectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
	}

	/// <summary>
	/// The vertices of the Mesh.
	/// </summary>
	public List<Vector2> Vertices { get; private set; }

	/// <summary>
	/// The edges of the Mesh.
	/// </summary>
	public List<Vector2> Edges { get; private set; }

	/// <summary>
	/// The normals of the Mesh.
	/// </summary>
	public List<Vector2> Normals { get; private set; }

	/// <summary>
	/// The Axis-Aligned Bounding Box (AABB) of the Mesh.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public AABB AABB => _aabb;

	/// <summary>
	/// The center of the Mesh.
	/// </summary>
	public Vector2 Center => new Vector2(_aabb.Size.X/2f, _aabb.Size.Y/2f);

	/// <summary>
	/// Destroys the Mesh.
	/// </summary>
	public void Destroy () {
		Vertices.Clear();
		Vertices = null;
		Edges.Clear();
		Edges = null;
	}

	/// <summary>
	/// Adds a list of points to the Mesh.
	/// </summary>
	/// <param name="points">The points to add to the Mesh.</param>
	void AddPoints (List<Vector2> points) {
		Vertices = new List<Vector2>();
		Edges = new List<Vector2>();
		Normals = new List<Vector2>();
		for (int i = 0; i < points.Count; i++) {
			Vertices.Add(points[i]);
			int index = (i + 1)%points.Count;
			float x = points[index].X - points[i].X;
			float y = points[index].Y - points[i].Y;

			Vector2 vector2 = new Vector2(x, y);
			Edges.Add(vector2);

			Vector2 item = Vector2.RightNormal(vector2);
			Normals.Add(item);
		}

		_aabb = GetAABB();
	}

	/// <summary>
	/// Adds a rectangle to the Mesh.
	/// </summary>
	/// <param name="x">The x-coordinate of the rectangle.</param>
	/// <param name="y">The y-coordinate of the rectangle.</param>
	/// <param name="width">The width of the rectangle.</param>
	/// <param name="height">The height of the rectangle.</param>
	void AddRectangle (float x, float y, float width, float height) {
		AddPoints(new List<Vector2> {
			new Vector2(x, y),
			new Vector2(x + width, y),
			new Vector2(x + width, y + height),
			new Vector2(x, y + height)
		});
	}

	/// <summary>
	/// Gets the Axis-Aligned Bounding Box (AABB) of the Mesh.
	/// </summary>
	/// <returns>The AABB of the Mesh.</returns>
	// ReSharper disable once InconsistentNaming
	AABB GetAABB () {
		float xMinValue = float.MinValue;
		float yMinValue = float.MinValue;
		float xMaxValue = float.MaxValue;
		float yMaxValue = float.MaxValue;
		foreach (Vector2 vertex in Vertices) {
			xMaxValue = vertex.X < xMaxValue ? vertex.X : xMaxValue;
			xMinValue = vertex.X > xMinValue ? vertex.X : xMinValue;
			yMaxValue = vertex.Y < yMaxValue ? vertex.Y : yMaxValue;
			yMinValue = vertex.Y > yMinValue ? vertex.Y : yMinValue;
		}

		return new AABB(new Vector2(xMaxValue, yMaxValue), new Vector2(xMinValue - xMaxValue, yMinValue - yMaxValue));
	}
}
