using DewDrop.Utilities;
namespace DewDrop.Maps.MapData; 

public struct Trigger {
	public string Script;
	public Vector2 Position;
	public List<Vector2> Points = new List<Vector2>(); 
	public int Flag;

	public Trigger (string script, Vector2 position, List<Vector2> points, int flag) {
		Script = script;
		Position = position;
		Points = points;
		Flag = flag;
	}

}
