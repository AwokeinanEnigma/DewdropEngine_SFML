#region

using DewDrop;
using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.GUI.Fonts;
using DewDrop.Scenes;
using DewDrop.UserInput;
using DewDrop.Utilities;
using ImGuiNET;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

#endregion

namespace Prototype.Scenes
{
    public class TestScene : SceneBase
    {
        private Player _playerEntity;
        public Text funnyText;
        private ShapeEntity2 overlayEntity;

        private RenderPipeline pipeline;

        private TextRenderer title;
        public EntityManager EntityManager { get; private set; }
        public CollisionManager CollisionManager { get; private set; }

        public TestScene()
        {
            #region Initialize

            funnyText = new Text("swag", new FontData().Font);
            funnyText.FillColor = Color.Red;

            pipeline = new RenderPipeline(Engine.RenderTexture);
            EntityManager = new EntityManager();

            #endregion

            #region Create Entities

            _playerEntity = new Player(
                new RectangleShape(new Vector2f(11, 20)),
                new Vector2(160, 90),
                new Vector2(11, 20),
                new Vector2(0, 0), 90000, pipeline, null, Color.Green, Color.Green);
            EntityManager.AddEntity(_playerEntity);
            pipeline.Add(_playerEntity);


            overlayEntity = new ShapeEntity2(
                new RectangleShape(new Vector2f(1, 1)),
                new Vector2(160, 90),
                new Vector2(20, 20),
                new Vector2(0, 0), 90000, pipeline, Color.Blue, Color.Blue);
            EntityManager.AddEntity(overlayEntity);
            pipeline.Add(overlayEntity);

            #endregion

            Engine.RenderImGUI += EngineOnRenderImGUI;
            DDDebug.DumpLogs();
        }


        private void EngineOnRenderImGUI()
        {
            ImGui.Begin("Dewdrop Debug Utilities");
            ImGui.Text($"Garbage Allocated: {GC.GetTotalMemory(false) / 1024L}KB");
            ImGui.Separator();

            if (ImGui.Button("Force GC Collection")) GC.Collect();
            ImGui.End();
        }


        public override void Focus()
        {
            base.Focus();
            ViewManager.Instance.EntityFollow = _playerEntity;
            ViewManager.Instance.Center = new Vector2(160f, 90f);
        }


        public override void Update()
        {
            base.Update();
            EntityManager.Update();
        }

        public override void Draw()
        {
            pipeline.Draw();
            Engine.RenderTexture.Draw(funnyText);
            base.Draw();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                // dispose here
            }

            base.Dispose(disposing);
        }
    }
}