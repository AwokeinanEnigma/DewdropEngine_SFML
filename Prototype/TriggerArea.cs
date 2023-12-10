using DewDrop.Collision;
using DewDrop.Updatable;
using DewDrop.Utilities;
using DewDrop.Wren;
using SFML.Graphics;
using SFML.System;

namespace Prototype
{
	public class TriggerArea : ICollidable, IDisposable
	{
		public Vector2 Position
		{
			get => this._position;
			set => this._position = value;
		}

		public Vector2 Velocity => Vector2.ZERO_VECTOR;

		public AABB AABB => this._mesh.AABB;

		public Mesh Mesh => this._mesh;

		public bool Solid
		{
			get => this._solid;
			set => this._solid = value;
		}

		public int Flag => this._flag;

		public string Script => this._script;
		
		public bool IsTrigger { get; set; }

		public Wreno Wreno { get; set; }
		public VertexArray DebugVerts { get; private set; }
		bool _triggered;
		bool _lastTriggered;

		 Vector2 _position;
		 readonly Mesh _mesh;
		 bool _solid;
		 readonly int _flag;
		 readonly string _script;
		 public int Priority => 200;
		 
		 public TriggerArea(Vector2 position, List<Vector2> points, int flag, string script)
		{
			this._position = position;
			this._mesh = new Mesh(points);
			this._flag = flag;
			this._script = script;
			CollidingWith = new List<ICollidable>();
			string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), $"{script}.wren");
			string scriptContents = File.ReadAllText(scriptPath);
			Wreno = WrenManager.MakeWreno(scriptContents);
			Wreno.Run();

			IsTrigger = true;
			this._solid = true;

			uint vertexCount = (uint)(this._mesh.Vertices.Count + 1);
			VertexArray vertexArray = new VertexArray(PrimitiveType.LineStrip, vertexCount);

			for (int i = 0; i < this._mesh.Vertices.Count; i++)
			{
				vertexArray[(uint)i] = new Vertex(this._mesh.Vertices[i], Color.Magenta);
			}

			vertexArray[vertexCount - 1] = new Vertex(this._mesh.Vertices[0], Color.Magenta);
			this.DebugVerts = vertexArray;
		}


		 public void OnTriggerStay (ICollidable context) {
			 //Outer.Log($"OnTriggerStay {context}");
		 }

		public void OnTriggerEnter (ICollidable context) {
			Wreno.Call("onTriggerEnter");
		}
		public void OnTriggerExit (ICollidable context) {
			Wreno.Call("onTriggerExit");
		}
		public List<ICollidable> CollidingWith { get; }
		public void Dispose () {
			Wreno.Dispose();
			DebugVerts.Dispose();
		}
	}
}
