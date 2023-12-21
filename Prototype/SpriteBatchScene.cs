using Californium;
using DewDrop;
using DewDrop.Graphics;
using DewDrop.Scenes;
using DewDrop.Utilities;
using SFML.Graphics;
namespace Prototype; 

public class SpriteBatchScene : SceneBase{
SpriteBatch _batch;
SpriteGraphic texture;
BatchedSprite sprite;
	public SpriteBatchScene () { }
	public override void Focus () {
		base.Focus(); 
		_batch = new SpriteBatch();
		this.texture = new SpriteGraphic("C:\\\\Users\\\\Tom\\\\Documents\\\\bear.dat", "walk north", new Vector2(160, 90), 100);
		sprite = new BatchedSprite( new Texture("C:/Users/Tom/Documents/workbin/prayedontheimmovable.png"));
		sprite.Position = new Vector2(160, 90);
		sprite.Origin = new Vector2(320,180);
	}
	public override void TransitionIn () { base.TransitionIn(); }
	public override void Unfocus () { base.Unfocus(); }
	public override void Update () { base.Update(); }
	public override void Draw () {
		base.Draw();
		_batch.Begin();
		for (int i = 0; i < 30; i++) {
			sprite.Position = new Vector2(160 + (i * 8), 90 + (i * 8));
			sprite.Origin = new Vector2(320 + (i*8),180 + (i*8));
			_batch.Draw(sprite);
		}
		_batch.End(Engine.RenderTexture, RenderStates.Default);
	}
	protected override void Dispose (bool disposing) { base.Dispose(disposing); }
}
